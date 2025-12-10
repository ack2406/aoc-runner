using System.Diagnostics;
using AocRunner.Models;

namespace AocRunner.Runners;

public class GoRunner : ILanguageRunner
{
    public async Task<RunResult> Run(SolutionMetadata metadata, string input, int? part)
    {
        var entryPoint = metadata.EntryPoint ?? "solution.go";
        var entryPath = Path.Combine(metadata.Path, entryPoint);

        if (!File.Exists(entryPath))
        {
            return new RunResult { Success = false, Error = $"Entry point not found: {entryPath}" };
        }

        var partArg = part.HasValue ? part.Value.ToString() : string.Empty;

        var psi = new ProcessStartInfo
        {
            FileName = "go",
            Arguments = $"run \"{entryPath}\" {partArg}".Trim(),
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
                return new RunResult { Success = false, Error = "Failed to start Go process" };

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
            return new RunResult { Success = false, Error = $"Go runner failed: {ex.Message}" };
        }
    }
}
