using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using static MonitorRecall.Native;

namespace MonitorRecall;

public record WindowInfo(long Handle, string Process, string Class, string Title,
    int ShowCmd, int Left, int Top, int Right, int Bottom, int Flags = 0, string ExePath = "", int TaskbarOrder = -1);

public record LayoutData(DateTime Saved, string Monitors, List<WindowInfo> Windows);

public static class LayoutManager
{
    public static readonly string DefaultPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MonitorRecall", "layout.json");

    static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    /// <summary>モニター構成を表す文字列（各モニター矩形をソートして連結）</summary>
    public static string MonitorSignature()
    {
        var parts = new List<string>();
        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr m, IntPtr dc, ref RECT r, IntPtr l) =>
        {
            parts.Add($"{r.Left},{r.Top},{r.Right},{r.Bottom}");
            return true;
        }, IntPtr.Zero);
        parts.Sort(StringComparer.Ordinal);
        return string.Join("|", parts);
    }

    public static List<WindowInfo> Capture(bool taskbarOrder = true)
    {
        var handles = new List<IntPtr>();
        EnumWindows((h, _) =>
        {
            // 非表示でも「最小化かつタイトルバーあり」はタスクトレイ格納中（Thunderbird など）とみなして対象にする
            if (!IsWindowVisible(h) && !IsInTray(h)) return true;
            if (GetWindow(h, 4 /*GW_OWNER*/) != IntPtr.Zero) return true;          // ダイアログ等
            if ((GetWindowLong(h, -20 /*GWL_EXSTYLE*/) & 0x80 /*WS_EX_TOOLWINDOW*/) != 0) return true;
            DwmGetWindowAttribute(h, 14 /*DWMWA_CLOAKED*/, out int cloaked, sizeof(int));
            if (cloaked != 0) return true;                                          // 別仮想デスクトップ等
            if (Title(h).Length == 0) return true;
            if (ClassName(h) == "OperationStatusWindow") return true;               // エクスプローラーのコピー進捗ダイアログ
            handles.Add(h);
            return true;
        }, IntPtr.Zero);

        var result = new List<WindowInfo>();
        foreach (var h in handles)
        {
            var p = new WINDOWPLACEMENT { length = Marshal.SizeOf<WINDOWPLACEMENT>() };
            if (!GetWindowPlacement(h, ref p)) continue;
            GetWindowThreadProcessId(h, out uint pid);
            string proc;
            try { using var pr = System.Diagnostics.Process.GetProcessById((int)pid); proc = pr.ProcessName; }
            catch { proc = ""; }
            result.Add(new WindowInfo(h.ToInt64(), proc, ClassName(h), Title(h), p.showCmd,
                p.rcNormal.Left, p.rcNormal.Top, p.rcNormal.Right, p.rcNormal.Bottom, p.flags, ProcessPath(pid)));
        }

        if (!taskbarOrder) return result;
        var order = Taskbar.Order(result.Select(w => w.Title).ToList());
        return result.Select((w, i) => w with { TaskbarOrder = order[i] }).ToList();
    }

    static readonly object FileLock = new();

    /// <param name="canCommit">書き込み直前に評価し、false なら保存を破棄する（収集中にモニター構成が変わった場合など）</param>
    public static LayoutData? Save(string path, Func<bool>? canCommit = null)
    {
        lock (FileLock) return SaveCore(path, canCommit);
    }

    static LayoutData? SaveCore(string path, Func<bool>? canCommit)
    {
        var signature = MonitorSignature();
        var windows = Capture();
        // タスクトレイに格納中（非表示）のウインドウは列挙されないため、前回の保存内容を引き継ぐ。
        // 引き継がないと、トレイから再表示したときに復元対象から消えている
        LayoutData? prev = null;
        try { prev = Load(path); } catch { }
        if (prev != null)
        {
            // ハンドルが残っている非表示ウインドウに加え、トレイ格納時にウインドウを破棄するアプリのため
            // 「プロセスは動いているが表示中のウインドウが 1 つもない」ものも引き継ぐ
            var running = System.Diagnostics.Process.GetProcesses().Select(pr => { using (pr) return pr.ProcessName; }).ToHashSet();
            var visibleProcs = windows.Select(c => c.Process).ToHashSet();
            windows.AddRange(prev.Windows.Where(w => !windows.Any(c => c.Handle == w.Handle) &&
                (IsHiddenAlive(w) || (running.Contains(w.Process) && !visibleProcs.Contains(w.Process)))));
        }

        // 収集には時間がかかるため、その間にモニター構成が変わってウインドウが集約されていたら
        // 崩れた配置で上書きしないよう破棄する
        if (MonitorSignature() != signature || (canCommit != null && !canCommit())) return null;

        var data = new LayoutData(DateTime.Now, signature, windows);
        WriteCore(path, data);
        return data;
    }

    public static void Write(string path, LayoutData data)
    {
        lock (FileLock) WriteCore(path, data);
    }

    static void WriteCore(string path, LayoutData data)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var tmp = path + ".tmp";
        File.WriteAllText(tmp, JsonSerializer.Serialize(data, JsonOptions));
        File.Move(tmp, path, overwrite: true);
    }

    public static LayoutData? Load(string path)
    {
        string json;
        lock (FileLock)
        {
            if (!File.Exists(path)) return null;
            json = File.ReadAllText(path);
        }
        return JsonSerializer.Deserialize<LayoutData>(json);
    }

    /// <summary>タスクトレイに格納中のウインドウか（非表示・最小化・タイトルバーあり）</summary>
    static bool IsInTray(IntPtr h) =>
        !IsWindowVisible(h) && IsIconic(h) && (GetWindowLong(h, -16 /*GWL_STYLE*/) & 0xC00000 /*WS_CAPTION*/) == 0xC00000;

    /// <summary>保存時のウインドウが今も存在し、非表示（タスクトレイ格納中など）か</summary>
    static bool IsHiddenAlive(WindowInfo w)
    {
        var h = new IntPtr(w.Handle);
        if (!IsWindow(h) || IsWindowVisible(h)) return false;
        GetWindowThreadProcessId(h, out uint pid);
        // ハンドルが別プロセスに再利用されていないか確認
        if (w.ExePath.Length > 0) return string.Equals(ProcessPath(pid), w.ExePath, StringComparison.OrdinalIgnoreCase);
        try { using var pr = Process.GetProcessById((int)pid); return pr.ProcessName == w.Process; }
        catch { return false; }
    }

    /// <summary>保存データと現在のウインドウを対応付ける（保存エントリ → 対象ウインドウ）</summary>
    static List<(WindowInfo saved, WindowInfo target)> Match(LayoutData data, List<WindowInfo> current)
    {
        var used = new HashSet<long>();
        var result = new List<(WindowInfo, WindowInfo)>();
        foreach (var w in data.Windows)
        {
            // 1) 同じハンドル  2) プロセス+クラス+タイトル  3) プロセス+クラス
            var target =
                current.FirstOrDefault(c => !used.Contains(c.Handle) && c.Handle == w.Handle && c.Process == w.Process)
                ?? current.FirstOrDefault(c => !used.Contains(c.Handle) && c.Process == w.Process && c.Class == w.Class && c.Title == w.Title)
                ?? current.FirstOrDefault(c => !used.Contains(c.Handle) && c.Process == w.Process && c.Class == w.Class);
            if (target == null) continue;
            used.Add(target.Handle);
            result.Add((w, target));
        }
        return result;
    }

    /// <summary>
    /// 新たに表示されたウインドウ（タスクトレイからの再表示など）を保存位置へ戻す。
    /// 既に表示中のウインドウに対応する保存エントリは使わない
    /// </summary>
    public static bool RestoreOne(string path, IntPtr hwnd)
    {
        LayoutData? data;
        try { data = Load(path); } catch { return false; }
        if (data == null) return false;

        var pair = Match(data, Capture(taskbarOrder: false)).FirstOrDefault(m => m.target.Handle == hwnd.ToInt64());
        if (pair.saved == null) return false;
        var w = pair.saved;
        var cur = pair.target;
        if (cur.Left == w.Left && cur.Top == w.Top && cur.Right == w.Right && cur.Bottom == w.Bottom) return false;

        if (cur.ShowCmd == SW_SHOWMAXIMIZED) ShowWindow(hwnd, SW_RESTORE);
        var p = new WINDOWPLACEMENT
        {
            length = Marshal.SizeOf<WINDOWPLACEMENT>(),
            // ユーザーが開いたところなので最小化はせず、最大化だけ再現する
            showCmd = w.ShowCmd == SW_SHOWMAXIMIZED || (w.Flags & WPF_RESTORETOMAXIMIZED) != 0 ? SW_SHOWMAXIMIZED : SW_SHOWNOACTIVATE,
            rcNormal = new RECT { Left = w.Left, Top = w.Top, Right = w.Right, Bottom = w.Bottom },
        };
        if (p.showCmd == SW_SHOWMAXIMIZED)
        {
            // 通常表示で目的のモニターへ移してから最大化する
            var n = p; n.showCmd = SW_SHOWNOACTIVATE;
            SetWindowPlacement(hwnd, ref n);
        }
        return SetWindowPlacement(hwnd, ref p);
    }

    /// <returns>(復元数, 保存数)</returns>
    public static (int restored, int total) Restore(string path)
    {
        var data = Load(path);
        if (data == null) return (0, 0);

        var current = Capture();
        var used = new HashSet<long>();
        var placed = new List<(IntPtr h, int order, int index)>();
        int count = 0;

        foreach (var w in data.Windows)
        {
            // 1) 同じハンドル  2) プロセス+クラス+タイトル  3) プロセス+クラス
            var target =
                current.FirstOrDefault(c => !used.Contains(c.Handle) && c.Handle == w.Handle && c.Process == w.Process)
                ?? current.FirstOrDefault(c => !used.Contains(c.Handle) && c.Process == w.Process && c.Class == w.Class && c.Title == w.Title)
                ?? current.FirstOrDefault(c => !used.Contains(c.Handle) && c.Process == w.Process && c.Class == w.Class);
            if (target == null)
            {
                // トレイに格納中のウインドウは非表示のまま位置だけ戻し、再表示時に正しい位置へ出す
                if (IsHiddenAlive(w))
                {
                    var hp = new WINDOWPLACEMENT
                    {
                        length = Marshal.SizeOf<WINDOWPLACEMENT>(),
                        showCmd = SW_HIDE,
                        flags = w.Flags & WPF_RESTORETOMAXIMIZED,
                        rcNormal = new RECT { Left = w.Left, Top = w.Top, Right = w.Right, Bottom = w.Bottom },
                    };
                    if (SetWindowPlacement(new IntPtr(w.Handle), ref hp)) count++;
                }
                continue;
            }
            used.Add(target.Handle);

            var h = new IntPtr(target.Handle);
            if (!IsWindowVisible(h))
            {
                // トレイ格納中は非表示のまま位置だけ戻す（表示するとトレイから出てしまう）
                var tp = new WINDOWPLACEMENT
                {
                    length = Marshal.SizeOf<WINDOWPLACEMENT>(),
                    showCmd = SW_HIDE,
                    flags = w.Flags & WPF_RESTORETOMAXIMIZED,
                    rcNormal = new RECT { Left = w.Left, Top = w.Top, Right = w.Right, Bottom = w.Bottom },
                };
                if (SetWindowPlacement(h, ref tp)) count++;
                continue;
            }
            placed.Add((h, w.TaskbarOrder, placed.Count));
            var p = new WINDOWPLACEMENT
            {
                length = Marshal.SizeOf<WINDOWPLACEMENT>(),
                rcNormal = new RECT { Left = w.Left, Top = w.Top, Right = w.Right, Bottom = w.Bottom },
            };

            // 最大化中のウインドウは一度元に戻さないと別モニターへ移らない
            if (target.ShowCmd == SW_SHOWMAXIMIZED) ShowWindow(h, SW_RESTORE);

            // いったん通常表示で目的のモニターへ移す。最小化のまま位置だけ変えると
            // タスクバーボタンが移動せず、アクティブにした時点で初めて移るため。
            p.showCmd = SW_SHOWNOACTIVATE;
            SetWindowPlacement(h, ref p);

            // 保存時の表示状態に戻す（フォーカスは奪わない）
            p.showCmd = w.ShowCmd switch
            {
                SW_SHOWMAXIMIZED => SW_SHOWMAXIMIZED,
                SW_SHOWMINIMIZED or SW_MINIMIZE or SW_SHOWMINNOACTIVE => SW_SHOWMINNOACTIVE,
                _ => SW_SHOWNOACTIVATE,
            };
            p.flags = w.Flags & WPF_RESTORETOMAXIMIZED;   // 最大化から最小化したものは最大化で戻す
            if (SetWindowPlacement(h, ref p)) count++;
        }

        // タスクバーボタンを保存時の並び順で登録し直す（順序不明のものは後ろへ）
        if (placed.Any(x => x.order >= 0))
            Taskbar.Reorder(placed.OrderBy(x => x.order < 0).ThenBy(x => x.order).ThenBy(x => x.index).Select(x => x.h).ToList());

        return (count, data.Windows.Count);
    }
}
