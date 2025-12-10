using System.Diagnostics;
using AocRunner.Models;
using AocRunner.Runners;
using AocRunner.Utils;

return await CliApp.RunAsync(args);

static class CliApp
{
    public static async Task<int> RunAsync(string[] args)
    {
        if (args.Length == 0 || IsHelp(args[0]))
            return UsagePrinter.Print();

        var command = args[0].ToLowerInvariant();
        var remaining = args.Skip(1).ToArray();

        return command switch
        {
            "run" => await RunCommandAsync(remaining),
            "new" => await NewAsync(remaining),
            _ => UsagePrinter.Print($"Unknown command '{command}'.")
        };
    }

    private static async Task<int> RunCommandAsync(string[] args)
    {
        if (args.Length < 2)
            return UsagePrinter.Print("Run requires <year> <day>.");

        if (!int.TryParse(args[0], out var year))
            return UsagePrinter.Print("Year must be an integer.");
        if (!int.TryParse(args[1], out var day))
            return UsagePrinter.Print("Day must be an integer.");

        bool useSample = false;
        int? part = null;

        for (var i = 2; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg is "--sample" or "-s")
            {
                useSample = true;
                continue;
            }

            if (part == null && int.TryParse(arg, out var parsedPart))
            {
                part = parsedPart;
                continue;
            }

            return UsagePrinter.Print($"Unknown argument '{arg}' for run command.");
        }

        var config = ConfigManager.Load();

        var metadata = SolutionMetadata.Load(config.SolutionPath, year, day);
        if (metadata == null)
        {
            Console.WriteLine($"No solution found for {year} day {day:D2}");
            return 1;
        }

        string input;

        if (useSample)
        {
            var samplePath = Path.Combine(metadata.Path, "sample.txt");
            if (!File.Exists(samplePath))
            {
                Console.WriteLine($"No sample.txt found at {samplePath}. Create it and add sample input.");
                return 1;
            }

            input = await File.ReadAllTextAsync(samplePath);
        }
        else
        {
            input = await InputDownloader.GetInputAsync(year, day, config);
        }

        var runner = RunnerFactory.Create(metadata.Language);

        Console.WriteLine($"Running {year} day {day:D2} ({metadata.Language}){(useSample ? " [sample]" : "")}");
        Console.WriteLine(new string('=', 50));

        var sw = Stopwatch.StartNew();
        var result = await runner.Run(metadata, input, part);
        sw.Stop();

        if (!result.Success)
        {
            var error = string.IsNullOrWhiteSpace(result.Error) ? "Unknown error" : result.Error;
            Console.WriteLine($"Error: {error}");
            return 1;
        }

        if (part.HasValue)
        {
            Console.WriteLine($"Part {part}: {result.Output}");
        }
        else
        {
            Console.WriteLine(result.Output);
        }
        Console.WriteLine($"\nTime: {sw.ElapsedMilliseconds}ms");
        return 0;
    }

    private static async Task<int> NewAsync(string[] args)
    {
        if (args.Length < 2)
            return UsagePrinter.Print("New requires <year> <day>.");

        if (!int.TryParse(args[0], out var year))
            return UsagePrinter.Print("Year must be an integer.");
        if (!int.TryParse(args[1], out var day))
            return UsagePrinter.Print("Day must be an integer.");

        var language = "python";

        for (var i = 2; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg is "--lang" or "-l")
            {
                if (i + 1 >= args.Length)
                    return UsagePrinter.Print("Missing language after --lang.");

                language = args[++i];
            }
            else
            {
                return UsagePrinter.Print($"Unknown argument '{arg}' for new command.");
            }
        }

        var config = ConfigManager.Load();
        var scaffolder = new SolutionScaffolder(config.SolutionPath);

        try
        {
            var result = await scaffolder.CreateAsync(year, day, language);
            Console.WriteLine(result.Message);
            return result.Success ? 0 : 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to scaffold day {day:D2} ({language}): {ex.Message}");
            return 1;
        }
    }

    private static bool IsHelp(string arg) =>
        arg is "-h" or "--help" or "help";
}
