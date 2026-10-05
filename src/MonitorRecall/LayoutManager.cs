using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using static MonitorRecall.Native;

namespace MonitorRecall;

public record WindowInfo(long Handle, string Process, string Class, string Title,
    int ShowCmd, int Left, int Top, int Right, int Bottom, int Flags = 0, string ExePath = "");

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

    public static List<WindowInfo> Capture()
    {
        var handles = new List<IntPtr>();
        EnumWindows((h, _) =>
        {
            if (!IsWindowVisible(h)) return true;
            if (GetWindow(h, 4 /*GW_OWNER*/) != IntPtr.Zero) return true;          // ダイアログ等
            if ((GetWindowLong(h, -20 /*GWL_EXSTYLE*/) & 0x80 /*WS_EX_TOOLWINDOW*/) != 0) return true;
            DwmGetWindowAttribute(h, 14 /*DWMWA_CLOAKED*/, out int cloaked, sizeof(int));
            if (cloaked != 0) return true;                                          // 別仮想デスクトップ等
            if (Title(h).Length == 0) return true;
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
        return result;
    }

    public static LayoutData Save(string path)
    {
        var data = new LayoutData(DateTime.Now, MonitorSignature(), Capture());
        Write(path, data);
        return data;
    }

    public static void Write(string path, LayoutData data)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var tmp = path + ".tmp";
        File.WriteAllText(tmp, JsonSerializer.Serialize(data, JsonOptions));
        File.Move(tmp, path, overwrite: true);
    }

    public static LayoutData? Load(string path) =>
        File.Exists(path) ? JsonSerializer.Deserialize<LayoutData>(File.ReadAllText(path)) : null;

    /// <returns>(復元数, 保存数)</returns>
    public static (int restored, int total) Restore(string path)
    {
        var data = Load(path);
        if (data == null) return (0, 0);

        var current = Capture();
        var used = new HashSet<long>();
        int count = 0;

        foreach (var w in data.Windows)
        {
            // 1) 同じハンドル  2) プロセス+クラス+タイトル  3) プロセス+クラス
            var target =
                current.FirstOrDefault(c => !used.Contains(c.Handle) && c.Handle == w.Handle && c.Process == w.Process)
                ?? current.FirstOrDefault(c => !used.Contains(c.Handle) && c.Process == w.Process && c.Class == w.Class && c.Title == w.Title)
                ?? current.FirstOrDefault(c => !used.Contains(c.Handle) && c.Process == w.Process && c.Class == w.Class);
            if (target == null) continue;
            used.Add(target.Handle);

            var h = new IntPtr(target.Handle);
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
        return (count, data.Windows.Count);
    }
}
