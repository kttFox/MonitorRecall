using System.Runtime.InteropServices;
using System.Windows.Automation;

namespace MonitorRecall;

/// <summary>タスクバーボタンの並び順の取得と並べ替え</summary>
internal static class Taskbar
{
    /// <summary>全タスクバー（メイン＋セカンダリ）のボタン名を左から順に返す。取得できなければ空</summary>
    public static List<string> ButtonNames()
    {
        var names = new List<string>();
        try
        {
            var trays = AutomationElement.RootElement.FindAll(TreeScope.Children, new OrCondition(
                new PropertyCondition(AutomationElement.ClassNameProperty, "Shell_TrayWnd"),
                new PropertyCondition(AutomationElement.ClassNameProperty, "Shell_SecondaryTrayWnd")));
            foreach (AutomationElement tray in trays)
            {
                // Windows 10: MSTaskListWClass の子ボタン
                var list = tray.FindFirst(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ClassNameProperty, "MSTaskListWClass"));
                var buttons = list != null
                    ? list.FindAll(TreeScope.Children, Condition.TrueCondition)
                    // Windows 11: XAML のタスクバーボタン
                    : tray.FindAll(TreeScope.Descendants,
                        new PropertyCondition(AutomationElement.ClassNameProperty, "Taskbar.TaskListButtonAutomationPeer"));
                foreach (AutomationElement b in buttons) names.Add(b.Current.Name);
            }
        }
        catch { names.Clear(); }
        return names;
    }

    /// <summary>ボタン名とウインドウタイトルを突き合わせ、ウインドウごとの並び順（不明は -1）を返す</summary>
    public static int[] Order(IReadOnlyList<string> titles)
    {
        var order = Enumerable.Repeat(-1, titles.Count).ToArray();
        var names = ButtonNames();
        var used = new bool[names.Count];
        // ボタン名は「タイトル - 1 個の実行中ウィンドウ」のように後ろに付くことがある。
        // 「MonitorRecall」が「MonitorRecall - MainForm.cs …」を横取りしないよう、
        // 長いタイトルから順に、候補のうち最も短いボタン名と対応させる
        foreach (int i in Enumerable.Range(0, titles.Count).OrderByDescending(i => titles[i].Length))
        {
            int best = -1;
            for (int j = 0; j < names.Count; j++)
            {
                if (used[j]) continue;
                if (names[j] != titles[i] && !names[j].StartsWith(titles[i] + " - ", StringComparison.Ordinal)) continue;
                if (best < 0 || names[j].Length < names[best].Length) best = j;
            }
            if (best >= 0) { used[best] = true; order[i] = best; }
        }
        return order;
    }

    /// <summary>指定順にタスクバーへ登録し直す（登録し直したボタンは末尾に並ぶ）</summary>
    public static void Reorder(IList<IntPtr> handles)
    {
        try
        {
            var tb = (ITaskbarList)new CTaskbarList();
            tb.HrInit();
            // 先に全部外してから順に追加する。Explorer は非同期に処理するため、間隔を空ける
            foreach (var h in handles) tb.DeleteTab(h);
            Thread.Sleep(300);
            foreach (var h in handles)
            {
                tb.AddTab(h);
                Thread.Sleep(50);
            }
            Marshal.ReleaseComObject(tb);
        }
        catch { }
    }

    [ComImport, Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
    class CTaskbarList { }

    [ComImport, Guid("56FDF342-FD6D-11d0-958A-006097C9A090"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ITaskbarList
    {
        void HrInit();
        void AddTab(IntPtr hwnd);
        void DeleteTab(IntPtr hwnd);
        void ActivateTab(IntPtr hwnd);
        void SetActiveAlt(IntPtr hwnd);
    }
}
