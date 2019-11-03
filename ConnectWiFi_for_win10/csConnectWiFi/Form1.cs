using System;
using System.Threading;
using System.Windows.Forms;

namespace csConnectWiFi {
    public partial class Form1 : Form {
        private ConnectWiFi connectWiFi;
        static int cnt = 3;
        static int FailedTimes = 0;
        static string sWaitinfo = "Attempt to connect in \n{0} seconds.";
        public Form1() {
            InitializeComponent();
        }
        public void SetTimerInterval(int sec) {
            cnt = sec;
        }
        private void Form1_Load(object sender, EventArgs e) {
            //ConnectNow.Visible = false;
            Thread.Sleep(10);
        }
        private void Button1_Click(object sender, EventArgs e) {
            Application.Exit();
        }
        private void Timer1_Tick(object sender, EventArgs e) {
            if (++FailedTimes == 5) {
                FailedTimes = 0;
                tWaitToConnect.Enabled = false;
                ExitTimer.Enabled = true;
                cnt = 3;
                label1.Text = "Something goes wrong with WiFi...";
                NotificationHelper.Popup("Something goes wrong with WiFi :(");
                //ConnectNow.Visible = false;
                //Program.form1.pictureBox1.Image = Properties.Resources.error;
                return;
            }
            if (cnt == 0) {
                ++FailedTimes;
                connectWiFi.Work();
                Thread.Sleep(50);
                connectWiFi.Work();
                cnt = 3;
                tWaitToConnect.Enabled = false;
            } else {
                label1.Text = string.Format(sWaitinfo, cnt--);
            }
        }
        private void Timer1_Tick_1(object sender, EventArgs e) {
            if (cnt == 0) {
                NotificationHelper.Cleanup();
                Application.Exit();
            } else {
                OK.Text = string.Format("OK({0})", cnt--);
            }
        }
        private void IPTryAgain_Tick(object sender, EventArgs e) {
            if (FailedTimes == 5) {
                FailedTimes = 0;
                tWaitToConnect.Enabled = false;
                IPTryAgain.Enabled = false;
                ExitTimer.Enabled = true;
                //ConnectNow.Visible = false;
                cnt = 3;
                //label1.Text = "Something goes wrong with WiFi...";
                NotificationHelper.Popup("Something goes wrong with WiFi :(");
                //Program.form1.pictureBox1.Image = Properties.Resources.error;
                
                return;
            }
            if (cnt == 0) {
                ++FailedTimes;
                IPTryAgain.Enabled = false;
                cnt = 3;
                ConnectWiFi.ConnectToScut_student();
                Thread.Sleep(100);
                connectWiFi.GetIP();
                connectWiFi.Work();
            } else {
                label1.Text = string.Format(sWaitinfo, cnt--);
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            Application.Exit();
        }
        private void Button1_KeyPress(object sender, KeyPressEventArgs e) {
            Application.Exit();
        }
        private void Button1_KeyDown(object sender, KeyEventArgs e) {
            Application.Exit();
        }
        private void Form1_Shown(object sender, EventArgs e) {
            try {
                this.Visible = false;
                connectWiFi = new ConnectWiFi();
                int x = ConnectWiFi.CreateMutexA(IntPtr.Zero, true, "ConnectWiFiSCUT");
                if (x == 0) Application.Exit();
                connectWiFi.Getuser();
                if (!ConnectWiFi.IsInTargetArea()) {
                    NotificationHelper.Popup("Ooops, it seems that you are not in the dormitory.",2);
                    label1.Text = "You are not in the dorm.";
                    pictureBox1.Image = Properties.Resources.error;
                    ExitTimer.Interval = 1000;
                    ExitTimer.Enabled = true;
                    return;
                }
                ConnectWiFi.ConnectToScut_student();
                Thread.Sleep(100);
                connectWiFi.GetIP();
                if (connectWiFi.status && this.label1.Text == "You are online.") {
                    //pictureBox1.Image = Properties.Resources.success;
                    //OK.Focus();
                    SetTimerInterval(2);
                    ExitTimer.Interval = 1000;
                    ExitTimer.Enabled = true;
                    return;
                } else if (connectWiFi.status) {
                    tWaitToConnect.Interval = 1000;
                    SetTimerInterval(2);
                    tWaitToConnect.Enabled = true;
                    //ConnectNow.Visible = true;
                }

                //MessageBox.Show("233333");
                return;
            } catch (DllNotFoundException) {
                //label1.Text = "Missing ConnectWiFi_x64.dll";
                NotificationHelper.Popup("Make sure you have ConnectWiFi_x64.dll!",2);
                //Program.form1.pictureBox1.Image = Properties.Resources.error;
                SetTimerInterval(2);
                ExitTimer.Interval = 1000;
                ExitTimer.Enabled = true;
                return;
            }
        }
    }
}
