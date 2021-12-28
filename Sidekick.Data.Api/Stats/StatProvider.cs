using Sidekick.Data.Api.Client;
using Sidekick.Data.Api.Stats.Models;
using Sidekick.Data.Common;

namespace Sidekick.Data.Api.Stats;

public class StatProvider
{
    private readonly DataFileProvider dataFileProvider;
    private readonly ApiClient apiClient;

    public StatProvider(
        DataFileProvider dataFileProvider,
        ApiClient apiClient)
    {
        this.dataFileProvider = dataFileProvider;
        this.apiClient = apiClient;
    }

    public Dictionary<string, List<StatCategory>>? StatCategories { get; set; }

    public async Task Build()
    {
        if (StatCategories != null) return;

        StatCategories = await apiClient.FetchAll<StatCategory>("api/trade/data/stats");

        foreach (var category in StatCategories)
        {
            await dataFileProvider.WriteJson($"Api/stats.{category.Key}.json", category.Value);
        }
    }
}
