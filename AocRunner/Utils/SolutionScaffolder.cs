using System.Text.Json;
using AocRunner.Models;

namespace AocRunner.Utils;

public class SolutionScaffolder
{
    private readonly string _solutionsRoot;
    private readonly TemplateRenderer _templates;

    public SolutionScaffolder(string solutionsRoot)
    {
        _solutionsRoot = Path.GetFullPath(solutionsRoot);
        var templatesRoot = Path.Combine(AppContext.BaseDirectory, "Templates");
        _templates = new TemplateRenderer(templatesRoot);
    }

    public async Task<ScaffoldResult> CreateAsync(int year, int day, string language)
    {
        var dayDir = Path.Combine(_solutionsRoot, year.ToString(), $"day{day:D2}");
        var hasExistingFiles = Directory.Exists(dayDir) && Directory.EnumerateFileSystemEntries(dayDir).Any();

        if (hasExistingFiles)
        {
            return new ScaffoldResult(false, $"Day {day:D2} for {year} already exists at {dayDir}. Aborting to avoid overwriting.");
        }

        Directory.CreateDirectory(dayDir);

        var languageKey = language.ToLowerInvariant();
        var entryPoint = GetEntryPoint(languageKey);
        var solutionPath = Path.Combine(dayDir, entryPoint);

        if (!File.Exists(solutionPath))
        {
            var template = await _templates.RenderAsync(
                languageKey,
                "solution",
                new Dictionary<string, string>
                {
                    ["year"] = year.ToString(),
                    ["day"] = day.ToString("D2")
                });
            await File.WriteAllTextAsync(solutionPath, template);
        }

        var metadata = new SolutionMetadata
        {
            Year = year,
            Day = day,
            Language = languageKey,
            EntryPoint = entryPoint
        };
        var metaJson = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(Path.Combine(dayDir, "meta.json"), metaJson);

        var samplePath = Path.Combine(dayDir, "sample.txt");
        if (!File.Exists(samplePath))
        {
            await File.WriteAllTextAsync(samplePath, "");
        }

        return new ScaffoldResult(true, $"Created day {day:D2} for {year} ({languageKey}) in {dayDir}");
    }

    private static string GetEntryPoint(string language) => language switch
    {
        "python" => "solution.py",
        _ => throw new NotSupportedException($"Language {language} not supported yet.")
    };
}

public record ScaffoldResult(bool Success, string Message);
