namespace WinBMA
{
    partial class ManageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageForm));
            this.tlpNew = new System.Windows.Forms.TableLayoutPanel();
            this.ssStatus = new System.Windows.Forms.StatusStrip();
            this.lblCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.barTime = new System.Windows.Forms.ToolStripProgressBar();
            this.splSplit = new System.Windows.Forms.SplitContainer();
            this.tlpLeftPane = new System.Windows.Forms.TableLayoutPanel();
            this.tsTools = new System.Windows.Forms.ToolStrip();
            this.mnuNew = new System.Windows.Forms.ToolStripButton();
            this.mnuDelete = new System.Windows.Forms.ToolStripButton();
            this.mnuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.mnuImport = new System.Windows.Forms.ToolStripButton();
            this.mnuExport = new System.Windows.Forms.ToolStripButton();
            this.lstAuths = new System.Windows.Forms.ListBox();
            this.tlpRight = new System.Windows.Forms.TableLayoutPanel();
            this.grpNew = new System.Windows.Forms.GroupBox();
            this.tlpChooseRegion = new System.Windows.Forms.TableLayoutPanel();
            this.tlpRegionButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnRegionEU = new System.Windows.Forms.Button();
            this.btnRegionUS = new System.Windows.Forms.Button();
            this.lblNewInfo = new System.Windows.Forms.Label();
            this.tlpInfo = new System.Windows.Forms.TableLayoutPanel();
            this.lblRegion = new System.Windows.Forms.Label();
            this.lblToken = new System.Windows.Forms.Label();
            this.lblSerial = new System.Windows.Forms.Label();
            this.txtSerial = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblAuthCode = new System.Windows.Forms.Label();
            this.txtRegion = new System.Windows.Forms.TextBox();
            this.txtToken = new System.Windows.Forms.TextBox();
            this.lblAuthKey = new WinBMA.AntiAliasLabel();
            this.timGenerate = new System.Windows.Forms.Timer(this.components);
            this.tlpNew.SuspendLayout();
            this.ssStatus.SuspendLayout();
            this.splSplit.Panel1.SuspendLayout();
            this.splSplit.Panel2.SuspendLayout();
            this.splSplit.SuspendLayout();
            this.tlpLeftPane.SuspendLayout();
            this.tsTools.SuspendLayout();
            this.tlpRight.SuspendLayout();
            this.grpNew.SuspendLayout();
            this.tlpChooseRegion.SuspendLayout();
            this.tlpRegionButtons.SuspendLayout();
            this.tlpInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpNew
            // 
            this.tlpNew.ColumnCount = 1;
            this.tlpNew.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpNew.Controls.Add(this.ssStatus, 0, 1);
            this.tlpNew.Controls.Add(this.splSplit, 0, 0);
            this.tlpNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpNew.Location = new System.Drawing.Point(0, 0);
            this.tlpNew.Name = "tlpNew";
            this.tlpNew.RowCount = 2;
            this.tlpNew.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpNew.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpNew.Size = new System.Drawing.Size(654, 372);
            this.tlpNew.TabIndex = 0;
            // 
            // ssStatus
            // 
            this.ssStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblCount,
            this.barTime});
            this.ssStatus.Location = new System.Drawing.Point(0, 350);
            this.ssStatus.Name = "ssStatus";
            this.ssStatus.ShowItemToolTips = true;
            this.ssStatus.Size = new System.Drawing.Size(654, 22);
            this.ssStatus.SizingGrip = false;
            this.ssStatus.TabIndex = 1;
            this.ssStatus.Text = "statusStrip1";
            // 
            // lblCount
            // 
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(537, 17);
            this.lblCount.Spring = true;
            this.lblCount.Text = "0 Authenticators";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // barTime
            // 
            this.barTime.Maximum = 30000;
            this.barTime.Name = "barTime";
            this.barTime.Size = new System.Drawing.Size(100, 16);
            this.barTime.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // splSplit
            // 
            this.splSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splSplit.IsSplitterFixed = true;
            this.splSplit.Location = new System.Drawing.Point(3, 3);
            this.splSplit.Name = "splSplit";
            // 
            // splSplit.Panel1
            // 
            this.splSplit.Panel1.Controls.Add(this.tlpLeftPane);
            // 
            // splSplit.Panel2
            // 
            this.splSplit.Panel2.Controls.Add(this.tlpRight);
            this.splSplit.Size = new System.Drawing.Size(648, 344);
            this.splSplit.SplitterDistance = 250;
            this.splSplit.TabIndex = 1;
            // 
            // tlpLeftPane
            // 
            this.tlpLeftPane.BackColor = System.Drawing.SystemColors.Window;
            this.tlpLeftPane.ColumnCount = 1;
            this.tlpLeftPane.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLeftPane.Controls.Add(this.tsTools, 0, 0);
            this.tlpLeftPane.Controls.Add(this.lstAuths, 0, 1);
            this.tlpLeftPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpLeftPane.Location = new System.Drawing.Point(0, 0);
            this.tlpLeftPane.Name = "tlpLeftPane";
            this.tlpLeftPane.RowCount = 2;
            this.tlpLeftPane.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpLeftPane.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLeftPane.Size = new System.Drawing.Size(250, 344);
            this.tlpLeftPane.TabIndex = 0;
            // 
            // tsTools
            // 
            this.tsTools.AllowMerge = false;
            this.tsTools.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNew,
            this.mnuDelete,
            this.mnuSeparator,
            this.mnuImport,
            this.mnuExport});
            this.tsTools.Location = new System.Drawing.Point(0, 0);
            this.tsTools.Name = "tsTools";
            this.tsTools.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.tsTools.Size = new System.Drawing.Size(250, 25);
            this.tsTools.Stretch = true;
            this.tsTools.TabIndex = 0;
            this.tsTools.Text = "Manage";
            // 
            // mnuNew
            // 
            this.mnuNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuNew.Image = ((System.Drawing.Image)(resources.GetObject("mnuNew.Image")));
            this.mnuNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuNew.Name = "mnuNew";
            this.mnuNew.Size = new System.Drawing.Size(23, 22);
            this.mnuNew.Text = "New";
            this.mnuNew.Click += new System.EventHandler(this.mnuNew_Click);
            // 
            // mnuDelete
            // 
            this.mnuDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuDelete.Image = ((System.Drawing.Image)(resources.GetObject("mnuDelete.Image")));
            this.mnuDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuDelete.Name = "mnuDelete";
            this.mnuDelete.Size = new System.Drawing.Size(23, 22);
            this.mnuDelete.Text = "Delete";
            this.mnuDelete.Click += new System.EventHandler(this.mnuDelete_Click);
            // 
            // mnuSeparator
            // 
            this.mnuSeparator.Name = "mnuSeparator";
            this.mnuSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // mnuImport
            // 
            this.mnuImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuImport.Image = ((System.Drawing.Image)(resources.GetObject("mnuImport.Image")));
            this.mnuImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuImport.Name = "mnuImport";
            this.mnuImport.Size = new System.Drawing.Size(23, 22);
            this.mnuImport.Text = "Import";
            this.mnuImport.Click += new System.EventHandler(this.mnuImport_Click);
            // 
            // mnuExport
            // 
            this.mnuExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuExport.Image = ((System.Drawing.Image)(resources.GetObject("mnuExport.Image")));
            this.mnuExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuExport.Name = "mnuExport";
            this.mnuExport.Size = new System.Drawing.Size(23, 22);
            this.mnuExport.Text = "Export";
            this.mnuExport.Click += new System.EventHandler(this.mnuExport_Click);
            // 
            // lstAuths
            // 
            this.lstAuths.AllowDrop = true;
            this.lstAuths.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstAuths.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAuths.FormattingEnabled = true;
            this.lstAuths.Location = new System.Drawing.Point(3, 28);
            this.lstAuths.Name = "lstAuths";
            this.lstAuths.Size = new System.Drawing.Size(244, 313);
            this.lstAuths.TabIndex = 1;
            this.lstAuths.SelectedIndexChanged += new System.EventHandler(this.lstAuths_SelectedIndexChanged);
            this.lstAuths.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstAuths_DragDrop);
            this.lstAuths.DragOver += new System.Windows.Forms.DragEventHandler(this.lstAuths_DragOver);
            this.lstAuths.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstAuths_MouseDown);
            // 
            // tlpRight
            // 
            this.tlpRight.ColumnCount = 1;
            this.tlpRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRight.Controls.Add(this.grpNew, 0, 0);
            this.tlpRight.Controls.Add(this.tlpInfo, 0, 1);
            this.tlpRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRight.Location = new System.Drawing.Point(0, 0);
            this.tlpRight.Name = "tlpRight";
            this.tlpRight.RowCount = 2;
            this.tlpRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRight.Size = new System.Drawing.Size(394, 344);
            this.tlpRight.TabIndex = 0;
            // 
            // grpNew
            // 
            this.grpNew.AutoSize = true;
            this.grpNew.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpNew.Controls.Add(this.tlpChooseRegion);
            this.grpNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpNew.Location = new System.Drawing.Point(3, 3);
            this.grpNew.Margin = new System.Windows.Forms.Padding(3, 3, 3, 13);
            this.grpNew.Name = "grpNew";
            this.grpNew.Padding = new System.Windows.Forms.Padding(13);
            this.grpNew.Size = new System.Drawing.Size(388, 106);
            this.grpNew.TabIndex = 1;
            this.grpNew.TabStop = false;
            this.grpNew.Text = "New Authenticator";
            this.grpNew.Visible = false;
            // 
            // tlpChooseRegion
            // 
            this.tlpChooseRegion.AutoSize = true;
            this.tlpChooseRegion.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpChooseRegion.ColumnCount = 1;
            this.tlpChooseRegion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpChooseRegion.Controls.Add(this.tlpRegionButtons, 0, 1);
            this.tlpChooseRegion.Controls.Add(this.lblNewInfo, 0, 0);
            this.tlpChooseRegion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpChooseRegion.Location = new System.Drawing.Point(13, 28);
            this.tlpChooseRegion.Name = "tlpChooseRegion";
            this.tlpChooseRegion.RowCount = 2;
            this.tlpChooseRegion.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpChooseRegion.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpChooseRegion.Size = new System.Drawing.Size(362, 65);
            this.tlpChooseRegion.TabIndex = 0;
            // 
            // tlpRegionButtons
            // 
            this.tlpRegionButtons.AutoSize = true;
            this.tlpRegionButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpRegionButtons.ColumnCount = 2;
            this.tlpRegionButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpRegionButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpRegionButtons.Controls.Add(this.btnRegionEU, 1, 0);
            this.tlpRegionButtons.Controls.Add(this.btnRegionUS, 0, 0);
            this.tlpRegionButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRegionButtons.Location = new System.Drawing.Point(3, 22);
            this.tlpRegionButtons.Name = "tlpRegionButtons";
            this.tlpRegionButtons.RowCount = 1;
            this.tlpRegionButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpRegionButtons.Size = new System.Drawing.Size(356, 40);
            this.tlpRegionButtons.TabIndex = 1;
            // 
            // btnRegionEU
            // 
            this.btnRegionEU.AutoSize = true;
            this.btnRegionEU.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRegionEU.Location = new System.Drawing.Point(181, 3);
            this.btnRegionEU.Name = "btnRegionEU";
            this.btnRegionEU.Size = new System.Drawing.Size(172, 34);
            this.btnRegionEU.TabIndex = 1;
            this.btnRegionEU.Text = "Europe";
            this.btnRegionEU.UseVisualStyleBackColor = true;
            this.btnRegionEU.Click += new System.EventHandler(this.btnRegionEU_Click);
            // 
            // btnRegionUS
            // 
            this.btnRegionUS.AutoSize = true;
            this.btnRegionUS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRegionUS.Location = new System.Drawing.Point(3, 3);
            this.btnRegionUS.Name = "btnRegionUS";
            this.btnRegionUS.Size = new System.Drawing.Size(172, 34);
            this.btnRegionUS.TabIndex = 0;
            this.btnRegionUS.Text = "North America";
            this.btnRegionUS.UseVisualStyleBackColor = true;
            this.btnRegionUS.Click += new System.EventHandler(this.btnRegionUS_Click);
            // 
            // lblNewInfo
            // 
            this.lblNewInfo.AutoSize = true;
            this.lblNewInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNewInfo.Location = new System.Drawing.Point(3, 0);
            this.lblNewInfo.Margin = new System.Windows.Forms.Padding(3, 0, 3, 6);
            this.lblNewInfo.Name = "lblNewInfo";
            this.lblNewInfo.Size = new System.Drawing.Size(356, 13);
            this.lblNewInfo.TabIndex = 0;
            this.lblNewInfo.Text = "Please choose the region of your choice for your authenticator.";
            // 
            // tlpInfo
            // 
            this.tlpInfo.ColumnCount = 2;
            this.tlpInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tlpInfo.Controls.Add(this.lblRegion, 0, 4);
            this.tlpInfo.Controls.Add(this.lblToken, 0, 3);
            this.tlpInfo.Controls.Add(this.lblSerial, 0, 2);
            this.tlpInfo.Controls.Add(this.txtSerial, 1, 2);
            this.tlpInfo.Controls.Add(this.lblName, 0, 0);
            this.tlpInfo.Controls.Add(this.txtName, 1, 0);
            this.tlpInfo.Controls.Add(this.lblAuthCode, 0, 6);
            this.tlpInfo.Controls.Add(this.txtRegion, 1, 4);
            this.tlpInfo.Controls.Add(this.txtToken, 1, 3);
            this.tlpInfo.Controls.Add(this.lblAuthKey, 1, 6);
            this.tlpInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpInfo.Location = new System.Drawing.Point(3, 125);
            this.tlpInfo.Name = "tlpInfo";
            this.tlpInfo.RowCount = 8;
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpInfo.Size = new System.Drawing.Size(388, 216);
            this.tlpInfo.TabIndex = 0;
            // 
            // lblRegion
            // 
            this.lblRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRegion.AutoSize = true;
            this.lblRegion.Location = new System.Drawing.Point(3, 101);
            this.lblRegion.Name = "lblRegion";
            this.lblRegion.Size = new System.Drawing.Size(91, 13);
            this.lblRegion.TabIndex = 6;
            this.lblRegion.Text = "Region:";
            this.lblRegion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblToken
            // 
            this.lblToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblToken.AutoSize = true;
            this.lblToken.Location = new System.Drawing.Point(3, 73);
            this.lblToken.Name = "lblToken";
            this.lblToken.Size = new System.Drawing.Size(91, 13);
            this.lblToken.TabIndex = 4;
            this.lblToken.Text = "Token:";
            this.lblToken.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSerial
            // 
            this.lblSerial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSerial.AutoSize = true;
            this.lblSerial.Location = new System.Drawing.Point(3, 45);
            this.lblSerial.Name = "lblSerial";
            this.lblSerial.Size = new System.Drawing.Size(91, 13);
            this.lblSerial.TabIndex = 2;
            this.lblSerial.Text = "Serial:";
            this.lblSerial.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSerial
            // 
            this.txtSerial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSerial.Location = new System.Drawing.Point(100, 41);
            this.txtSerial.Name = "txtSerial";
            this.txtSerial.ReadOnly = true;
            this.txtSerial.Size = new System.Drawing.Size(285, 22);
            this.txtSerial.TabIndex = 3;
            // 
            // lblName
            // 
            this.lblName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(3, 7);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(91, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name:";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(100, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(285, 22);
            this.txtName.TabIndex = 1;
            this.txtName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtName_KeyUp);
            this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
            // 
            // lblAuthCode
            // 
            this.lblAuthCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAuthCode.AutoSize = true;
            this.lblAuthCode.Location = new System.Drawing.Point(3, 154);
            this.lblAuthCode.Name = "lblAuthCode";
            this.lblAuthCode.Size = new System.Drawing.Size(91, 13);
            this.lblAuthCode.TabIndex = 8;
            this.lblAuthCode.Text = "Auth Key:";
            this.lblAuthCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtRegion
            // 
            this.txtRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRegion.Location = new System.Drawing.Point(100, 97);
            this.txtRegion.Name = "txtRegion";
            this.txtRegion.ReadOnly = true;
            this.txtRegion.Size = new System.Drawing.Size(285, 22);
            this.txtRegion.TabIndex = 7;
            // 
            // txtToken
            // 
            this.txtToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtToken.Location = new System.Drawing.Point(100, 69);
            this.txtToken.Name = "txtToken";
            this.txtToken.ReadOnly = true;
            this.txtToken.Size = new System.Drawing.Size(285, 22);
            this.txtToken.TabIndex = 5;
            // 
            // lblAuthKey
            // 
            this.lblAuthKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAuthKey.AutoSize = true;
            this.lblAuthKey.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblAuthKey.Font = new System.Drawing.Font("Tahoma", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAuthKey.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblAuthKey.Location = new System.Drawing.Point(100, 132);
            this.lblAuthKey.Name = "lblAuthKey";
            this.lblAuthKey.Size = new System.Drawing.Size(285, 58);
            this.lblAuthKey.TabIndex = 9;
            this.lblAuthKey.TabStop = true;
            this.lblAuthKey.Text = "00000000";
            this.lblAuthKey.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAuthKey.Click += new System.EventHandler(this.lblAuthKey_Click);
            // 
            // timGenerate
            // 
            this.timGenerate.Enabled = true;
            this.timGenerate.Interval = 200;
            this.timGenerate.Tick += new System.EventHandler(this.timGenerate_Tick);
            // 
            // ManageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 372);
            this.Controls.Add(this.tlpNew);
            this.Font = System.Drawing.SystemFonts.DefaultFont;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(660, 400);
            this.Name = "ManageForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Authenticators";
            this.tlpNew.ResumeLayout(false);
            this.tlpNew.PerformLayout();
            this.ssStatus.ResumeLayout(false);
            this.ssStatus.PerformLayout();
            this.splSplit.Panel1.ResumeLayout(false);
            this.splSplit.Panel2.ResumeLayout(false);
            this.splSplit.ResumeLayout(false);
            this.tlpLeftPane.ResumeLayout(false);
            this.tlpLeftPane.PerformLayout();
            this.tsTools.ResumeLayout(false);
            this.tsTools.PerformLayout();
            this.tlpRight.ResumeLayout(false);
            this.tlpRight.PerformLayout();
            this.grpNew.ResumeLayout(false);
            this.grpNew.PerformLayout();
            this.tlpChooseRegion.ResumeLayout(false);
            this.tlpChooseRegion.PerformLayout();
            this.tlpRegionButtons.ResumeLayout(false);
            this.tlpRegionButtons.PerformLayout();
            this.tlpInfo.ResumeLayout(false);
            this.tlpInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpNew;
        private System.Windows.Forms.StatusStrip ssStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblCount;
        private System.Windows.Forms.ToolStripProgressBar barTime;
        private System.Windows.Forms.SplitContainer splSplit;
        private System.Windows.Forms.TableLayoutPanel tlpLeftPane;
        private System.Windows.Forms.ToolStrip tsTools;
        private System.Windows.Forms.ToolStripButton mnuNew;
        private System.Windows.Forms.ToolStripButton mnuDelete;
        private System.Windows.Forms.ListBox lstAuths;
        private System.Windows.Forms.TableLayoutPanel tlpRight;
        private System.Windows.Forms.GroupBox grpNew;
        private System.Windows.Forms.TableLayoutPanel tlpChooseRegion;
        private System.Windows.Forms.TableLayoutPanel tlpRegionButtons;
        private System.Windows.Forms.Button btnRegionEU;
        private System.Windows.Forms.Button btnRegionUS;
        private System.Windows.Forms.Label lblNewInfo;
        private System.Windows.Forms.TableLayoutPanel tlpInfo;
        private System.Windows.Forms.Label lblRegion;
        private System.Windows.Forms.Label lblToken;
        private System.Windows.Forms.Label lblSerial;
        private System.Windows.Forms.TextBox txtSerial;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblAuthCode;
        private System.Windows.Forms.TextBox txtRegion;
        private System.Windows.Forms.TextBox txtToken;
        private AntiAliasLabel lblAuthKey;
        private System.Windows.Forms.ToolStripSeparator mnuSeparator;
        private System.Windows.Forms.ToolStripButton mnuImport;
        private System.Windows.Forms.ToolStripButton mnuExport;
        private System.Windows.Forms.Timer timGenerate;


    }
}