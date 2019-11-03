using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Windows.UI;
using Windows.Data;
//using System.IO;
using note = Windows.UI.Notifications;
using manager = Windows.UI.Notifications.ToastNotificationManager;

namespace csConnectWiFi {
    static class ConsoleHelper {
        static public void Initialize(bool alwaysCreateNewConsole = true) {
            bool consoleAttached = true;
            if (alwaysCreateNewConsole
                || (AttachConsole(ATTACH_PARRENT) == 0
                && Marshal.GetLastWin32Error() != ERROR_ACCESS_DENIED)) {
                consoleAttached = AllocConsole() != 0;
            }
            if (consoleAttached) {
                InitializeOutStream();
                InitializeInStream();
            }
        }
        static public bool Free() {
            return FreeConsole();
        }
        private static void InitializeOutStream() {
            var fs = CreateFileStream("CONOUT$", GENERIC_WRITE, FILE_SHARE_WRITE, FileAccess.Write);
            if (fs != null) {
                var writer = new StreamWriter(fs) { AutoFlush = true };
                Console.SetOut(writer);
                Console.SetError(writer);
            }
        }

        private static void InitializeInStream() {
            var fs = CreateFileStream("CONIN$", GENERIC_READ, FILE_SHARE_READ, FileAccess.Read);
            if (fs != null) {
                Console.SetIn(new StreamReader(fs));
            }
        }
        private static FileStream CreateFileStream(string name, uint win32DesiredAccess, uint win32ShareMode,
                                FileAccess dotNetFileAccess) {
            var file = new SafeFileHandle(CreateFileW(name, win32DesiredAccess, win32ShareMode, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero), true);
            if (!file.IsInvalid) {
                var fs = new FileStream(file, dotNetFileAccess);
                return fs;
            }
            return null;
        }

        [DllImport("kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();
        [DllImport("kernel32.dll",
            EntryPoint = "FreeConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern bool FreeConsole();
        [DllImport("kernel32.dll",
            EntryPoint = "AttachConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern UInt32 AttachConsole(UInt32 dwProcessId);

        [DllImport("kernel32.dll",
            EntryPoint = "CreateFileW",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr CreateFileW(
              string lpFileName,
              UInt32 dwDesiredAccess,
              UInt32 dwShareMode,
              IntPtr lpSecurityAttributes,
              UInt32 dwCreationDisposition,
              UInt32 dwFlagsAndAttributes,
              IntPtr hTemplateFile
            );
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        private const UInt32 WM_CLOSE = 0x0010;
        private const UInt32 GENERIC_WRITE = 0x40000000;
        private const UInt32 GENERIC_READ = 0x80000000;
        private const UInt32 FILE_SHARE_READ = 0x00000001;
        private const UInt32 FILE_SHARE_WRITE = 0x00000002;
        private const UInt32 OPEN_EXISTING = 0x00000003;
        private const UInt32 FILE_ATTRIBUTE_NORMAL = 0x80;
        private const UInt32 ERROR_ACCESS_DENIED = 5;

        private const UInt32 ATTACH_PARRENT = 0xFFFFFFFF;
    }
    static public class NotificationHelper {
        static note.ToastNotification toast = null;
        static readonly string sub_title = "SCUT-WiFi-Helper";
        static public void Popup(string s,int imgID = 0) {
            var toastXML = manager.GetTemplateContent(note.ToastTemplateType.ToastImageAndText01);
            var imgtag = toastXML.GetElementsByTagName("image");
            string ss = "./img/";
            if(imgID == 0) {
                ss += "logo.ico";
            }else if(imgID == 1) {
                ss += "ok.png";
            } else {
                ss += "error.png";
            }
            imgtag[0].Attributes.GetNamedItem("src").NodeValue = "file:///" + Path.GetFullPath(ss);
            var texttag = toastXML.GetElementsByTagName("text");
            texttag[0].AppendChild(toastXML.CreateTextNode(s));
            toast = new note.ToastNotification(toastXML);
            manager.CreateToastNotifier(sub_title).Show(toast);
        }
        static public void Cleanup() {
            manager.CreateToastNotifier(sub_title).Hide(toast);
        }
    }
    class ConnectWiFi {
        public readonly HttpClient hc = new HttpClient();
        public readonly string URL_getIP = "http://s.scut.edu.cn";
        public readonly string IP_Pattern =
            @"^(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)$";
        public readonly string user_Pattern = @"^20\d{10}$";
        public readonly string ACName = "WX6108E-slot7-AC",
                                        wlanid = "scut-student",
                                        wlanacip = "172.21.255.250";
        public string ConnectURL;

        public RegistryKey key, my;
        public bool status = false;
        private string IP, username, password;
        //private bool bAutoConnect = false;
        public void SaveAccInfo() {
            ConsoleHelper.Initialize();
            System.Console.WriteLine("Please input your ID:");
            string tmp = System.Console.ReadLine();
            while (!Regex.IsMatch(tmp, user_Pattern)) {
                System.Console.WriteLine("Please input your ID(20xxxxxxxxxx):");
                tmp = System.Console.ReadLine();
            }
            username = tmp;
            System.Console.WriteLine("Please input your password:");
            tmp = System.Console.ReadLine();
            password = tmp;
            if (key != null) {
                if (my == null)
                    my = key.CreateSubKey("Software\\LogDormWifi", true);
                my.SetValue("user_id", username);
                my.SetValue("password", password);
            }
        }
        private void TryAgain(int type) {
            switch (type) {
                case 1: //获取IP超时
                    NotificationHelper.Popup("TLE getting IP.Try again in" + 3.ToString() + "seconds.");
                    //Program.form1.label1.Text = "TLE getting IP.Try Again";
                    Thread.Sleep(500);
                    Program.form1.SetTimerInterval(3);
                    //Program.form1.ConnectNow.Visible = false;
                    Program.form1.IPTryAgain.Enabled = true;
                    break;
                case 2: //连接超时
                    NotificationHelper.Popup("TLE when connect.Try again in" + 3.ToString() + "seconds.");
                    //Program.form1.label1.Text = "TLE when connect.Try Again";
                    Thread.Sleep(500);
                    //Program.form1.ConnectNow.Visible = true;
                    Program.form1.SetTimerInterval(3);
                    Program.form1.tWaitToConnect.Enabled = true;
                    break;
                default:
                    break;
            }
        }
        public void GetIP() {
            try {
                //throw new HttpRequestException("23333");
                string res = hc.GetStringAsync(URL_getIP).Result;
                int pos = res.IndexOf("ss5=\""),
                        end = res.IndexOf("\"", pos + 5);
                if (pos == -1) {
                    pos = res.IndexOf("CTYPE html PUBLIC");
                    if (pos != -1) {
                        status = true;
                        ConsoleHelper.Free();
                        NotificationHelper.Popup("You are now online.",1);
                        Program.form1.label1.Text = "You are online.";
                        //Program.form1.OK.Text = "OK";
                        //Program.form1.ConnectNow.Visible = false;
                        //Program.form1.pictureBox1.Image = Properties.Resources.success;
                        return;
                    }
                    ConsoleHelper.Free();
                    Program.form1.label1.Text = "Failed to get IP.";
                    NotificationHelper.Popup("Failed to get IP.",2);

                    //Program.form1.ConnectNow.Visible = false;

                    throw new HttpRequestException("2333");
                }
                string tmp = res.Substring(pos + 5, end - pos - 5);
                status = Regex.IsMatch(tmp, IP_Pattern);
                if (status)
                    IP = tmp;
                Program.form1.label1.Text = "Your IP:" + IP;
                status = true;
                //Program.form1.ConnectNow.Visible = true;
                return;
            } catch (HttpRequestException) {
                TryAgain(1);
            } catch (WebException) {
                TryAgain(1);
            } catch (AggregateException) {
                TryAgain(1);
            }
        }
        public void Work() {
            try {
                //System.Console.WriteLine(tmp);
                ConnectURL = "https://s.scut.edu.cn:801/eportal/?c=ACSetting&a=Login&wlanuserip=" + IP + "&wlanacip=" + wlanacip + "&wlanacname=" + ACName + "&redirect=&session=&vlanid=" + wlanid + "&port=&iTermType=1&protocol=https:";
                var post = new Dictionary<string, string> {
                    {"DDDDD",username },
                    {"upass",password },
                    {"R1","0" },
                    {"R2","" },
                    {"R6", "0"},
                    { "para", "00" },
                    { "0MKKey","123456" }
                };
                FormUrlEncodedContent content = new FormUrlEncodedContent(post);
                var Res = hc.PostAsync(ConnectURL, content).Result;
                var Tmp = Res.Content;
                string ans = Tmp.ReadAsStringAsync().Result;
                if (ans.IndexOf("成功") != -1) {
                    NotificationHelper.Popup("You are now online.", 1);

                    Program.form1.label1.Text = "You are online.";
                    //Program.form1.OK.Text = "OK";
                    Program.form1.SetTimerInterval(2);
                    //Program.form1.ConnectNow.Visible = false;
                    Program.form1.ExitTimer.Enabled = true;
                    //Program.form1.pictureBox1.Image = Properties.Resources.success;
                } else if (ans.IndexOf("已使用") != -1) {
                    NotificationHelper.Popup("You are now online.", 1);

                    Program.form1.label1.Text = "You are online.";
                    //Program.form1.OK.Text = "OK";
                    Program.form1.SetTimerInterval(2);
                    Program.form1.ExitTimer.Enabled = true;
                    //Program.form1.ConnectNow.Visible = false;

                    Program.form1.pictureBox1.Image = Properties.Resources.success;
                } else {
                    NotificationHelper.Popup("Failed to login!", 2);

                    //Program.form1.label1.Text = "Failed to login!";
                    //Program.form1.ConnectNow.Visible = false;
                }
                ConsoleHelper.Free();

            } catch (HttpRequestException) {
                TryAgain(1);
            } catch (WebException) {
                TryAgain(2);
            } catch (AggregateException) {
                TryAgain(2);
            }
        }
        public void Getuser() {
            key = Registry.CurrentUser;
            my = key.OpenSubKey("Software\\LogDormWifi", true);
            if (null == my)
                SaveAccInfo();
            username = my.GetValue("user_id").ToString();
            password = my.GetValue("password").ToString();
            if (username == null || password == null
                || !Regex.IsMatch(username, user_Pattern)) {
                SaveAccInfo();
            }
        }
        [DllImport("Kernel32.dll",
            EntryPoint = "CreateMutexA",
            CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int CreateMutexA(IntPtr x, bool bInitialOwner, string lpName);
        [DllImport("ConnectWiFi_x64.dll",
            EntryPoint = "isInTargetArea",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool IsInTargetArea();
        [DllImport("ConnectWiFi_x64.dll",
            EntryPoint = "ConnectToScut_student",
            CallingConvention = CallingConvention.StdCall)]
        public static extern void ConnectToScut_student();
    }
    static class Program {
        static public Form1 form1;
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form1 = new Form1();
            Application.Run(form1);
        }
    }
}
