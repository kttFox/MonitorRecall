using System.Text.Json;

namespace MonitorRecall;

/// <summary>エクスポートファイルの形式（設定 + 保存済みのウインドウ配置）</summary>
public record ExportData(string App, int Version, DateTime Exported, AppSettings Settings, LayoutData? Layout);

public static class SettingsTransfer
{
    const string AppName = "MonitorRecall";
    const int CurrentVersion = 1;

    static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static void Export(string file, AppSettings settings, LayoutData? layout)
    {
        var data = new ExportData(AppName, CurrentVersion, DateTime.Now, settings, layout);
        File.WriteAllText(file, JsonSerializer.Serialize(data, JsonOptions));
    }

    /// <exception cref="InvalidDataException">MonitorRecall のエクスポートファイルでない場合</exception>
    public static ExportData Import(string file)
    {
        ExportData? data;
        try { data = JsonSerializer.Deserialize<ExportData>(File.ReadAllText(file)); }
        catch (JsonException ex) { throw new InvalidDataException("ファイルの形式が正しくありません。", ex); }

        if (data == null || data.App != AppName || data.Settings == null)
            throw new InvalidDataException("MonitorRecall のエクスポートファイルではありません。");
        if (data.Version > CurrentVersion)
            throw new InvalidDataException("新しいバージョンの MonitorRecall で作成されたファイルです。");
        return data;
    }
}
