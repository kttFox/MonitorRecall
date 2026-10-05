using System.Text.Json;

namespace MonitorRecall;

public class AppSettings
{
    public int IntervalSeconds { get; set; } = 10;
    public int RestoreDelaySeconds { get; set; } = 5;
    public bool AutoWatch { get; set; } = true;

    static readonly string FilePath =
        Path.Combine(Path.GetDirectoryName(LayoutManager.DefaultPath)!, "settings.json");

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(FilePath))
                return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath)) ?? new();
        }
        catch { }
        return new();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }
}
