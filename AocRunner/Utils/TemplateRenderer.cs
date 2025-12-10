namespace AocRunner.Utils;

public class TemplateRenderer
{
    private readonly string _templatesRoot;

    public TemplateRenderer(string templatesRoot)
    {
        _templatesRoot = templatesRoot;
    }

    public async Task<string> RenderAsync(string language, string templateName, IReadOnlyDictionary<string, string> tokens)
    {
        var dir = Path.Combine(_templatesRoot, language.ToLowerInvariant());
        if (!Directory.Exists(dir))
            throw new DirectoryNotFoundException($"No templates directory for language '{language}' at {dir}");

        var primary = Path.Combine(dir, $"{templateName}.tpl");
        var path = File.Exists(primary)
            ? primary
            : Directory.GetFiles(dir, $"{templateName}.*.tpl", SearchOption.TopDirectoryOnly).FirstOrDefault();

        if (string.IsNullOrEmpty(path))
            throw new FileNotFoundException($"Template '{templateName}' for language '{language}' not found.", primary);

        var content = await File.ReadAllTextAsync(path);

        foreach (var token in tokens)
        {
            content = content.Replace($"{{{{{token.Key}}}}}", token.Value);
        }

        return content;
    }
}
