namespace csConnectWiFi {
    partial class Form1 {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.OK = new System.Windows.Forms.Button();
            this.tWaitToConnect = new System.Windows.Forms.Timer(this.components);
            this.ExitTimer = new System.Windows.Forms.Timer(this.components);
            this.IPTryAgain = new System.Windows.Forms.Timer(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Desktop;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(215, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(327, 48);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please Stand by...";
            this.label1.UseMnemonic = false;
            //this.label1.Click += new System.EventHandler(this.Label1_Click);
            // 
            // OK
            // 
            this.OK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.OK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OK.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.OK.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.OK.Location = new System.Drawing.Point(693, 117);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(178, 58);
            this.OK.TabIndex = 2;
            this.OK.Text = "Cancel";
            this.OK.UseVisualStyleBackColor = false;
            this.OK.Click += new System.EventHandler(this.Button1_Click);
            this.OK.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.OK.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Button1_KeyPress);
            // 
            // tWaitToConnect
            // 
            this.tWaitToConnect.Interval = 1000;
            this.tWaitToConnect.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // ExitTimer
            // 
            this.ExitTimer.Interval = 1000;
            this.ExitTimer.Tick += new System.EventHandler(this.Timer1_Tick_1);
            // 
            // IPTryAgain
            // 
            this.IPTryAgain.Interval = 1000;
            this.IPTryAgain.Tick += new System.EventHandler(this.IPTryAgain_Tick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Desktop;
            this.pictureBox1.Image = global::csConnectWiFi.Properties.Resources.connect;
            this.pictureBox1.Location = new System.Drawing.Point(61, 35);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            //this.pictureBox1.Click += new System.EventHandler(this.PictureBox1_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.Desktop;
            this.label2.Location = new System.Drawing.Point(2, 2);
            this.label2.Margin = new System.Windows.Forms.Padding(1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(897, 197);
            this.label2.TabIndex = 3;
            this.label2.Text = "label2";
            //this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(901, 201);
            this.ControlBox = false;
            this.Controls.Add(this.OK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connect";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button OK;
        public System.Windows.Forms.Timer tWaitToConnect;
        public System.Windows.Forms.Timer ExitTimer;
        public System.Windows.Forms.Timer IPTryAgain;
        private System.Windows.Forms.Label label2;
    }
}

