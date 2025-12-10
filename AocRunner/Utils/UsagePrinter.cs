namespace AocRunner.Utils;

public static class UsagePrinter
{
    public static int Print(string? error = null)
    {
        if (!string.IsNullOrWhiteSpace(error))
        {
            Console.WriteLine($"Error: {error}");
            Console.WriteLine();
        }

        Console.WriteLine("Usage: ./aoc [run|new] <year> <day> [part]");
        Console.WriteLine("Options:");
        Console.WriteLine("  --sample           Use sample.txt for input instead of downloading");
        Console.WriteLine("  --lang <language>  Language template when creating a new day (default: python)");
        Console.WriteLine("Examples:");
        Console.WriteLine("  ./aoc run 2025 1");
        Console.WriteLine("  ./aoc run 2025 1 2");
        Console.WriteLine("  ./aoc run 2025 1 2 --sample");
        Console.WriteLine("  ./aoc new 2025 1 --lang python");
        return 1;
    }
}
