namespace AocRunner.Models;

public class RunResult
{
    public bool Success { get; set; }
    public string Output { get; set; } = "";
    public string? Error { get; set; }
}
