using System.Text.Json;

namespace AocRunner.Utils;

public class Config
{
    public string SolutionPath { get; set; } = "../aoc-solutions";
    public string? SessionToken { get; set; }
    public string CacheDir { get; set; } = ".cache";
}

public static class ConfigManager
{
    public static Config Load()
    {
        var configPath = ResolveConfigPath();
        if (!File.Exists(configPath))
        {
            var defaultConfig = new Config();
            Directory.CreateDirectory(Path.GetDirectoryName(configPath)!);
            File.WriteAllText(configPath, JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true }));
            return defaultConfig;
        }

        var json = File.ReadAllText(configPath);
        return JsonSerializer.Deserialize<Config>(json) ?? new Config();
    }

    private static string ResolveConfigPath()
    {
        var baseDir = AppContext.BaseDirectory;

        // When running via `dotnet run --project AocRunner`, bin/../../.. points to AocRunner/.
        // Keep config in the parent (repo root) so relative paths like ../aoc-solutions stay correct.
        var projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
        if (File.Exists(Path.Combine(projectRoot, "AocRunner.csproj")))
        {
            var repoRoot = Directory.GetParent(projectRoot)?.FullName ?? projectRoot;
            return Path.Combine(repoRoot, "config.json");
        }

        // Fallback to alongside the executable (publish scenarios)
        return Path.Combine(baseDir, "config.json");
    }
}
