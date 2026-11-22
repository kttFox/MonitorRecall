using Microsoft.Win32;
using Timer = System.Windows.Forms.Timer;

namespace MonitorRecall;

/// <summary>
/// 正常なモニター構成の間は定期保存し、構成が崩れたら保存を停止、
/// 元の構成に戻ったら自動復元する。UI スレッド上で動作する。
/// </summary>
internal sealed class LayoutWatcher : IDisposable
{
    readonly string _path;
    readonly Timer _saveTimer = new();
    readonly Timer _restoreTimer = new();
    string _lastSignature;
    readonly Native.WinEventProc _showProc;   // GC されないよう保持
    readonly IntPtr _showHook;
    readonly Dictionary<IntPtr, DateTime> _shown = new();
    readonly Timer _shownTimer = new() { Interval = 500 };

    public string BaselineSignature { get; private set; }
    public bool Enabled => _saveTimer.Enabled;
    public bool RestorePending => _restoreTimer.Enabled;

    public event Action<string>? Log;
    public event Action? StateChanged;

    public LayoutWatcher(string path, int intervalSec, int restoreDelaySec)
    {
        _path = path;
        BaselineSignature = _lastSignature = LayoutManager.MonitorSignature();
        SetInterval(intervalSec);
        SetRestoreDelay(restoreDelaySec);
        _saveTimer.Tick += (_, _) => Check();
        _restoreTimer.Tick += (_, _) =>
        {
            _restoreTimer.Stop();
            if (LayoutManager.MonitorSignature() == BaselineSignature) Restore();
            StateChanged?.Invoke();
        };
        SystemEvents.DisplaySettingsChanged += OnDisplayChanged;

        // タスクトレイなどから再表示されたウインドウを保存位置へ戻す
        _showProc = OnWindowShown;
        _showHook = Native.SetWinEventHook(Native.EVENT_OBJECT_SHOW, Native.EVENT_OBJECT_SHOW,
            IntPtr.Zero, _showProc, 0, 0, Native.WINEVENT_OUTOFCONTEXT);
        _shownTimer.Tick += (_, _) => RestoreShown();
    }

    void OnWindowShown(IntPtr hook, uint ev, IntPtr hwnd, int idObject, int idChild, uint thread, uint time)
    {
        if (idObject != 0 /*OBJID_WINDOW*/ || hwnd == IntPtr.Zero || !Enabled) return;
        if (Native.GetAncestor(hwnd, 2 /*GA_ROOT*/) != hwnd) return;   // 子ウインドウは対象外
        // アプリ側の初期配置が終わるのを待ってから処理する
        _shown[hwnd] = DateTime.Now;
        _shownTimer.Start();
    }

    void RestoreShown()
    {
        var ready = _shown.Where(kv => DateTime.Now - kv.Value >= TimeSpan.FromMilliseconds(400)).Select(kv => kv.Key).ToList();
        foreach (var h in ready) _shown.Remove(h);
        if (_shown.Count == 0) _shownTimer.Stop();
        if (ready.Count == 0 || _restoreTimer.Enabled || LayoutManager.MonitorSignature() != BaselineSignature) return;

        foreach (var h in ready)
        {
            if (!LayoutManager.RestoreOne(_path, h)) continue;
            Log?.Invoke($"再表示されたウインドウを復元しました: {Native.Title(h)}");
        }
    }

    public void SetInterval(int sec) => _saveTimer.Interval = Math.Max(1, sec) * 1000;
    public void SetRestoreDelay(int sec) => _restoreTimer.Interval = Math.Max(1, sec * 1000);

    public void Start()
    {
        if (Enabled) return;
        if (LayoutManager.MonitorSignature() == BaselineSignature) _ = Save(quiet: true);
        _saveTimer.Start();
        Log?.Invoke("自動監視を開始しました");
        StateChanged?.Invoke();
    }

    public void Stop()
    {
        if (!Enabled) return;
        _saveTimer.Stop();
        _restoreTimer.Stop();
        Log?.Invoke("自動監視を停止しました");
        StateChanged?.Invoke();
    }

    /// <summary>現在のモニター構成を正常（基準）とする</summary>
    public void ResetBaseline()
    {
        BaselineSignature = _lastSignature = LayoutManager.MonitorSignature();
        _restoreTimer.Stop();
        Log?.Invoke($"基準のモニター構成を更新しました（{MonitorCount(BaselineSignature)} 台）");
        _ = Save();
    }

    bool _saving;
    int _generation;   // モニター構成の変化ごとに増やす（保存中の変化検出用）

    /// <summary>
    /// ウインドウ情報の収集（UI Automation によるタスクバー走査など）は時間がかかり、
    /// 応答しないアプリがあると待たされるため、UI スレッドを止めないようスレッドプールで行う
    /// </summary>
    public async Task Save(bool quiet = false)
    {
        if (_saving) return;   // 前回の保存がまだ終わっていなければ今回は見送る
        _saving = true;
        try
        {
            var gen = Volatile.Read(ref _generation);
            var baseline = BaselineSignature;
            var data = await Task.Run(() => LayoutManager.Save(_path, () =>
                Volatile.Read(ref _generation) == gen && LayoutManager.MonitorSignature() == baseline));
            if (data == null) Log?.Invoke("保存中にモニター構成が変化したため、保存を破棄しました");
            else if (!quiet) Log?.Invoke($"{data.Windows.Count} 個のウインドウを保存しました");
        }
        catch (Exception ex)
        {
            Log?.Invoke($"保存に失敗しました: {ex.Message}");
        }
        finally
        {
            _saving = false;
        }
        StateChanged?.Invoke();
    }

    public (int restored, int total) Restore()
    {
        var result = LayoutManager.Restore(_path);
        Log?.Invoke(result.total == 0
            ? "保存データがありません"
            : $"{result.restored} / {result.total} 個のウインドウを復元しました");
        return result;
    }

    void OnDisplayChanged(object? sender, EventArgs e)
    {
        Interlocked.Increment(ref _generation);
        if (Enabled) Check();
        else StateChanged?.Invoke();
    }

    void Check()
    {
        var sig = LayoutManager.MonitorSignature();
        if (sig != _lastSignature)
        {
            _lastSignature = sig;
            Interlocked.Increment(ref _generation);
            _restoreTimer.Stop();
            if (sig == BaselineSignature)
            {
                Log?.Invoke($"モニター構成が基準に戻りました。{_restoreTimer.Interval / 1000} 秒後に復元します");
                _restoreTimer.Start();
            }
            else
            {
                Log?.Invoke($"モニター構成が変化しました（{MonitorCount(sig)} 台）。保存を停止します");
            }
            StateChanged?.Invoke();
            return;
        }
        // 正常構成かつ復元待ちでない間だけ保存（集約後の配置で上書きしない）
        if (sig == BaselineSignature && !_restoreTimer.Enabled)
            _ = Save(quiet: true);
    }

    public static int MonitorCount(string signature) =>
        signature.Length == 0 ? 0 : signature.Split('|').Length;

    public void Dispose()
    {
        SystemEvents.DisplaySettingsChanged -= OnDisplayChanged;
        if (_showHook != IntPtr.Zero) Native.UnhookWinEvent(_showHook);
        _shownTimer.Dispose();
        _saveTimer.Dispose();
        _restoreTimer.Dispose();
    }
}
