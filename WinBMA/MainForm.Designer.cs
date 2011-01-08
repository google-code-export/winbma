namespace WinBMA
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ssStatus = new System.Windows.Forms.StatusStrip();
            this.lblSerial = new System.Windows.Forms.ToolStripStatusLabel();
            this.barTime = new System.Windows.Forms.ToolStripProgressBar();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.mnuAuths = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuResync = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.cmbAuths = new System.Windows.Forms.ToolStripComboBox();
            this.timGenerate = new System.Windows.Forms.Timer(this.components);
            this.lblAuthKey = new WinBMA.AntiAliasLabel();
            this.ssStatus.SuspendLayout();
            this.mnuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // ssStatus
            // 
            this.ssStatus.AllowMerge = false;
            this.ssStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblSerial,
            this.barTime});
            this.ssStatus.Location = new System.Drawing.Point(0, 122);
            this.ssStatus.Name = "ssStatus";
            this.ssStatus.ShowItemToolTips = true;
            this.ssStatus.Size = new System.Drawing.Size(329, 22);
            this.ssStatus.SizingGrip = false;
            this.ssStatus.TabIndex = 2;
            this.ssStatus.Text = "WinBMA";
            // 
            // lblSerial
            // 
            this.lblSerial.Name = "lblSerial";
            this.lblSerial.Size = new System.Drawing.Size(181, 17);
            this.lblSerial.Spring = true;
            this.lblSerial.Text = " ";
            this.lblSerial.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSerial.Click += new System.EventHandler(this.lblSerial_Click);
            // 
            // barTime
            // 
            this.barTime.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.barTime.Maximum = 30000;
            this.barTime.Name = "barTime";
            this.barTime.Size = new System.Drawing.Size(100, 16);
            this.barTime.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAuths,
            this.cmbAuths});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.mnuMain.Size = new System.Drawing.Size(329, 35);
            this.mnuMain.TabIndex = 0;
            this.mnuMain.Text = "WinBMA";
            // 
            // mnuAuths
            // 
            this.mnuAuths.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNew,
            this.mnuView,
            this.toolStripMenuItem1,
            this.mnuResync,
            this.toolStripSeparator1,
            this.mnuExit});
            this.mnuAuths.Image = ((System.Drawing.Image)(resources.GetObject("mnuAuths.Image")));
            this.mnuAuths.Name = "mnuAuths";
            this.mnuAuths.Size = new System.Drawing.Size(66, 31);
            this.mnuAuths.Text = "&Auths";
            // 
            // mnuNew
            // 
            this.mnuNew.Image = ((System.Drawing.Image)(resources.GetObject("mnuNew.Image")));
            this.mnuNew.Name = "mnuNew";
            this.mnuNew.Size = new System.Drawing.Size(144, 22);
            this.mnuNew.Text = "Create &New...";
            this.mnuNew.Click += new System.EventHandler(this.mnuNew_Click);
            // 
            // mnuView
            // 
            this.mnuView.Image = ((System.Drawing.Image)(resources.GetObject("mnuView.Image")));
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(144, 22);
            this.mnuView.Text = "&Manage...";
            this.mnuView.Click += new System.EventHandler(this.mnuView_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(141, 6);
            // 
            // mnuResync
            // 
            this.mnuResync.Image = ((System.Drawing.Image)(resources.GetObject("mnuResync.Image")));
            this.mnuResync.Name = "mnuResync";
            this.mnuResync.Size = new System.Drawing.Size(144, 22);
            this.mnuResync.Text = "Resync &Time";
            this.mnuResync.Click += new System.EventHandler(this.mnuResync_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(141, 6);
            // 
            // mnuExit
            // 
            this.mnuExit.Image = ((System.Drawing.Image)(resources.GetObject("mnuExit.Image")));
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.mnuExit.Size = new System.Drawing.Size(144, 22);
            this.mnuExit.Text = "E&xit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // cmbAuths
            // 
            this.cmbAuths.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.cmbAuths.AutoSize = false;
            this.cmbAuths.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAuths.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbAuths.Margin = new System.Windows.Forms.Padding(1, 4, 1, 4);
            this.cmbAuths.Name = "cmbAuths";
            this.cmbAuths.Size = new System.Drawing.Size(245, 23);
            this.cmbAuths.SelectedIndexChanged += new System.EventHandler(this.cmbAuths_SelectedIndexChanged);
            // 
            // timGenerate
            // 
            this.timGenerate.Enabled = true;
            this.timGenerate.Interval = 200;
            this.timGenerate.Tick += new System.EventHandler(this.timGenerate_Tick);
            // 
            // lblAuthKey
            // 
            this.lblAuthKey.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblAuthKey.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAuthKey.Font = new System.Drawing.Font("Tahoma", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAuthKey.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblAuthKey.Location = new System.Drawing.Point(0, 35);
            this.lblAuthKey.Name = "lblAuthKey";
            this.lblAuthKey.Size = new System.Drawing.Size(329, 87);
            this.lblAuthKey.TabIndex = 1;
            this.lblAuthKey.TabStop = true;
            this.lblAuthKey.Text = "00000000";
            this.lblAuthKey.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAuthKey.Click += new System.EventHandler(this.lblAuthKey_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 144);
            this.Controls.Add(this.lblAuthKey);
            this.Controls.Add(this.ssStatus);
            this.Controls.Add(this.mnuMain);
            this.Font = System.Drawing.SystemFonts.DefaultFont;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mnuMain;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WinBMA {0}";
            this.ssStatus.ResumeLayout(false);
            this.ssStatus.PerformLayout();
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip ssStatus;
        private System.Windows.Forms.ToolStripProgressBar barTime;
        private System.Windows.Forms.ToolStripStatusLabel lblSerial;
        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem mnuAuths;
        private System.Windows.Forms.ToolStripMenuItem mnuNew;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.ToolStripComboBox cmbAuths;
        private System.Windows.Forms.ToolStripMenuItem mnuResync;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Timer timGenerate;
        private AntiAliasLabel lblAuthKey;

    }
}

