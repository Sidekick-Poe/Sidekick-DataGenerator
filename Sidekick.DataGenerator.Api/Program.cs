using Sidekick.Data.Api.Modifiers;
using Sidekick.Data.Api.StaticItems;
using Sidekick.Data.Common;
using Sidekick.DataGenerator.Api.Client;
using System.Text.Json;

Console.WriteLine("Sidekick Api Data Extraction Tool");
Console.WriteLine("Starting extraction");

var configuration = new DataConfiguration();
var outputPath = Path.Combine(configuration.DataPath, "Api/");

try
{
    // Create the HttpClient with default options for this api
    using var apiClient = new ApiClient();

    // Make sure the output directory is empty and exists
    if (Directory.Exists(outputPath))
    {
        Directory.Delete(outputPath, true);
    }
    Directory.CreateDirectory(outputPath);

    // Loop through all languages and extract modifiers
    foreach (var baseUrl in Languages.BaseUrls)
    {
        Console.WriteLine($"Starting {baseUrl.Key} modifiers");

        // Get modifiers from the api
        var results = await apiClient.Fetch<ApiCategory>(baseUrl.Key, "api/trade/data/stats");

        using var stream = File.Create(Path.Combine(outputPath, $"modifiers.{baseUrl.Key}.json"));
        await JsonSerializer.SerializeAsync(stream, results);

        Console.WriteLine($"Finished {baseUrl.Key} modifiers");
    }

    // Loop through all languages and extract static
    foreach (var baseUrl in Languages.BaseUrls)
    {
        Console.WriteLine($"Starting {baseUrl.Key} static items");

        // Get static items from the api
        var results = await apiClient.Fetch<StaticItemCategory>(baseUrl.Key, "api/trade/data/static");

        using var stream = File.Create(Path.Combine(outputPath, $"static.{baseUrl.Key}.json"));
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

Console.WriteLine("Done! This program will close in 5 seconds.");
_ = Task.Run(async () =>
{
    await Task.Delay(5000);
    Environment.Exit(0);
});
Console.ReadKey();
