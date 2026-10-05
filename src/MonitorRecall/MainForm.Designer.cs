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
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuMain = new MenuStrip();
            mnuFile = new ToolStripMenuItem();
            mnuExport = new ToolStripMenuItem();
            mnuImport = new ToolStripMenuItem();
            mnuFileSep = new ToolStripSeparator();
            mnuFileExit = new ToolStripMenuItem();
            tlpRoot = new TableLayoutPanel();
            grpStatus = new GroupBox();
            tlpStatus = new TableLayoutPanel();
            lblWatchCaption = new Label();
            lblWatch = new Label();
            lblMonitorCaption = new Label();
            lblMonitor = new Label();
            lblSavedCaption = new Label();
            lblSaved = new Label();
            flpButtons = new FlowLayoutPanel();
            btnSave = new Button();
            btnRestore = new Button();
            btnBaseline = new Button();
            grpOptions = new GroupBox();
            flpOptions = new FlowLayoutPanel();
            chkWatch = new CheckBox();
            lblInterval = new Label();
            numInterval = new NumericUpDown();
            lblDelay = new Label();
            numDelay = new NumericUpDown();
            chkStartup = new CheckBox();
            grpWindows = new GroupBox();
            lvWindows = new ListView();
            colProcess = new ColumnHeader();
            colTitle = new ColumnHeader();
            colState = new ColumnHeader();
            colPosition = new ColumnHeader();
            colSize = new ColumnHeader();
            colMonitor = new ColumnHeader();
            imgIcons = new ImageList(components);
            grpLog = new GroupBox();
            txtLog = new TextBox();
            trayIcon = new NotifyIcon(components);
            dlgExport = new SaveFileDialog();
            dlgImport = new OpenFileDialog();
            trayMenu = new ContextMenuStrip(components);
            mnuShow = new ToolStripMenuItem();
            mnuSep1 = new ToolStripSeparator();
            mnuSave = new ToolStripMenuItem();
            mnuRestore = new ToolStripMenuItem();
            mnuSep2 = new ToolStripSeparator();
            mnuExit = new ToolStripMenuItem();
            menuMain.SuspendLayout();
            tlpRoot.SuspendLayout();
            grpStatus.SuspendLayout();
            tlpStatus.SuspendLayout();
            flpButtons.SuspendLayout();
            grpOptions.SuspendLayout();
            flpOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numInterval).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDelay).BeginInit();
            grpWindows.SuspendLayout();
            grpLog.SuspendLayout();
            trayMenu.SuspendLayout();
            SuspendLayout();
            //
            // menuMain
            //
            menuMain.Items.AddRange(new ToolStripItem[] { mnuFile });
            menuMain.Location = new Point(0, 0);
            menuMain.Name = "menuMain";
            menuMain.Size = new Size(820, 24);
            menuMain.TabIndex = 1;
            //
            // mnuFile
            //
            mnuFile.DropDownItems.AddRange(new ToolStripItem[] { mnuExport, mnuImport, mnuFileSep, mnuFileExit });
            mnuFile.Name = "mnuFile";
            mnuFile.Size = new Size(67, 20);
            mnuFile.Text = "ファイル(&F)";
            //
            // mnuExport
            //
            mnuExport.Name = "mnuExport";
            mnuExport.Size = new Size(180, 22);
            mnuExport.Text = "エクスポート(&E)...";
            mnuExport.Click += mnuExport_Click;
            //
            // mnuImport
            //
            mnuImport.Name = "mnuImport";
            mnuImport.Size = new Size(180, 22);
            mnuImport.Text = "インポート(&I)...";
            mnuImport.Click += mnuImport_Click;
            //
            // mnuFileSep
            //
            mnuFileSep.Name = "mnuFileSep";
            mnuFileSep.Size = new Size(177, 6);
            //
            // mnuFileExit
            //
            mnuFileExit.Name = "mnuFileExit";
            mnuFileExit.Size = new Size(180, 22);
            mnuFileExit.Text = "終了(&X)";
            mnuFileExit.Click += mnuExit_Click;
            //
            // tlpRoot
            //
            tlpRoot.ColumnCount = 1;
            tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpRoot.Controls.Add(grpStatus, 0, 0);
            tlpRoot.Controls.Add(flpButtons, 0, 1);
            tlpRoot.Controls.Add(grpOptions, 0, 2);
            tlpRoot.Controls.Add(grpWindows, 0, 3);
            tlpRoot.Controls.Add(grpLog, 0, 4);
            tlpRoot.Dock = DockStyle.Fill;
            tlpRoot.Location = new Point(0, 24);
            tlpRoot.Name = "tlpRoot";
            tlpRoot.Padding = new Padding(8);
            tlpRoot.RowCount = 5;
            tlpRoot.RowStyles.Add(new RowStyle());
            tlpRoot.RowStyles.Add(new RowStyle());
            tlpRoot.RowStyles.Add(new RowStyle());
            tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            tlpRoot.Size = new Size(820, 560);
            tlpRoot.TabIndex = 0;
            //
            // grpStatus
            //
            grpStatus.AutoSize = true;
            grpStatus.Controls.Add(tlpStatus);
            grpStatus.Dock = DockStyle.Fill;
            grpStatus.Location = new Point(11, 11);
            grpStatus.Name = "grpStatus";
            grpStatus.Size = new Size(798, 100);
            grpStatus.TabIndex = 0;
            grpStatus.TabStop = false;
            grpStatus.Text = "状態";
            //
            // tlpStatus
            //
            tlpStatus.AutoSize = true;
            tlpStatus.ColumnCount = 2;
            tlpStatus.ColumnStyles.Add(new ColumnStyle());
            tlpStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpStatus.Controls.Add(lblWatchCaption, 0, 0);
            tlpStatus.Controls.Add(lblWatch, 1, 0);
            tlpStatus.Controls.Add(lblMonitorCaption, 0, 1);
            tlpStatus.Controls.Add(lblMonitor, 1, 1);
            tlpStatus.Controls.Add(lblSavedCaption, 0, 2);
            tlpStatus.Controls.Add(lblSaved, 1, 2);
            tlpStatus.Dock = DockStyle.Fill;
            tlpStatus.Location = new Point(3, 19);
            tlpStatus.Name = "tlpStatus";
            tlpStatus.Padding = new Padding(4);
            tlpStatus.RowCount = 3;
            tlpStatus.RowStyles.Add(new RowStyle());
            tlpStatus.RowStyles.Add(new RowStyle());
            tlpStatus.RowStyles.Add(new RowStyle());
            tlpStatus.Size = new Size(792, 78);
            tlpStatus.TabIndex = 0;
            //
            // lblWatchCaption
            //
            lblWatchCaption.AutoSize = true;
            lblWatchCaption.Location = new Point(7, 8);
            lblWatchCaption.Margin = new Padding(3, 4, 8, 4);
            lblWatchCaption.Name = "lblWatchCaption";
            lblWatchCaption.Size = new Size(58, 15);
            lblWatchCaption.TabIndex = 0;
            lblWatchCaption.Text = "監視状態:";
            //
            // lblWatch
            //
            lblWatch.AutoSize = true;
            lblWatch.Font = new Font("Yu Gothic UI", 9F, FontStyle.Bold);
            lblWatch.Location = new Point(76, 8);
            lblWatch.Margin = new Padding(3, 4, 3, 4);
            lblWatch.Name = "lblWatch";
            lblWatch.Size = new Size(43, 15);
            lblWatch.TabIndex = 1;
            lblWatch.Text = "停止中";
            //
            // lblMonitorCaption
            //
            lblMonitorCaption.AutoSize = true;
            lblMonitorCaption.Location = new Point(7, 31);
            lblMonitorCaption.Margin = new Padding(3, 4, 8, 4);
            lblMonitorCaption.Name = "lblMonitorCaption";
            lblMonitorCaption.Size = new Size(50, 15);
            lblMonitorCaption.TabIndex = 2;
            lblMonitorCaption.Text = "モニター:";
            //
            // lblMonitor
            //
            lblMonitor.AutoSize = true;
            lblMonitor.Location = new Point(76, 31);
            lblMonitor.Margin = new Padding(3, 4, 3, 4);
            lblMonitor.Name = "lblMonitor";
            lblMonitor.Size = new Size(12, 15);
            lblMonitor.TabIndex = 3;
            lblMonitor.Text = "-";
            //
            // lblSavedCaption
            //
            lblSavedCaption.AutoSize = true;
            lblSavedCaption.Location = new Point(7, 54);
            lblSavedCaption.Margin = new Padding(3, 4, 8, 4);
            lblSavedCaption.Name = "lblSavedCaption";
            lblSavedCaption.Size = new Size(58, 15);
            lblSavedCaption.TabIndex = 4;
            lblSavedCaption.Text = "最終保存:";
            //
            // lblSaved
            //
            lblSaved.AutoSize = true;
            lblSaved.Location = new Point(76, 54);
            lblSaved.Margin = new Padding(3, 4, 3, 4);
            lblSaved.Name = "lblSaved";
            lblSaved.Size = new Size(12, 15);
            lblSaved.TabIndex = 5;
            lblSaved.Text = "-";
            //
            // flpButtons
            //
            flpButtons.AutoSize = true;
            flpButtons.Controls.Add(btnSave);
            flpButtons.Controls.Add(btnRestore);
            flpButtons.Controls.Add(btnBaseline);
            flpButtons.Dock = DockStyle.Fill;
            flpButtons.Location = new Point(11, 117);
            flpButtons.Name = "flpButtons";
            flpButtons.Size = new Size(798, 35);
            flpButtons.TabIndex = 1;
            //
            // btnSave
            //
            btnSave.AutoSize = true;
            btnSave.Location = new Point(3, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(8, 2, 8, 2);
            btnSave.Size = new Size(90, 29);
            btnSave.TabIndex = 0;
            btnSave.Text = "今すぐ保存";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            //
            // btnRestore
            //
            btnRestore.AutoSize = true;
            btnRestore.Location = new Point(99, 3);
            btnRestore.Name = "btnRestore";
            btnRestore.Padding = new Padding(8, 2, 8, 2);
            btnRestore.Size = new Size(90, 29);
            btnRestore.TabIndex = 1;
            btnRestore.Text = "今すぐ復元";
            btnRestore.UseVisualStyleBackColor = true;
            btnRestore.Click += btnRestore_Click;
            //
            // btnBaseline
            //
            btnBaseline.AutoSize = true;
            btnBaseline.Location = new Point(195, 3);
            btnBaseline.Name = "btnBaseline";
            btnBaseline.Padding = new Padding(8, 2, 8, 2);
            btnBaseline.Size = new Size(190, 29);
            btnBaseline.TabIndex = 2;
            btnBaseline.Text = "現在のモニター構成を基準にする";
            btnBaseline.UseVisualStyleBackColor = true;
            btnBaseline.Click += btnBaseline_Click;
            //
            // grpOptions
            //
            grpOptions.AutoSize = true;
            grpOptions.Controls.Add(flpOptions);
            grpOptions.Dock = DockStyle.Fill;
            grpOptions.Location = new Point(11, 158);
            grpOptions.Name = "grpOptions";
            grpOptions.Size = new Size(798, 60);
            grpOptions.TabIndex = 2;
            grpOptions.TabStop = false;
            grpOptions.Text = "設定";
            //
            // flpOptions
            //
            flpOptions.AutoSize = true;
            flpOptions.Controls.Add(chkWatch);
            flpOptions.Controls.Add(lblInterval);
            flpOptions.Controls.Add(numInterval);
            flpOptions.Controls.Add(lblDelay);
            flpOptions.Controls.Add(numDelay);
            flpOptions.Controls.Add(chkStartup);
            flpOptions.Dock = DockStyle.Fill;
            flpOptions.Location = new Point(3, 19);
            flpOptions.Name = "flpOptions";
            flpOptions.Padding = new Padding(4);
            flpOptions.Size = new Size(792, 38);
            flpOptions.TabIndex = 0;
            //
            // chkWatch
            //
            chkWatch.AutoSize = true;
            chkWatch.Location = new Point(7, 9);
            chkWatch.Margin = new Padding(3, 5, 19, 3);
            chkWatch.Name = "chkWatch";
            chkWatch.Size = new Size(74, 19);
            chkWatch.TabIndex = 0;
            chkWatch.Text = "自動監視";
            chkWatch.UseVisualStyleBackColor = true;
            chkWatch.CheckedChanged += chkWatch_CheckedChanged;
            //
            // lblInterval
            //
            lblInterval.AutoSize = true;
            lblInterval.Location = new Point(103, 11);
            lblInterval.Margin = new Padding(3, 7, 0, 0);
            lblInterval.Name = "lblInterval";
            lblInterval.Size = new Size(79, 15);
            lblInterval.TabIndex = 1;
            lblInterval.Text = "保存間隔(秒):";
            //
            // numInterval
            //
            numInterval.Location = new Point(185, 7);
            numInterval.Margin = new Padding(3, 3, 19, 3);
            numInterval.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            numInterval.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numInterval.Name = "numInterval";
            numInterval.Size = new Size(60, 23);
            numInterval.TabIndex = 2;
            numInterval.Value = new decimal(new int[] { 10, 0, 0, 0 });
            numInterval.ValueChanged += numInterval_ValueChanged;
            //
            // lblDelay
            //
            lblDelay.AutoSize = true;
            lblDelay.Location = new Point(267, 11);
            lblDelay.Margin = new Padding(3, 7, 0, 0);
            lblDelay.Name = "lblDelay";
            lblDelay.Size = new Size(109, 15);
            lblDelay.TabIndex = 3;
            lblDelay.Text = "復元までの待ち(秒):";
            //
            // numDelay
            //
            numDelay.Location = new Point(379, 7);
            numDelay.Margin = new Padding(3, 3, 19, 3);
            numDelay.Maximum = new decimal(new int[] { 120, 0, 0, 0 });
            numDelay.Name = "numDelay";
            numDelay.Size = new Size(60, 23);
            numDelay.TabIndex = 4;
            numDelay.Value = new decimal(new int[] { 5, 0, 0, 0 });
            numDelay.ValueChanged += numDelay_ValueChanged;
            //
            // chkStartup
            //
            chkStartup.AutoSize = true;
            chkStartup.Location = new Point(461, 9);
            chkStartup.Margin = new Padding(3, 5, 3, 3);
            chkStartup.Name = "chkStartup";
            chkStartup.Size = new Size(110, 19);
            chkStartup.TabIndex = 5;
            chkStartup.Text = "ログオン時に起動";
            chkStartup.UseVisualStyleBackColor = true;
            chkStartup.CheckedChanged += chkStartup_CheckedChanged;
            //
            // grpWindows
            //
            grpWindows.Controls.Add(lvWindows);
            grpWindows.Dock = DockStyle.Fill;
            grpWindows.Location = new Point(11, 224);
            grpWindows.Name = "grpWindows";
            grpWindows.Size = new Size(798, 227);
            grpWindows.TabIndex = 3;
            grpWindows.TabStop = false;
            grpWindows.Text = "保存済みのウインドウ";
            //
            // lvWindows
            //
            lvWindows.Columns.AddRange(new ColumnHeader[] { colProcess, colTitle, colState, colPosition, colSize, colMonitor });
            lvWindows.Dock = DockStyle.Fill;
            lvWindows.FullRowSelect = true;
            lvWindows.GridLines = true;
            lvWindows.HideSelection = false;
            lvWindows.Location = new Point(3, 19);
            lvWindows.Name = "lvWindows";
            lvWindows.Size = new Size(792, 205);
            lvWindows.SmallImageList = imgIcons;
            lvWindows.TabIndex = 0;
            lvWindows.UseCompatibleStateImageBehavior = false;
            lvWindows.View = View.Details;
            //
            // colProcess
            //
            colProcess.Text = "プロセス";
            colProcess.Width = 120;
            //
            // colTitle
            //
            colTitle.Text = "タイトル";
            colTitle.Width = 300;
            //
            // colState
            //
            colState.Text = "状態";
            //
            // colPosition
            //
            colPosition.Text = "位置";
            colPosition.Width = 110;
            //
            // colSize
            //
            colSize.Text = "サイズ";
            colSize.Width = 90;
            //
            // colMonitor
            //
            colMonitor.Text = "モニター";
            colMonitor.Width = 90;
            //
            // imgIcons
            //
            imgIcons.ColorDepth = ColorDepth.Depth32Bit;
            imgIcons.ImageSize = new Size(16, 16);
            imgIcons.TransparentColor = Color.Transparent;
            //
            // grpLog
            //
            grpLog.Controls.Add(txtLog);
            grpLog.Dock = DockStyle.Fill;
            grpLog.Location = new Point(11, 457);
            grpLog.Name = "grpLog";
            grpLog.Size = new Size(798, 92);
            grpLog.TabIndex = 4;
            grpLog.TabStop = false;
            grpLog.Text = "ログ";
            //
            // txtLog
            //
            txtLog.BackColor = SystemColors.Window;
            txtLog.Dock = DockStyle.Fill;
            txtLog.Location = new Point(3, 19);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(792, 70);
            txtLog.TabIndex = 0;
            //
            // trayIcon
            //
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Text = "MonitorRecall";
            trayIcon.Visible = true;
            trayIcon.DoubleClick += mnuShow_Click;
            //
            // dlgExport
            //
            dlgExport.DefaultExt = "json";
            dlgExport.FileName = "MonitorRecall.json";
            dlgExport.Filter = "MonitorRecall 設定 (*.json)|*.json|すべてのファイル (*.*)|*.*";
            dlgExport.Title = "設定のエクスポート";
            //
            // dlgImport
            //
            dlgImport.DefaultExt = "json";
            dlgImport.Filter = "MonitorRecall 設定 (*.json)|*.json|すべてのファイル (*.*)|*.*";
            dlgImport.Title = "設定のインポート";
            //
            // trayMenu
            //
            trayMenu.Items.AddRange(new ToolStripItem[] { mnuShow, mnuSep1, mnuSave, mnuRestore, mnuSep2, mnuExit });
            trayMenu.Name = "trayMenu";
            trayMenu.Size = new Size(161, 104);
            //
            // mnuShow
            //
            mnuShow.Name = "mnuShow";
            mnuShow.Size = new Size(160, 22);
            mnuShow.Text = "ウインドウを表示";
            mnuShow.Click += mnuShow_Click;
            //
            // mnuSep1
            //
            mnuSep1.Name = "mnuSep1";
            mnuSep1.Size = new Size(157, 6);
            //
            // mnuSave
            //
            mnuSave.Name = "mnuSave";
            mnuSave.Size = new Size(160, 22);
            mnuSave.Text = "今すぐ保存";
            mnuSave.Click += btnSave_Click;
            //
            // mnuRestore
            //
            mnuRestore.Name = "mnuRestore";
            mnuRestore.Size = new Size(160, 22);
            mnuRestore.Text = "今すぐ復元";
            mnuRestore.Click += btnRestore_Click;
            //
            // mnuSep2
            //
            mnuSep2.Name = "mnuSep2";
            mnuSep2.Size = new Size(157, 6);
            //
            // mnuExit
            //
            mnuExit.Name = "mnuExit";
            mnuExit.Size = new Size(160, 22);
            mnuExit.Text = "終了";
            mnuExit.Click += mnuExit_Click;
            //
            // MainForm
            //
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(820, 584);
            Controls.Add(tlpRoot);
            Controls.Add(menuMain);
            MainMenuStrip = menuMain;
            Font = new Font("Yu Gothic UI", 9F);
            MinimumSize = new Size(600, 420);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MonitorRecall - マルチモニターのウインドウ配置を記憶・復元";
            menuMain.ResumeLayout(false);
            menuMain.PerformLayout();
            tlpRoot.ResumeLayout(false);
            tlpRoot.PerformLayout();
            grpStatus.ResumeLayout(false);
            grpStatus.PerformLayout();
            tlpStatus.ResumeLayout(false);
            tlpStatus.PerformLayout();
            flpButtons.ResumeLayout(false);
            flpButtons.PerformLayout();
            grpOptions.ResumeLayout(false);
            grpOptions.PerformLayout();
            flpOptions.ResumeLayout(false);
            flpOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numInterval).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDelay).EndInit();
            grpWindows.ResumeLayout(false);
            grpLog.ResumeLayout(false);
            grpLog.PerformLayout();
            trayMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuMain;
        private ToolStripMenuItem mnuFile;
        private ToolStripMenuItem mnuExport;
        private ToolStripMenuItem mnuImport;
        private ToolStripSeparator mnuFileSep;
        private ToolStripMenuItem mnuFileExit;
        private TableLayoutPanel tlpRoot;
        private GroupBox grpStatus;
        private TableLayoutPanel tlpStatus;
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
