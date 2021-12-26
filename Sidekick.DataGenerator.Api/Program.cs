using Sidekick.Data.Api.Client;
using Sidekick.Data.Api.Modifiers;
using Sidekick.Data.Api.StaticItems;
using Sidekick.Data.Common;
using System.Text.Json;

Console.WriteLine("Sidekick Api Data extraction tool");
Console.WriteLine("Starting extraction");

var configuration = new DataConfiguration();
var apiPath = Path.Combine(configuration.DataPath, "Api/");

// Set base url for the apis
var baseUrls = new Dictionary<string, string>()
{
    { "de", "https://de.pathofexile.com/" },
    { "en", "https://www.pathofexile.com/" },
    { "es", "https://es.pathofexile.com/" },
    { "fr", "https://fr.pathofexile.com/" },
    { "kr", "https://poe.game.daum.net/" },
    { "pt", "https://br.pathofexile.com/" },
    { "ru", "https://ru.pathofexile.com/" },
    { "th", "https://th.pathofexile.com/" },
    { "zh", "http://web.poe.garena.tw/" },
};

try
{
    // Create the HttpClient with default options for this api
    using var apiClient = new ApiClient();

    // Make sure the output directory is empty and exists
    if (Directory.Exists(apiPath))
    {
        Directory.Delete(apiPath, true);
    }
    Directory.CreateDirectory(apiPath);

    // Loop through all languages and extract modifiers
    foreach (var baseUrl in baseUrls)
    {
        Console.WriteLine($"Starting {baseUrl.Key} modifiers");

        // Get modifiers from the api
        var results = await apiClient.Fetch<ApiCategory>(baseUrl.Key, "api/trade/data/stats");

        using var stream = File.Create(Path.Combine(apiPath, $"modifiers.{baseUrl.Key}.json"));
        await JsonSerializer.SerializeAsync(stream, results);

        Console.WriteLine($"Finished {baseUrl.Key} modifiers");
    }

    // Loop through all languages and extract static
    foreach (var baseUrl in baseUrls)
    {
        Console.WriteLine($"Starting {baseUrl.Key} static items");

        // Get static items from the api
        var results = await apiClient.Fetch<StaticItemCategory>(baseUrl.Key, "api/trade/data/static");

        using var stream = File.Create(Path.Combine(apiPath, $"static.{baseUrl.Key}.json"));
        await JsonSerializer.SerializeAsync(stream, results);

        Console.WriteLine($"Finished {baseUrl.Key} static items");
    }
}
catch (Exception e)
{
    Console.WriteLine($"An exception occured! {e.Message}");
    Console.ReadKey();
    throw;
}

Console.WriteLine("Done!");
Console.ReadKey();
