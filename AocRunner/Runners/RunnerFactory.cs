namespace AocRunner.Runners;

public static class RunnerFactory
{
    public static ILanguageRunner Create(string language)
    {
        return language.ToLower() switch
        {
            "python" => new PythonRunner(),
            "go" => new GoRunner(),
            "scala" => new ScalaRunner(),
            _ => throw new NotSupportedException($"Language {language} not supported")
        };
    }
}
