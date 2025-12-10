using System.Diagnostics;
using AocRunner.Models;

namespace AocRunner.Runners;

public class PythonRunner : ILanguageRunner
{
    public async Task<RunResult> Run(SolutionMetadata metadata, string input, int? part)
    {
        var entryPoint = metadata.EntryPoint ?? "solution.py";
        var scriptPath = Path.Combine(metadata.Path, entryPoint);

        var psi = new ProcessStartInfo
        {
            FileName = "python3",
            Arguments = $"\"{scriptPath}\" {(part.HasValue ? part.Value.ToString() : "")}",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(psi);
        if (process == null)
            return new RunResult { Success = false, Error = "Failed to start Python" };

        await process.StandardInput.WriteAsync(input);
        process.StandardInput.Close();

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        return new RunResult
        {
            Success = process.ExitCode == 0,
            Output = output.Trim(),
            Error = string.IsNullOrEmpty(error) ? null : error
        };
    }
}
