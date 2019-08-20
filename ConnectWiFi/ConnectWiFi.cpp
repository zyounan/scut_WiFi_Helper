#include "stdafx.h"
using namespace std;
using ustring = basic_string<unsigned char>;
string APName = "scut-student";
HANDLE hClient = nullptr;
DWORD dwMaxClient = 2;
DWORD dwCurVersion = 0;
DWORD dwResult = 0;
DWORD dwRetVal = 0;
int iRet = 0;
unsigned int i, j, k;
PWLAN_INTERFACE_INFO_LIST pIfList = NULL;
PWLAN_INTERFACE_INFO pIfInfo = NULL;
PWLAN_AVAILABLE_NETWORK_LIST pBssList = NULL;
PWLAN_AVAILABLE_NETWORK pBssEntry = NULL;
WLAN_CONNECTION_PARAMETERS scut_student;
int iRSSI = 0;
void FREE() {
	if (pBssList)
		WlanFreeMemory(pBssList);
	if (pIfList)
		WlanFreeMemory(pIfList);
	pBssList = NULL;
	pIfList = NULL;
}
void onError(int ErrCode) {
	char buffer[256]{ 0 };
	sprintf(buffer, "Runtime Fail with %d\n", ErrCode);
	MessageBoxA(nullptr, buffer, "Runtime Fail!", MB_ICONERROR);
	FREE();
	exit(-1);
}
void onError(const int lineNum = 0, int code = 0, const string& errInfo = "") {
	char buffer[256]{ 0 };
	if (!errInfo.length())
		sprintf(buffer, "Error at line %d with code %d.\n", lineNum, code);
	else
		sprintf(buffer, "%s\nCode:%d\nLine:%d\n", errInfo.c_str(), code, lineNum);
	MessageBoxA(nullptr, buffer, "连接时错误!", MB_ICONERROR);
	FREE();
	exit(-1);
}
void _DebugReport(const string & str) {
#ifdef _DEBUG
	cout << "[DEBUG]" << str << endl;
#endif
	return;
}
bool _APIChecker(DWORD Res) {
	return Res == ERROR_SUCCESS || GetLastError() == ERROR_SUCCESS;
}
function<bool(DWORD)> APIChecker = _APIChecker;
template <typename Result, typename ...Args>
Result APICaller(const function<bool(Result)>& checker, Result lastCode, Result(__stdcall *fn)(const Args...), Args... args) {
	Result res = fn(forward<Args>(args)...);
	if (!checker(lastCode) || !checker(res)) onError(GetLastError(), -1);
	return res;
}
void init() {
	dwResult = APICaller(APIChecker, (DWORD)ERROR_SUCCESS, WlanOpenHandle, dwMaxClient, (PVOID)NULL, &dwCurVersion, &hClient);
	dwResult = APICaller(APIChecker, dwResult, WlanEnumInterfaces, hClient, (PVOID)NULL, &pIfList);
	if (pIfList->dwNumberOfItems > 1)
		onError(__LINE__, -2, "不支持好多个无线设备。溜了\n");
	pIfInfo = &pIfList->InterfaceInfo[0];
	/*if (dwResult != ERROR_SUCCESS) {
		onError(__LINE__, dwResult);
	}
	dwResult = WlanEnumInterfaces(hClient, NULL, &pIfList);
	if (dwResult != ERROR_SUCCESS) {
		onError(__LINE__, dwResult);
	}
	if (pIfList->dwNumberOfItems > 1) {
		onError(__LINE__, -2, "不支持好多个无线设备。溜了\n");
	}
	pIfInfo = &pIfList->InterfaceInfo[0];*/
}
int findAP() {
	int result = 0;
	dwResult = APICaller(APIChecker, (DWORD)ERROR_SUCCESS, WlanGetAvailableNetworkList, hClient, const_cast<const GUID*>(&pIfInfo->InterfaceGuid), (DWORD)0, (PVOID)NULL, &pBssList);
	bool isDisconnected = false, found = false;
	for (j = 0; j < pBssList->dwNumberOfItems; ++j) {
		pBssEntry = (WLAN_AVAILABLE_NETWORK*)& pBssList->Network[j];
		if (pBssEntry->dot11Ssid.uSSIDLength == 0)continue;
		string _name_buffer;
		for (k = 0; k < pBssEntry->dot11Ssid.uSSIDLength; ++k)
			_name_buffer += (int)pBssEntry->dot11Ssid.ucSSID[k];
		_DebugReport(_name_buffer);
		if (_name_buffer == APName)
			found = true;
		if (found && !(pBssEntry->bNetworkConnectable)) {
			string tmpstr = "我们找到了热点" + APName + "，但是它目前不可用。";
			onError(__LINE__, GetLastError(), tmpstr);
		}
		else if (found) {
			scut_student.wlanConnectionMode = wlan_connection_mode_profile;
			scut_student.strProfile = pBssEntry->strProfileName;
			scut_student.dwFlags = 0;
			scut_student.pDot11Ssid = nullptr;
			scut_student.pDesiredBssidList = nullptr;
			scut_student.dot11BssType = pBssEntry->dot11BssType;
			return 0;
		}
	}
	if (!found) {
		string tmpstr = "兄啊，我这找不到你要的热点" + APName + "啊";
		onError(__LINE__, -3, tmpstr);
	}
	return -1;
}
/*int getAvailableAP() {
	/*dwResult = WlanGetAvailableNetworkList(hClient,
		&pIfInfo->InterfaceGuid,
		0,
		NULL,
		&pBssList);
	if (dwResult != ERROR_SUCCESS) {
		onError(__LINE__, dwResult);
	}
	dwResult = APICaller(APIChecker, (DWORD)ERROR_SUCCESS, WlanGetAvailableNetworkList, hClient, const_cast<const GUID*>(&pIfInfo->InterfaceGuid), (DWORD)0, (PVOID)NULL, &pBssList);

	int mark = 0;
	bool isDisconnected = false;
	for (j = 0; j < pBssList->dwNumberOfItems; j++) {
		pBssEntry =
			(WLAN_AVAILABLE_NETWORK *)& pBssList->Network[j];
		if (pBssEntry->dot11Ssid.uSSIDLength != 0) {
			string _buffer;
			for (k = 0; k < pBssEntry->dot11Ssid.uSSIDLength; k++) {
				_buffer += (int)pBssEntry->dot11Ssid.ucSSID[k];
			}
			if (_buffer == APName) {
				mark = 1;
				if (pBssEntry->dwFlags & WLAN_AVAILABLE_NETWORK_CONNECTED)
					mark = 2;
				else if (!(pBssEntry->bNetworkConnectable)) {
					onError(__LINE__, -1, "scut-student连接不可用！\n");
					FREE();
					return -1;
				}
				else {
					//获取scut-student的信息；网络不需要密码，所以不用构建XML
					scut_student.wlanConnectionMode = wlan_connection_mode_profile;
					scut_student.strProfile = pBssEntry->strProfileName;
					scut_student.dwFlags = 0;
					scut_student.pDot11Ssid = nullptr;
					scut_student.pDesiredBssidList = nullptr;
					scut_student.dot11BssType = pBssEntry->dot11BssType;

					dwResult =
						WlanConnect(hClient, &pIfInfo->InterfaceGuid, &scut_student, nullptr);
					if (dwResult != ERROR_SUCCESS)
						onError(__LINE__, dwResult);
				}
				break;
			}
			else if (pBssEntry->dwFlags & WLAN_AVAILABLE_NETWORK_CONNECTED) {
				printf("您现在连接的是%s而不是scut-student.帮您重连scut-student\n", _buffer.c_str());
				WlanDisconnect(hClient, &pIfInfo->InterfaceGuid, nullptr);
				isDisconnected = true;
				dwResult = WlanGetAvailableNetworkList(hClient,
					&pIfInfo->InterfaceGuid,
					0,
					NULL,
					&pBssList);
				//return -3;
			}
		}
	}
	return mark;
}*/
void _Connect_WIFI() {
	if (pIfInfo->isState == wlan_interface_state_not_ready)
		onError(__LINE__, -3, "兄啊你这设备没法用啊\n");
	else if (pIfInfo->isState == wlan_interface_state_connected) {
		int res =
			MessageBoxW(nullptr, L"兄啊你都连接到无线网上了，要我帮你切换🐎？", L"不懂就问", MB_YESNO | MB_ICONQUESTION);
		if (res == IDYES) {
			APICaller(APIChecker, (DWORD)ERROR_SUCCESS, WlanDisconnect, hClient, const_cast<const GUID*>(&pIfInfo->InterfaceGuid), (PVOID)NULL);

		}
		else return;
	}
	if (findAP() == 0) {
		dwResult = APICaller(APIChecker, (DWORD)ERROR_SUCCESS, WlanConnect, hClient,
			const_cast<const GUID*>(&pIfInfo->InterfaceGuid), &scut_student, (PVOID)NULL);
	}

}
__declspec(dllexport) void __stdcall Connect(const char* SSID) {
	APName = SSID;
	init();
	_Connect_WIFI();
	FREE();
}
__declspec(dllexport) void __stdcall ConnectToScut_student() {
	Connect("scut-student");
}
