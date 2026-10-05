namespace MonitorRecall;

internal sealed partial class MainForm : Form
{
    readonly string _path;
    readonly AppSettings _settings;
    readonly LayoutWatcher _watcher;
    readonly bool _startHidden;
    bool _loading;
    bool _exiting;

    /// <summary>デザイナー用</summary>
    public MainForm() : this(LayoutManager.DefaultPath, new AppSettings(), false) { }

    public MainForm(string path, AppSettings settings, bool startHidden)
    {
        InitializeComponent();

        _path = path;
        _settings = settings;
        _startHidden = startHidden;
        _watcher = new LayoutWatcher(path, settings.IntervalSeconds, settings.RestoreDelaySeconds);

        Icon = LoadAppIcon(Size.Empty);
        trayIcon.Icon = LoadAppIcon(SystemInformation.SmallIconSize);
        int iconSize = LogicalToDeviceUnits(16);
        imgIcons.ImageSize = new Size(iconSize, iconSize);

        // 設定値を画面へ反映（反映中の変更イベントは無視）
        _loading = true;
        numInterval.Value = Math.Clamp(settings.IntervalSeconds, (int)numInterval.Minimum, (int)numInterval.Maximum);
        numDelay.Value = Math.Clamp(settings.RestoreDelaySeconds, (int)numDelay.Minimum, (int)numDelay.Maximum);
        chkWatch.Checked = settings.AutoWatch;
        chkStartup.Checked = Startup.IsEnabled;
        _loading = false;

        _watcher.Log += AppendLog;
        _watcher.StateChanged += RefreshView;

        if (settings.AutoWatch) _watcher.Start();
        RefreshView();
    }

    // ---- イベントハンドラ ----

    void btnSave_Click(object? sender, EventArgs e) => _watcher.Save();
    void btnRestore_Click(object? sender, EventArgs e) => _watcher.Restore();
    void btnBaseline_Click(object? sender, EventArgs e) => _watcher.ResetBaseline();

    void mnuExport_Click(object? sender, EventArgs e)
    {
        if (dlgExport.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            SettingsTransfer.Export(dlgExport.FileName, _settings, LayoutManager.Load(_path));
            AppendLog($"設定をエクスポートしました: {dlgExport.FileName}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "エクスポートに失敗しました", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    void mnuImport_Click(object? sender, EventArgs e)
    {
        if (dlgImport.ShowDialog(this) != DialogResult.OK) return;
        ExportData data;
        try { data = SettingsTransfer.Import(dlgImport.FileName); }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "インポートに失敗しました", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // 設定を反映（画面 → 変更イベントで監視と settings.json に反映される）
        _settings.IntervalSeconds = data.Settings.IntervalSeconds;
        _settings.RestoreDelaySeconds = data.Settings.RestoreDelaySeconds;
        _settings.AutoWatch = data.Settings.AutoWatch;
        _loading = true;
        numInterval.Value = Math.Clamp(_settings.IntervalSeconds, (int)numInterval.Minimum, (int)numInterval.Maximum);
        numDelay.Value = Math.Clamp(_settings.RestoreDelaySeconds, (int)numDelay.Minimum, (int)numDelay.Maximum);
        chkWatch.Checked = _settings.AutoWatch;
        _loading = false;
        _settings.IntervalSeconds = (int)numInterval.Value;
        _settings.RestoreDelaySeconds = (int)numDelay.Value;
        _watcher.SetInterval(_settings.IntervalSeconds);
        _watcher.SetRestoreDelay(_settings.RestoreDelaySeconds);
        _settings.Save();
        AppendLog($"設定をインポートしました: {dlgImport.FileName}");

        // 配置は自動保存で上書きされるため、取り込む場合はその場で復元する
        if (data.Layout is { Windows.Count: > 0 } layout &&
            MessageBox.Show(this,
                $"ファイルには {layout.Windows.Count} 個のウインドウ配置（{layout.Saved:yyyy/MM/dd HH:mm} 保存）が含まれています。\n今すぐこの配置に復元しますか？",
                "配置の復元", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            LayoutManager.Write(_path, layout);
            _watcher.Restore();
        }

        if (_settings.AutoWatch) _watcher.Start(); else _watcher.Stop();
        RefreshView();
    }

    void chkWatch_CheckedChanged(object? sender, EventArgs e)
    {
        if (_loading) return;
        if (chkWatch.Checked) _watcher.Start(); else _watcher.Stop();
        _settings.AutoWatch = chkWatch.Checked;
        _settings.Save();
    }

    void numInterval_ValueChanged(object? sender, EventArgs e)
    {
        if (_loading) return;
        _settings.IntervalSeconds = (int)numInterval.Value;
        _watcher.SetInterval(_settings.IntervalSeconds);
        _settings.Save();
    }

    void numDelay_ValueChanged(object? sender, EventArgs e)
    {
        if (_loading) return;
        _settings.RestoreDelaySeconds = (int)numDelay.Value;
        _watcher.SetRestoreDelay(_settings.RestoreDelaySeconds);
        _settings.Save();
    }

    void chkStartup_CheckedChanged(object? sender, EventArgs e)
    {
        if (_loading) return;
        try
        {
            Startup.IsEnabled = chkStartup.Checked;
            AppendLog(chkStartup.Checked ? "ログオン時の自動起動を有効にしました" : "ログオン時の自動起動を無効にしました");
        }
        catch (Exception ex) { AppendLog("自動起動の設定に失敗しました: " + ex.Message); }
    }

    void mnuShow_Click(object? sender, EventArgs e) => ShowMain();

    void mnuExit_Click(object? sender, EventArgs e)
    {
        _exiting = true;
        Close();
    }

    // ---- フォーム制御 ----

    protected override void SetVisibleCore(bool value)
    {
        // --minimized 起動時は最初の表示を抑止してトレイのみにする
        if (_startHidden && !IsHandleCreated)
        {
            CreateHandle();
            value = false;
        }
        base.SetVisibleCore(value);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // × ボタンはトレイへ格納。終了はトレイメニューから
        if (!_exiting && e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Hide();
            trayIcon.ShowBalloonTip(2000, "MonitorRecall", "タスクトレイで動作を続けます", ToolTipIcon.Info);
            return;
        }
        _watcher.Dispose();
        trayIcon.Visible = false;
        base.OnFormClosing(e);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == Native.WM_SHOWME) ShowMain();
        base.WndProc(ref m);
    }

    void ShowMain()
    {
        Show();
        if (WindowState == FormWindowState.Minimized) WindowState = FormWindowState.Normal;
        Activate();
        RefreshView();
    }

    void RefreshView()
    {
        if (InvokeRequired) { BeginInvoke(RefreshView); return; }

        var current = LayoutManager.MonitorSignature();
        bool normal = current == _watcher.BaselineSignature;

        if (!_watcher.Enabled) { lblWatch.Text = "停止中"; lblWatch.ForeColor = SystemColors.GrayText; }
        else if (_watcher.RestorePending) { lblWatch.Text = "復元待ち"; lblWatch.ForeColor = Color.DarkOrange; }
        else if (normal) { lblWatch.Text = "監視中（自動保存しています）"; lblWatch.ForeColor = Color.ForestGreen; }
        else { lblWatch.Text = "保存停止中（モニター構成が基準と異なります）"; lblWatch.ForeColor = Color.Firebrick; }

        lblMonitor.Text = $"現在 {LayoutWatcher.MonitorCount(current)} 台 / 基準 {LayoutWatcher.MonitorCount(_watcher.BaselineSignature)} 台";

        var data = LayoutManager.Load(_path);
        lblSaved.Text = data == null ? "なし" : $"{data.Saved:yyyy/MM/dd HH:mm:ss}（{data.Windows.Count} 個）";
        var tip = $"MonitorRecall - {lblWatch.Text}";
        trayIcon.Text = tip.Length > 63 ? tip[..63] : tip;   // NotifyIcon.Text は 63 文字まで

        var screens = Screen.AllScreens;
        lvWindows.BeginUpdate();
        lvWindows.Items.Clear();
        foreach (var w in data?.Windows ?? [])
        {
            var rect = Rectangle.FromLTRB(w.Left, w.Top, w.Right, w.Bottom);
            int idx = Array.IndexOf(screens, Screen.FromRectangle(rect)) + 1;
            var state = w.ShowCmd switch { 2 => "最小化", 3 => "最大化", _ => "通常" };
            lvWindows.Items.Add(new ListViewItem([
                w.Process, w.Title, state, $"{w.Left}, {w.Top}", $"{rect.Width} x {rect.Height}", $"#{idx}",
            ], IconKey(w)));
        }
        lvWindows.EndUpdate();
    }

    /// <summary>埋め込みリソースの app.ico を読み込む（size が空なら全サイズ入り）</summary>
    static Icon LoadAppIcon(Size size)
    {
        using var s = typeof(MainForm).Assembly.GetManifestResourceStream("MonitorRecall.app.ico");
        if (s == null) return SystemIcons.Application;
        return size.IsEmpty ? new Icon(s) : new Icon(s, size);
    }

    /// <summary>実行ファイルのアイコンを ImageList に登録し、そのキーを返す</summary>
    string IconKey(WindowInfo w)
    {
        var exe = w.ExePath;
        if (string.IsNullOrEmpty(exe))   // 旧形式の保存データ: 起動中の同名プロセスから探す
        {
            var pr = System.Diagnostics.Process.GetProcessesByName(w.Process).FirstOrDefault();
            if (pr != null) using (pr) exe = Native.ProcessPath((uint)pr.Id);
        }
        var key = string.IsNullOrEmpty(exe) ? "*" : exe.ToLowerInvariant();
        if (imgIcons.Images.ContainsKey(key)) return key;

        Icon? icon = null;
        try { if (File.Exists(exe)) icon = Icon.ExtractAssociatedIcon(exe); } catch { }
        icon ??= SystemIcons.Application;
        using (var sized = new Icon(icon, imgIcons.ImageSize))
            imgIcons.Images.Add(key, sized.ToBitmap());
        return key;
    }

    void AppendLog(string msg)
    {
        if (InvokeRequired) { BeginInvoke(() => AppendLog(msg)); return; }
        txtLog.AppendText($"{DateTime.Now:HH:mm:ss}  {msg}{Environment.NewLine}");
    }
}
