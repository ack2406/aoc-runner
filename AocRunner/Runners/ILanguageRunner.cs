using AocRunner.Models;

namespace AocRunner.Runners;

public interface ILanguageRunner
{
    Task<RunResult> Run(SolutionMetadata metadata, string input, int? part);
}
