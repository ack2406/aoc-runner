using System.Text.Json;
using System.Text.Json.Serialization;

namespace AocRunner.Models;

public class SolutionMetadata
{
    public int Year { get; set; }
    public int Day { get; set; }
    public string Language { get; set; } = "";
    [JsonIgnore]
    public string Path { get; set; } = "";
    public string? EntryPoint { get; set; }

    public static SolutionMetadata? Load(string solutionsPath, int year, int day)
    {
        var root = System.IO.Path.GetFullPath(solutionsPath);
        var metaPath = System.IO.Path.Combine(
                root,
                year.ToString(),
                $"day{day:D2}",
                "meta.json"
                );

        if (!File.Exists(metaPath))
            return null;

        var json = File.ReadAllText(metaPath);
        var metadata = JsonSerializer.Deserialize<SolutionMetadata>(json);

        if (metadata != null)
        {
            metadata.Path = System.IO.Path.Combine(
                    root,
                    year.ToString(),
                    $"day{day:D2}"
                    );
        }

        return metadata;
    }
}
