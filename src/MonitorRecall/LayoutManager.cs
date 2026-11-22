using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using static MonitorRecall.Native;

namespace MonitorRecall;

public record WindowInfo(long Handle, string Process, string Class, string Title,
    int ShowCmd, int Left, int Top, int Right, int Bottom, int Flags = 0, string ExePath = "", int TaskbarOrder = -1);

public record LayoutData(DateTime Saved, string Monitors, List<WindowInfo> Windows);

/// <param name="Data">保存した（または変更がなく保存を見送った）配置。収集中にモニター構成が変わり破棄した場合は null</param>
/// <param name="Changed">配置に変更があり、ファイルへ書き込んだか</param>
public record SaveResult(LayoutData? Data, bool Changed);

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

    /// <summary>保存・復元の対象となるトップレベルウインドウか</summary>
    public static bool IsTarget(IntPtr h)
    {
        // 非表示でも「最小化かつタイトルバーあり」はタスクトレイ格納中（Thunderbird など）とみなして対象にする
        if (!IsWindowVisible(h) && !IsInTray(h)) return false;
        if (GetWindow(h, 4 /*GW_OWNER*/) != IntPtr.Zero) return false;          // ダイアログ等
        if ((GetWindowLong(h, -20 /*GWL_EXSTYLE*/) & 0x80 /*WS_EX_TOOLWINDOW*/) != 0) return false;
        DwmGetWindowAttribute(h, 14 /*DWMWA_CLOAKED*/, out int cloaked, sizeof(int));
        if (cloaked != 0) return false;                              // 別仮想デスクトップ等
        if (Title(h).Length == 0) return false;
        GetWindowThreadProcessId(h, out uint pid);
        if (pid == Environment.ProcessId) return false;                     // MonitorRecall 自身
        if (ClassName(h) == "OperationStatusWindow") return false;           // エクスプローラーのコピー進捗ダイアログ
        return true;
    }

    public static List<WindowInfo> Capture(bool taskbarOrder = true)
    {
        var handles = new List<IntPtr>();
        EnumWindows((h, _) =>
        {
            if (IsTarget(h)) handles.Add(h);
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

    static readonly string OwnProcessName = Process.GetCurrentProcess().ProcessName;

    /// <summary>履歴として残す件数</summary>
    public const int HistoryLimit = 100;

    /// <summary>この時間内に続いた変更は 1 件の履歴にまとめる</summary>
    static readonly TimeSpan HistoryMergeWindow = TimeSpan.FromMinutes(5);

    public static string HistoryDir(string path) => Path.Combine(Path.GetDirectoryName(path)!, "history");

    /// <summary>履歴ファイルの一覧（新しい順）</summary>
    public static List<string> HistoryFiles(string path)
    {
        var dir = HistoryDir(path);
        if (!Directory.Exists(dir)) return [];
        return Directory.GetFiles(dir, "*.json").OrderByDescending(f => Path.GetFileName(f), StringComparer.Ordinal).ToList();
    }

    /// <summary>
    /// 配置が同じかどうか。タイトルやハンドルは配置ではないので比較しない
    /// （タイトルはブラウザなどで頻繁に変わり、そのたびに履歴が増えてしまう）
    /// </summary>
    static bool SamePlacement(LayoutData a, LayoutData b)
    {
        static List<string> Keys(LayoutData d) => d.Windows
            .Select(w => $"{w.Process}	{w.Class}	{w.ShowCmd}	{w.Left},{w.Top},{w.Right},{w.Bottom}	{w.Flags & WPF_RESTORETOMAXIMIZED}	{w.TaskbarOrder}")
            .Order(StringComparer.Ordinal).ToList();
        return a.Monitors == b.Monitors && Keys(a).SequenceEqual(Keys(b));
    }

    /// <summary>
    /// 履歴に残すべき配置の変化があるか。両方に存在するウインドウ（ハンドルで対応付け）について、
    /// 位置・サイズ・最大化の有無が変わったか、タスクバー上の相対的な並び順が変わったかを見る。
    /// ウインドウの開閉や最小化・元に戻すだけ、他のウインドウの開閉による並び番号のずれは変化とみなさない
    /// </summary>
    static bool Rearranged(LayoutData prev, LayoutData cur)
    {
        if (prev.Monitors != cur.Monitors) return true;
        static bool Maximized(WindowInfo w) => w.ShowCmd == SW_SHOWMAXIMIZED || (w.Flags & WPF_RESTORETOMAXIMIZED) != 0;

        var before = prev.Windows.GroupBy(w => w.Handle).ToDictionary(g => g.Key, g => g.First());
        var pairs = cur.Windows.Where(w => before.ContainsKey(w.Handle)).Select(w => (old: before[w.Handle], now: w)).ToList();
        if (pairs.Any(p => p.old.Left != p.now.Left || p.old.Top != p.now.Top || p.old.Right != p.now.Right ||
                           p.old.Bottom != p.now.Bottom || Maximized(p.old) != Maximized(p.now)))
            return true;

        // 並び順が両方で取れているものだけで相対順序を比べる
        var ordered = pairs.Where(p => p.old.TaskbarOrder >= 0 && p.now.TaskbarOrder >= 0).ToList();
        return !ordered.OrderBy(p => p.old.TaskbarOrder).Select(p => p.now.Handle)
            .SequenceEqual(ordered.OrderBy(p => p.now.TaskbarOrder).Select(p => p.now.Handle));
    }

    static void AddHistory(string path, LayoutData data)
    {
        var dir = HistoryDir(path);
        Directory.CreateDirectory(dir);
        // 並べ替えの途中で何度も保存されるため、直前の履歴が新しいうちは置き換えて 1 件にまとめる。
        // その前の履歴（並べ替える前の配置）は残る
        var latest = HistoryFiles(path).FirstOrDefault();
        if (latest != null && DateTime.Now - File.GetLastWriteTime(latest) < HistoryMergeWindow)
            try { File.Delete(latest); } catch { }
        WriteCore(Path.Combine(dir, $"{data.Saved:yyyyMMdd-HHmmss-fff}.json"), data);
        foreach (var old in HistoryFiles(path).Skip(HistoryLimit))
            try { File.Delete(old); } catch { }
    }

    /// <param name="canCommit">書き込み直前に評価し、false なら保存を破棄する（収集中にモニター構成が変わった場合など）</param>
    public static SaveResult Save(string path, Func<bool>? canCommit = null)
    {
        // ウインドウ収集（UI Automation など）は数秒かかることがあるためロックの外で行い、
        // ロックは書き込みの判定と書き込みだけに限る。収集中までロックすると UI スレッドの Load が待たされる
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
        if (MonitorSignature() != signature || (canCommit != null && !canCommit())) return new(null, false);

        var data = new LayoutData(DateTime.Now, signature, windows);
        // 配置に変更がなければ書き込まない（履歴も増やさない）
        if (prev != null && SamePlacement(prev, data)) return new(prev, false);
        lock (FileLock)
        {
            WriteCore(path, data);
            // 履歴はウインドウの移動などがあったときだけ残す（開閉・最小化・タスクバーの番号ずれでは増やさない）
            if (prev == null || Rearranged(prev, data))
                try { AddHistory(path, data); } catch { }
        }
        return new(data, true);
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
        return data == null ? (0, 0) : Restore(data);
    }

    /// <param name="reorderTaskbar">タスクバーボタンの並び順も戻すか（時間がかかるのでプレビューでは省く）</param>
    public static (int restored, int total) Restore(LayoutData data, bool reorderTaskbar = true)
    {
        // 自分自身は対象外（古い保存データに含まれていても動かさない）
        data = data with { Windows = data.Windows.Where(w => w.Process != OwnProcessName).ToList() };
        var current = Capture(taskbarOrder: reorderTaskbar);
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
        if (reorderTaskbar && placed.Any(x => x.order >= 0))
            Taskbar.Reorder(placed.OrderBy(x => x.order < 0).ThenBy(x => x.order).ThenBy(x => x.index).Select(x => x.h).ToList());

        return (count, data.Windows.Count);
    }
}
