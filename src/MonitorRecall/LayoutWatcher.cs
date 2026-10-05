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
    }

    public void SetInterval(int sec) => _saveTimer.Interval = Math.Max(1, sec) * 1000;
    public void SetRestoreDelay(int sec) => _restoreTimer.Interval = Math.Max(1, sec * 1000);

    public void Start()
    {
        if (Enabled) return;
        if (LayoutManager.MonitorSignature() == BaselineSignature) Save(quiet: true);
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
        Save();
    }

    public LayoutData Save(bool quiet = false)
    {
        var data = LayoutManager.Save(_path);
        if (!quiet) Log?.Invoke($"{data.Windows.Count} 個のウインドウを保存しました");
        StateChanged?.Invoke();
        return data;
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
        if (Enabled) Check();
        else StateChanged?.Invoke();
    }

    void Check()
    {
        var sig = LayoutManager.MonitorSignature();
        if (sig != _lastSignature)
        {
            _lastSignature = sig;
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
            Save(quiet: true);
    }

    public static int MonitorCount(string signature) =>
        signature.Length == 0 ? 0 : signature.Split('|').Length;

    public void Dispose()
    {
        SystemEvents.DisplaySettingsChanged -= OnDisplayChanged;
        _saveTimer.Dispose();
        _restoreTimer.Dispose();
    }
}
