using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sidekick.Data.Api.Client;

internal class ApiClient : IDisposable
{
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

    public async Task<List<TReturn>> Fetch<TReturn>(string language, string path)
    {
        var response = await HttpClient.GetAsync(Languages.BaseUrls[language] + path);
        var content = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<FetchResult<TReturn>>(content, Options);
        return result?.Result;
    }

    public void Dispose()
    {
        HttpClient.Dispose();
    }
}
