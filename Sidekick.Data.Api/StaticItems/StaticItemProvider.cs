using Sidekick.Data.Api.Client;
using Sidekick.Data.Api.StaticItems.Models;
using Sidekick.Data.Common;

namespace Sidekick.Data.Api.Stats;

public class StaticItemProvider
{
    private readonly DataFileProvider dataFileProvider;
    private readonly ApiClient apiClient;

    public StaticItemProvider(
        DataFileProvider dataFileProvider,
        ApiClient apiClient)
    {
        this.dataFileProvider = dataFileProvider;
        this.apiClient = apiClient;
    }

    public Dictionary<string, List<StaticItemCategory>>? StaticItemCategories { get; set; }

    public async Task Build()
    {
        if (StaticItemCategories != null) return;

        StaticItemCategories = await apiClient.FetchAll<StaticItemCategory>("api/trade/data/static");

        foreach (var category in StaticItemCategories)
        {
            await dataFileProvider.WriteJson($"Api/static.{category.Key}.json", category.Value);
        }
    }
}
