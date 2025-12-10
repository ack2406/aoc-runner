namespace AocRunner.Utils;

public static class InputDownloader
{
    public static async Task<string> GetInputAsync(int year, int day, Config config)
    {
        var cacheDir = config.CacheDir;
        Directory.CreateDirectory(cacheDir);

        var cachePath = Path.Combine(cacheDir, $"{year}_day{day:D2}.txt");

        if (File.Exists(cachePath))
            return await File.ReadAllTextAsync(cachePath);

        if (string.IsNullOrEmpty(config.SessionToken))
            throw new Exception("No session token configured");

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Cookie", $"session={config.SessionToken}");

        var url = $"https://adventofcode.com/{year}/day/{day}/input";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var input = await response.Content.ReadAsStringAsync();
        await File.WriteAllTextAsync(cachePath, input);

        return input;
    }
}
