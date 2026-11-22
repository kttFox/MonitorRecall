namespace MonitorRecall
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.menuMain = new MenuStrip();
			this.mnuFile = new ToolStripMenuItem();
			this.mnuExport = new ToolStripMenuItem();
			this.mnuImport = new ToolStripMenuItem();
			this.mnuFileSep = new ToolStripSeparator();
			this.mnuFileExit = new ToolStripMenuItem();
			this.pnlRoot = new Panel();
			this.splitWindows = new SplitContainer();
			this.splitMain = new SplitContainer();
			this.grpHistory = new GroupBox();
			this.lstHistory = new ListBox();
			this.btnRestoreHistory = new Button();
			this.grpLog = new GroupBox();
			this.txtLog = new TextBox();
			this.grpOptions = new GroupBox();
			this.flpOptions = new FlowLayoutPanel();
			this.chkWatch = new CheckBox();
			this.lblInterval = new Label();
			this.numInterval = new NumericUpDown();
			this.lblDelay = new Label();
			this.numDelay = new NumericUpDown();
			this.chkStartup = new CheckBox();
			this.flpButtons = new FlowLayoutPanel();
			this.btnSave = new Button();
			this.btnRestore = new Button();
			this.btnBaseline = new Button();
			this.grpStatus = new GroupBox();
			this.lblWatchCaption = new Label();
			this.lblWatch = new Label();
			this.lblMonitorCaption = new Label();
			this.lblMonitor = new Label();
			this.lblSavedCaption = new Label();
			this.lblSaved = new Label();
			this.grpWindows = new GroupBox();
			this.lvWindows = new ListView();
			this.colProcess = new ColumnHeader();
			this.colTitle = new ColumnHeader();
			this.colState = new ColumnHeader();
			this.colPosition = new ColumnHeader();
			this.colSize = new ColumnHeader();
			this.colMonitor = new ColumnHeader();
			this.imgIcons = new ImageList( this.components );
			this.trayIcon = new NotifyIcon( this.components );
			this.trayMenu = new ContextMenuStrip( this.components );
			this.mnuShow = new ToolStripMenuItem();
			this.mnuSep1 = new ToolStripSeparator();
			this.mnuSave = new ToolStripMenuItem();
			this.mnuRestore = new ToolStripMenuItem();
			this.mnuSep2 = new ToolStripSeparator();
			this.mnuExit = new ToolStripMenuItem();
			this.dlgExport = new SaveFileDialog();
			this.dlgImport = new OpenFileDialog();
			this.menuMain.SuspendLayout();
			this.pnlRoot.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize)this.splitWindows ).BeginInit();
			this.splitWindows.Panel1.SuspendLayout();
			this.splitWindows.Panel2.SuspendLayout();
			this.splitWindows.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize)this.splitMain ).BeginInit();
			this.splitMain.Panel1.SuspendLayout();
			this.splitMain.Panel2.SuspendLayout();
			this.splitMain.SuspendLayout();
			this.grpHistory.SuspendLayout();
			this.grpLog.SuspendLayout();
			this.grpOptions.SuspendLayout();
			this.flpOptions.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize)this.numInterval ).BeginInit();
			( (System.ComponentModel.ISupportInitialize)this.numDelay ).BeginInit();
			this.flpButtons.SuspendLayout();
			this.grpStatus.SuspendLayout();
			this.grpWindows.SuspendLayout();
			this.trayMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuMain
			// 
			this.menuMain.Items.AddRange( new ToolStripItem[] { this.mnuFile } );
			this.menuMain.Location = new Point( 0, 0 );
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new Size( 820, 24 );
			this.menuMain.TabIndex = 1;
			// 
			// mnuFile
			// 
			this.mnuFile.DropDownItems.AddRange( new ToolStripItem[] { this.mnuExport, this.mnuImport, this.mnuFileSep, this.mnuFileExit } );
			this.mnuFile.Name = "mnuFile";
			this.mnuFile.Size = new Size( 67, 20 );
			this.mnuFile.Text = "ファイル(&F)";
			// 
			// mnuExport
			// 
			this.mnuExport.Name = "mnuExport";
			this.mnuExport.Size = new Size( 150, 22 );
			this.mnuExport.Text = "エクスポート(&E)...";
			this.mnuExport.Click +=  this.mnuExport_Click ;
			// 
			// mnuImport
			// 
			this.mnuImport.Name = "mnuImport";
			this.mnuImport.Size = new Size( 150, 22 );
			this.mnuImport.Text = "インポート(&I)...";
			this.mnuImport.Click +=  this.mnuImport_Click ;
			// 
			// mnuFileSep
			// 
			this.mnuFileSep.Name = "mnuFileSep";
			this.mnuFileSep.Size = new Size( 147, 6 );
			// 
			// mnuFileExit
			// 
			this.mnuFileExit.Name = "mnuFileExit";
			this.mnuFileExit.Size = new Size( 150, 22 );
			this.mnuFileExit.Text = "終了(&X)";
			this.mnuFileExit.Click +=  this.mnuExit_Click ;
			// 
			// pnlRoot
			// 
			this.pnlRoot.Controls.Add( this.splitWindows );
			this.pnlRoot.Dock = DockStyle.Fill;
			this.pnlRoot.Location = new Point( 0, 24 );
			this.pnlRoot.Name = "pnlRoot";
			this.pnlRoot.Padding = new Padding( 8 );
			this.pnlRoot.Size = new Size( 820, 560 );
			this.pnlRoot.TabIndex = 0;
			// 
			// splitWindows
			// 
			this.splitWindows.Dock = DockStyle.Fill;
			this.splitWindows.FixedPanel = FixedPanel.Panel1;
			this.splitWindows.Location = new Point( 8, 8 );
			this.splitWindows.Name = "splitWindows";
			// 
			// splitWindows.Panel1
			// 
			this.splitWindows.Panel1.Controls.Add( this.splitMain );
			this.splitWindows.Panel1.Controls.Add( this.grpOptions );
			this.splitWindows.Panel1.Controls.Add( this.flpButtons );
			this.splitWindows.Panel1.Controls.Add( this.grpStatus );
			// 
			// splitWindows.Panel2
			// 
			this.splitWindows.Panel2.Controls.Add( this.grpWindows );
			this.splitWindows.Size = new Size( 804, 544 );
			this.splitWindows.SplitterDistance = 420;
			this.splitWindows.TabIndex = 0;
			// 
			// splitMain
			// 
			this.splitMain.Dock = DockStyle.Fill;
			this.splitMain.Location = new Point( 0, 223 );
			this.splitMain.Name = "splitMain";
			this.splitMain.Orientation = Orientation.Horizontal;
			// 
			// splitMain.Panel1
			// 
			this.splitMain.Panel1.Controls.Add( this.grpHistory );
			// 
			// splitMain.Panel2
			// 
			this.splitMain.Panel2.Controls.Add( this.grpLog );
			this.splitMain.Size = new Size( 420, 321 );
			this.splitMain.SplitterDistance = 198;
			this.splitMain.TabIndex = 0;
			// 
			// grpHistory
			// 
			this.grpHistory.Controls.Add( this.lstHistory );
			this.grpHistory.Controls.Add( this.btnRestoreHistory );
			this.grpHistory.Dock = DockStyle.Fill;
			this.grpHistory.Location = new Point( 0, 0 );
			this.grpHistory.Name = "grpHistory";
			this.grpHistory.Size = new Size( 420, 198 );
			this.grpHistory.TabIndex = 0;
			this.grpHistory.TabStop = false;
			this.grpHistory.Text = "履歴";
			// 
			// lstHistory
			// 
			this.lstHistory.Dock = DockStyle.Fill;
			this.lstHistory.IntegralHeight = false;
			this.lstHistory.Location = new Point( 3, 19 );
			this.lstHistory.Name = "lstHistory";
			this.lstHistory.Size = new Size( 414, 147 );
			this.lstHistory.TabIndex = 0;
			this.lstHistory.SelectedIndexChanged +=  this.lstHistory_SelectedIndexChanged ;
			this.lstHistory.DoubleClick +=  this.btnRestoreHistory_Click ;
			// 
			// btnRestoreHistory
			// 
			this.btnRestoreHistory.Dock = DockStyle.Bottom;
			this.btnRestoreHistory.Location = new Point( 3, 166 );
			this.btnRestoreHistory.Name = "btnRestoreHistory";
			this.btnRestoreHistory.Size = new Size( 414, 29 );
			this.btnRestoreHistory.TabIndex = 1;
			this.btnRestoreHistory.Text = "選択した配置に復元";
			this.btnRestoreHistory.UseVisualStyleBackColor = true;
			this.btnRestoreHistory.Click +=  this.btnRestoreHistory_Click ;
			// 
			// grpLog
			// 
			this.grpLog.Controls.Add( this.txtLog );
			this.grpLog.Dock = DockStyle.Fill;
			this.grpLog.Location = new Point( 0, 0 );
			this.grpLog.Name = "grpLog";
			this.grpLog.Size = new Size( 420, 119 );
			this.grpLog.TabIndex = 0;
			this.grpLog.TabStop = false;
			this.grpLog.Text = "ログ";
			// 
			// txtLog
			// 
			this.txtLog.BackColor = SystemColors.Window;
			this.txtLog.Dock = DockStyle.Fill;
			this.txtLog.Location = new Point( 3, 19 );
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ReadOnly = true;
			this.txtLog.ScrollBars = ScrollBars.Vertical;
			this.txtLog.Size = new Size( 414, 97 );
			this.txtLog.TabIndex = 0;
			// 
			// grpOptions
			// 
			this.grpOptions.Controls.Add( this.flpOptions );
			this.grpOptions.Dock = DockStyle.Top;
			this.grpOptions.Location = new Point( 0, 131 );
			this.grpOptions.Name = "grpOptions";
			this.grpOptions.Size = new Size( 420, 92 );
			this.grpOptions.TabIndex = 2;
			this.grpOptions.TabStop = false;
			this.grpOptions.Text = "設定";
			// 
			// flpOptions
			// 
			this.flpOptions.Controls.Add( this.chkWatch );
			this.flpOptions.Controls.Add( this.lblInterval );
			this.flpOptions.Controls.Add( this.numInterval );
			this.flpOptions.Controls.Add( this.chkStartup );
			this.flpOptions.Controls.Add( this.lblDelay );
			this.flpOptions.Controls.Add( this.numDelay );
			this.flpOptions.Dock = DockStyle.Fill;
			this.flpOptions.Location = new Point( 3, 19 );
			this.flpOptions.Name = "flpOptions";
			this.flpOptions.Padding = new Padding( 4 );
			this.flpOptions.Size = new Size( 414, 70 );
			this.flpOptions.TabIndex = 0;
			// 
			// chkWatch
			// 
			this.chkWatch.AutoSize = true;
			this.chkWatch.Location = new Point( 7, 9 );
			this.chkWatch.Margin = new Padding( 3, 5, 19, 3 );
			this.chkWatch.Name = "chkWatch";
			this.chkWatch.Size = new Size( 74, 19 );
			this.chkWatch.TabIndex = 0;
			this.chkWatch.Text = "自動監視";
			this.chkWatch.UseVisualStyleBackColor = true;
			this.chkWatch.CheckedChanged +=  this.chkWatch_CheckedChanged ;
			// 
			// lblInterval
			// 
			this.lblInterval.AutoSize = true;
			this.lblInterval.Location = new Point( 103, 11 );
			this.lblInterval.Margin = new Padding( 3, 7, 0, 0 );
			this.lblInterval.Name = "lblInterval";
			this.lblInterval.Size = new Size( 78, 15 );
			this.lblInterval.TabIndex = 1;
			this.lblInterval.Text = "保存間隔(秒):";
			// 
			// numInterval
			// 
			this.numInterval.Location = new Point( 184, 7 );
			this.numInterval.Margin = new Padding( 3, 3, 19, 3 );
			this.numInterval.Maximum = new decimal( new int[] { 3600, 0, 0, 0 } );
			this.numInterval.Minimum = new decimal( new int[] { 1, 0, 0, 0 } );
			this.numInterval.Name = "numInterval";
			this.numInterval.Size = new Size( 60, 23 );
			this.numInterval.TabIndex = 2;
			this.numInterval.Value = new decimal( new int[] { 10, 0, 0, 0 } );
			this.numInterval.ValueChanged +=  this.numInterval_ValueChanged ;
			// 
			// lblDelay
			// 
			this.lblDelay.AutoSize = true;
			this.lblDelay.Location = new Point( 7, 40 );
			this.lblDelay.Margin = new Padding( 3, 7, 0, 0 );
			this.lblDelay.Name = "lblDelay";
			this.lblDelay.Size = new Size( 104, 15 );
			this.lblDelay.TabIndex = 3;
			this.lblDelay.Text = "復元までの待ち(秒):";
			// 
			// numDelay
			// 
			this.numDelay.Location = new Point( 114, 36 );
			this.numDelay.Margin = new Padding( 3, 3, 19, 3 );
			this.numDelay.Maximum = new decimal( new int[] { 120, 0, 0, 0 } );
			this.numDelay.Name = "numDelay";
			this.numDelay.Size = new Size( 60, 23 );
			this.numDelay.TabIndex = 4;
			this.numDelay.Value = new decimal( new int[] { 5, 0, 0, 0 } );
			this.numDelay.ValueChanged +=  this.numDelay_ValueChanged ;
			// 
			// chkStartup
			// 
			this.chkStartup.AutoSize = true;
			this.chkStartup.Location = new Point( 266, 9 );
			this.chkStartup.Margin = new Padding( 3, 5, 3, 3 );
			this.chkStartup.Name = "chkStartup";
			this.chkStartup.Size = new Size( 107, 19 );
			this.chkStartup.TabIndex = 5;
			this.chkStartup.Text = "ログオン時に起動";
			this.chkStartup.UseVisualStyleBackColor = true;
			this.chkStartup.CheckedChanged +=  this.chkStartup_CheckedChanged ;
			// 
			// flpButtons
			// 
			this.flpButtons.AutoSize = true;
			this.flpButtons.Controls.Add( this.btnSave );
			this.flpButtons.Controls.Add( this.btnRestore );
			this.flpButtons.Controls.Add( this.btnBaseline );
			this.flpButtons.Dock = DockStyle.Top;
			this.flpButtons.Location = new Point( 0, 96 );
			this.flpButtons.Name = "flpButtons";
			this.flpButtons.Size = new Size( 420, 35 );
			this.flpButtons.TabIndex = 1;
			// 
			// btnSave
			// 
			this.btnSave.AutoSize = true;
			this.btnSave.Location = new Point( 3, 3 );
			this.btnSave.Name = "btnSave";
			this.btnSave.Padding = new Padding( 8, 2, 8, 2 );
			this.btnSave.Size = new Size( 90, 29 );
			this.btnSave.TabIndex = 0;
			this.btnSave.Text = "今すぐ保存";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click +=  this.btnSave_Click ;
			// 
			// btnRestore
			// 
			this.btnRestore.AutoSize = true;
			this.btnRestore.Location = new Point( 99, 3 );
			this.btnRestore.Name = "btnRestore";
			this.btnRestore.Padding = new Padding( 8, 2, 8, 2 );
			this.btnRestore.Size = new Size( 90, 29 );
			this.btnRestore.TabIndex = 1;
			this.btnRestore.Text = "今すぐ復元";
			this.btnRestore.UseVisualStyleBackColor = true;
			this.btnRestore.Click +=  this.btnRestore_Click ;
			// 
			// btnBaseline
			// 
			this.btnBaseline.AutoSize = true;
			this.btnBaseline.Location = new Point( 195, 3 );
			this.btnBaseline.Name = "btnBaseline";
			this.btnBaseline.Padding = new Padding( 8, 2, 8, 2 );
			this.btnBaseline.Size = new Size( 190, 29 );
			this.btnBaseline.TabIndex = 2;
			this.btnBaseline.Text = "現在のモニター構成を基準にする";
			this.btnBaseline.UseVisualStyleBackColor = true;
			this.btnBaseline.Click +=  this.btnBaseline_Click ;
			// 
			// grpStatus
			// 
			this.grpStatus.Controls.Add( this.lblWatchCaption );
			this.grpStatus.Controls.Add( this.lblWatch );
			this.grpStatus.Controls.Add( this.lblMonitorCaption );
			this.grpStatus.Controls.Add( this.lblMonitor );
			this.grpStatus.Controls.Add( this.lblSavedCaption );
			this.grpStatus.Controls.Add( this.lblSaved );
			this.grpStatus.Dock = DockStyle.Top;
			this.grpStatus.Location = new Point( 0, 0 );
			this.grpStatus.Name = "grpStatus";
			this.grpStatus.Size = new Size( 420, 96 );
			this.grpStatus.TabIndex = 0;
			this.grpStatus.TabStop = false;
			this.grpStatus.Text = "状態";
			// 
			// lblWatchCaption
			// 
			this.lblWatchCaption.AutoSize = true;
			this.lblWatchCaption.Location = new Point( 12, 24 );
			this.lblWatchCaption.Margin = new Padding( 3, 4, 8, 4 );
			this.lblWatchCaption.Name = "lblWatchCaption";
			this.lblWatchCaption.Size = new Size( 58, 15 );
			this.lblWatchCaption.TabIndex = 0;
			this.lblWatchCaption.Text = "監視状態:";
			// 
			// lblWatch
			// 
			this.lblWatch.AutoSize = true;
			this.lblWatch.Font = new Font( "Yu Gothic UI", 9F, FontStyle.Bold );
			this.lblWatch.Location = new Point( 80, 24 );
			this.lblWatch.Margin = new Padding( 3, 4, 3, 4 );
			this.lblWatch.Name = "lblWatch";
			this.lblWatch.Size = new Size( 43, 15 );
			this.lblWatch.TabIndex = 1;
			this.lblWatch.Text = "停止中";
			// 
			// lblMonitorCaption
			// 
			this.lblMonitorCaption.AutoSize = true;
			this.lblMonitorCaption.Location = new Point( 12, 47 );
			this.lblMonitorCaption.Margin = new Padding( 3, 4, 8, 4 );
			this.lblMonitorCaption.Name = "lblMonitorCaption";
			this.lblMonitorCaption.Size = new Size( 45, 15 );
			this.lblMonitorCaption.TabIndex = 2;
			this.lblMonitorCaption.Text = "モニター:";
			// 
			// lblMonitor
			// 
			this.lblMonitor.AutoSize = true;
			this.lblMonitor.Location = new Point( 80, 47 );
			this.lblMonitor.Margin = new Padding( 3, 4, 3, 4 );
			this.lblMonitor.Name = "lblMonitor";
			this.lblMonitor.Size = new Size( 12, 15 );
			this.lblMonitor.TabIndex = 3;
			this.lblMonitor.Text = "-";
			// 
			// lblSavedCaption
			// 
			this.lblSavedCaption.AutoSize = true;
			this.lblSavedCaption.Location = new Point( 12, 70 );
			this.lblSavedCaption.Margin = new Padding( 3, 4, 8, 4 );
			this.lblSavedCaption.Name = "lblSavedCaption";
			this.lblSavedCaption.Size = new Size( 58, 15 );
			this.lblSavedCaption.TabIndex = 4;
			this.lblSavedCaption.Text = "最終保存:";
			// 
			// lblSaved
			// 
			this.lblSaved.AutoSize = true;
			this.lblSaved.Location = new Point( 80, 70 );
			this.lblSaved.Margin = new Padding( 3, 4, 3, 4 );
			this.lblSaved.Name = "lblSaved";
			this.lblSaved.Size = new Size( 12, 15 );
			this.lblSaved.TabIndex = 5;
			this.lblSaved.Text = "-";
			// 
			// grpWindows
			// 
			this.grpWindows.Controls.Add( this.lvWindows );
			this.grpWindows.Dock = DockStyle.Fill;
			this.grpWindows.Location = new Point( 0, 0 );
			this.grpWindows.Name = "grpWindows";
			this.grpWindows.Size = new Size( 380, 544 );
			this.grpWindows.TabIndex = 0;
			this.grpWindows.TabStop = false;
			this.grpWindows.Text = "保存済みのウインドウ";
			// 
			// lvWindows
			// 
			this.lvWindows.Columns.AddRange( new ColumnHeader[] { this.colProcess, this.colTitle, this.colState, this.colPosition, this.colSize, this.colMonitor } );
			this.lvWindows.Dock = DockStyle.Fill;
			this.lvWindows.FullRowSelect = true;
			this.lvWindows.GridLines = true;
			this.lvWindows.Location = new Point( 3, 19 );
			this.lvWindows.Name = "lvWindows";
			this.lvWindows.Size = new Size( 374, 522 );
			this.lvWindows.SmallImageList = this.imgIcons;
			this.lvWindows.TabIndex = 0;
			this.lvWindows.UseCompatibleStateImageBehavior = false;
			this.lvWindows.View = View.Details;
			// 
			// colProcess
			// 
			this.colProcess.Text = "プロセス";
			this.colProcess.Width = 120;
			// 
			// colTitle
			// 
			this.colTitle.Text = "タイトル";
			this.colTitle.Width = 300;
			// 
			// colState
			// 
			this.colState.Text = "状態";
			// 
			// colPosition
			// 
			this.colPosition.Text = "位置";
			this.colPosition.Width = 110;
			// 
			// colSize
			// 
			this.colSize.Text = "サイズ";
			this.colSize.Width = 90;
			// 
			// colMonitor
			// 
			this.colMonitor.Text = "モニター";
			this.colMonitor.Width = 90;
			// 
			// imgIcons
			// 
			this.imgIcons.ColorDepth = ColorDepth.Depth32Bit;
			this.imgIcons.ImageSize = new Size( 16, 16 );
			this.imgIcons.TransparentColor = Color.Transparent;
			// 
			// trayIcon
			// 
			this.trayIcon.ContextMenuStrip = this.trayMenu;
			this.trayIcon.Text = "MonitorRecall";
			this.trayIcon.Visible = true;
			this.trayIcon.DoubleClick +=  this.mnuShow_Click ;
			// 
			// trayMenu
			// 
			this.trayMenu.Items.AddRange( new ToolStripItem[] { this.mnuShow, this.mnuSep1, this.mnuSave, this.mnuRestore, this.mnuSep2, this.mnuExit } );
			this.trayMenu.Name = "trayMenu";
			this.trayMenu.Size = new Size( 152, 104 );
			// 
			// mnuShow
			// 
			this.mnuShow.Name = "mnuShow";
			this.mnuShow.Size = new Size( 151, 22 );
			this.mnuShow.Text = "ウインドウを表示";
			this.mnuShow.Click +=  this.mnuShow_Click ;
			// 
			// mnuSep1
			// 
			this.mnuSep1.Name = "mnuSep1";
			this.mnuSep1.Size = new Size( 148, 6 );
			// 
			// mnuSave
			// 
			this.mnuSave.Name = "mnuSave";
			this.mnuSave.Size = new Size( 151, 22 );
			this.mnuSave.Text = "今すぐ保存";
			this.mnuSave.Click +=  this.btnSave_Click ;
			// 
			// mnuRestore
			// 
			this.mnuRestore.Name = "mnuRestore";
			this.mnuRestore.Size = new Size( 151, 22 );
			this.mnuRestore.Text = "今すぐ復元";
			this.mnuRestore.Click +=  this.btnRestore_Click ;
			// 
			// mnuSep2
			// 
			this.mnuSep2.Name = "mnuSep2";
			this.mnuSep2.Size = new Size( 148, 6 );
			// 
			// mnuExit
			// 
			this.mnuExit.Name = "mnuExit";
			this.mnuExit.Size = new Size( 151, 22 );
			this.mnuExit.Text = "終了";
			this.mnuExit.Click +=  this.mnuExit_Click ;
			// 
			// dlgExport
			// 
			this.dlgExport.DefaultExt = "json";
			this.dlgExport.FileName = "MonitorRecall.json";
			this.dlgExport.Filter = "MonitorRecall 設定 (*.json)|*.json|すべてのファイル (*.*)|*.*";
			this.dlgExport.Title = "設定のエクスポート";
			// 
			// dlgImport
			// 
			this.dlgImport.DefaultExt = "json";
			this.dlgImport.Filter = "MonitorRecall 設定 (*.json)|*.json|すべてのファイル (*.*)|*.*";
			this.dlgImport.Title = "設定のインポート";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new SizeF( 96F, 96F );
			this.AutoScaleMode = AutoScaleMode.Dpi;
			this.ClientSize = new Size( 820, 584 );
			this.Controls.Add( this.pnlRoot );
			this.Controls.Add( this.menuMain );
			this.Font = new Font( "Yu Gothic UI", 9F );
			this.MainMenuStrip = this.menuMain;
			this.MinimumSize = new Size( 600, 420 );
			this.Name = "MainForm";
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "MonitorRecall - マルチモニターのウインドウ配置を記憶・復元";
			this.menuMain.ResumeLayout( false );
			this.menuMain.PerformLayout();
			this.pnlRoot.ResumeLayout( false );
			this.splitWindows.Panel1.ResumeLayout( false );
			this.splitWindows.Panel1.PerformLayout();
			this.splitWindows.Panel2.ResumeLayout( false );
			( (System.ComponentModel.ISupportInitialize)this.splitWindows ).EndInit();
			this.splitWindows.ResumeLayout( false );
			this.splitMain.Panel1.ResumeLayout( false );
			this.splitMain.Panel2.ResumeLayout( false );
			( (System.ComponentModel.ISupportInitialize)this.splitMain ).EndInit();
			this.splitMain.ResumeLayout( false );
			this.grpHistory.ResumeLayout( false );
			this.grpLog.ResumeLayout( false );
			this.grpLog.PerformLayout();
			this.grpOptions.ResumeLayout( false );
			this.flpOptions.ResumeLayout( false );
			this.flpOptions.PerformLayout();
			( (System.ComponentModel.ISupportInitialize)this.numInterval ).EndInit();
			( (System.ComponentModel.ISupportInitialize)this.numDelay ).EndInit();
			this.flpButtons.ResumeLayout( false );
			this.flpButtons.PerformLayout();
			this.grpStatus.ResumeLayout( false );
			this.grpStatus.PerformLayout();
			this.grpWindows.ResumeLayout( false );
			this.trayMenu.ResumeLayout( false );
			this.ResumeLayout( false );
			this.PerformLayout();
		}

		#endregion

		private MenuStrip menuMain;
        private ToolStripMenuItem mnuFile;
        private ToolStripMenuItem mnuExport;
        private ToolStripMenuItem mnuImport;
        private ToolStripSeparator mnuFileSep;
        private ToolStripMenuItem mnuFileExit;
        private Panel pnlRoot;
        private GroupBox grpStatus;
        private Label lblWatchCaption;
        private Label lblWatch;
        private Label lblMonitorCaption;
        private Label lblMonitor;
        private Label lblSavedCaption;
        private Label lblSaved;
        private FlowLayoutPanel flpButtons;
        private Button btnSave;
        private Button btnRestore;
        private Button btnBaseline;
        private GroupBox grpOptions;
        private FlowLayoutPanel flpOptions;
        private CheckBox chkWatch;
        private Label lblInterval;
        private NumericUpDown numInterval;
        private Label lblDelay;
        private NumericUpDown numDelay;
        private CheckBox chkStartup;
        private SplitContainer splitMain;
        private SplitContainer splitWindows;
        private GroupBox grpHistory;
        private ListBox lstHistory;
        private Button btnRestoreHistory;
        private GroupBox grpWindows;
        private ListView lvWindows;
        private ColumnHeader colProcess;
        private ColumnHeader colTitle;
        private ColumnHeader colState;
        private ColumnHeader colPosition;
        private ColumnHeader colSize;
        private ColumnHeader colMonitor;
        private ImageList imgIcons;
        private GroupBox grpLog;
        private TextBox txtLog;
        private NotifyIcon trayIcon;
        private SaveFileDialog dlgExport;
        private OpenFileDialog dlgImport;
        private ContextMenuStrip trayMenu;
        private ToolStripMenuItem mnuShow;
        private ToolStripSeparator mnuSep1;
        private ToolStripMenuItem mnuSave;
        private ToolStripMenuItem mnuRestore;
        private ToolStripSeparator mnuSep2;
        private ToolStripMenuItem mnuExit;
    }
}
