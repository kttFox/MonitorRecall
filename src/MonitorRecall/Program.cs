namespace MonitorRecall;

/// <summary>
/// MonitorRecall.exe                 メイン画面を表示（タスクトレイ常駐、自動保存・自動復元）
/// MonitorRecall.exe --minimized     タスクトレイのみで起動
/// MonitorRecall.exe save            現在の配置を保存
/// MonitorRecall.exe restore         保存した配置を復元
/// </summary>
internal static class Program
{
    [STAThread]
    static int Main(string[] args)
    {
        string command = "gui";
        string path = LayoutManager.DefaultPath;   // 保存先は固定
        bool minimized = false;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--minimized": minimized = true; break;
                default: command = args[i].ToLowerInvariant(); break;
            }
        }

        ApplicationConfiguration.Initialize();   // PerMonitorV2 DPI もここで設定される

        switch (command)
        {
            case "save":
                Native.AttachConsole(-1);
                var data = LayoutManager.Save(path);
                Console.WriteLine($"{data.Windows.Count} 個のウインドウを保存しました -> {path}");
                return 0;

            case "restore":
                Native.AttachConsole(-1);
                var (restored, total) = LayoutManager.Restore(path);
                Console.WriteLine(total == 0 ? $"保存データがありません: {path}" : $"{restored} / {total} 個のウインドウを復元しました");
                return 0;

            case "gui":
            case "watch":
                using (var mutex = new Mutex(true, @"Local\MonitorRecall.Watch", out bool created))
                {
                    if (!created)
                    {
                        // 既に起動中なら、そちらの画面を前面に出して終了
                        Native.PostMessage(Native.HWND_BROADCAST, Native.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
                        return 1;
                    }
                    Application.Run(new MainForm(path, AppSettings.Load(), minimized || command == "watch"));
                }
                return 0;

            default:
                Native.AttachConsole(-1);
                Console.WriteLine("usage: MonitorRecall [save|restore] [--minimized]");
                return 2;
        }
    }
}
