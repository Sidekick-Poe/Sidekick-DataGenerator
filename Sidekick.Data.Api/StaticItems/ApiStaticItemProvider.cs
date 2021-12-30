using Sidekick.Data.Api.Client;
using Sidekick.Data.Api.StaticItems.Models;
using Sidekick.Data.Common;

namespace Sidekick.Data.Api.StaticItems;

public class ApiStaticItemProvider
{
    private readonly DataFileProvider dataFileProvider;
    private readonly ApiClient apiClient;

    public ApiStaticItemProvider(
        DataFileProvider dataFileProvider,
        ApiClient apiClient)
    {
        this.dataFileProvider = dataFileProvider;
        this.apiClient = apiClient;
    }

    public Dictionary<string, List<StaticItemCategory>> StaticItemCategories { get; set; } = new();

    public async Task Build()
    {
        if (StaticItemCategories.Any()) return;

        StaticItemCategories = await apiClient.FetchAll<StaticItemCategory>("api/trade/data/static");

        foreach (var category in StaticItemCategories)
        {
            await dataFileProvider.WriteJson($"Api/raw_static.{category.Key}.json", category.Value);
        }
    }
}
