using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sidekick.Data.Api.Client;

public class ApiClient : IDisposable
{
    // Set base url for the apis
    private static Dictionary<string, string> Languages = new()
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

    public ApiClient()
    {
        HttpClient = new HttpClient();
        HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
        HttpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");

        Options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        Options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    public HttpClient HttpClient { get; }
    public JsonSerializerOptions Options { get; }

    public async Task<Dictionary<string, List<TReturn>>> FetchAll<TReturn>(string apiPath)
    {
        var results = new Dictionary<string, List<TReturn>>();

        // Loop through all languages and extract modifiers
        foreach (var baseUrl in Languages)
        {
            Console.WriteLine($"API Exporting {baseUrl.Key} {typeof(TReturn).Name}");

            var fetchResult = await Fetch<TReturn>(baseUrl.Key, apiPath);
            results.Add(baseUrl.Key, fetchResult);
        }

        return results;
    }

    private async Task<List<TReturn>> Fetch<TReturn>(string language, string path)
    {
        var response = await HttpClient.GetAsync(Languages[language] + path);
        var content = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<FetchResult<TReturn>>(content, Options);
        return result?.Result;
    }

    public void Dispose()
    {
        HttpClient.Dispose();
    }
}
