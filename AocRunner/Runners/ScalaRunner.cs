using System.Diagnostics;
using AocRunner.Models;

namespace AocRunner.Runners;

public class ScalaRunner : ILanguageRunner
{
    public async Task<RunResult> Run(SolutionMetadata metadata, string input, int? part)
    {
        var entryPoint = metadata.EntryPoint ?? "solution.scala";
        var entryPath = Path.Combine(metadata.Path, entryPoint);

        if (!File.Exists(entryPath))
        {
            return new RunResult { Success = false, Error = $"Entry point not found: {entryPath}" };
        }

        var executable = FindExecutable(new[] { "scala-cli", "scala" });
        if (executable == null)
        {
            return new RunResult { Success = false, Error = "Scala runner not found (install scala-cli or scala)" };
        }

        var partArg = part.HasValue ? part.Value.ToString() : string.Empty;
        var args = executable.EndsWith("scala-cli", StringComparison.OrdinalIgnoreCase)
            ? $"run \"{entryPath}\" {partArg}".Trim()
            : $"\"{entryPath}\" {partArg}".Trim();

        var psi = new ProcessStartInfo
        {
            FileName = executable,
            Arguments = args,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            WorkingDirectory = metadata.Path
        };

        try
        {
            using var process = Process.Start(psi);
            if (process == null)
                return new RunResult { Success = false, Error = "Failed to start Scala process" };

            await process.StandardInput.WriteAsync(input);
            process.StandardInput.Close();

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await Task.WhenAll(outputTask, errorTask, process.WaitForExitAsync());

            return new RunResult
            {
                Success = process.ExitCode == 0,
                Output = outputTask.Result.Trim(),
                Error = string.IsNullOrWhiteSpace(errorTask.Result) ? null : errorTask.Result
            };
        }
        catch (Exception ex)
        {
            return new RunResult { Success = false, Error = $"Scala runner failed: {ex.Message}" };
        }
    }

    private static string? FindExecutable(IEnumerable<string> names)
    {
        var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        var separators = OperatingSystem.IsWindows() ? ';' : ':';
        var extensions = OperatingSystem.IsWindows() ? new[] { ".exe", ".cmd", ".bat", "" } : new[] { "" };

        foreach (var dir in pathEnv.Split(separators, StringSplitOptions.RemoveEmptyEntries))
        {
            foreach (var name in names)
            {
                foreach (var ext in extensions)
                {
                    var candidate = Path.Combine(dir, name + ext);
                    if (File.Exists(candidate))
                        return candidate;
                }
            }
        }

        return null;
    }
}
