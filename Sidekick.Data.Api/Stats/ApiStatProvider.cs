using Sidekick.Data.Api.Client;
using Sidekick.Data.Api.Stats.ApiModels;
using Sidekick.Data.Api.Stats.Models;
using Sidekick.Data.Common;

namespace Sidekick.Data.Api.Stats;

public class ApiStatProvider
{
    private readonly DataFileProvider dataFileProvider;
    private readonly ApiClient apiClient;

    public ApiStatProvider(
        DataFileProvider dataFileProvider,
        ApiClient apiClient)
    {
        this.dataFileProvider = dataFileProvider;
        this.apiClient = apiClient;
    }

    public Dictionary<string, List<Stat>> Stats { get; set; } = new();

    public async Task Build()
    {
        if (Stats.Any()) return;

        var categories = await apiClient.FetchAll<ApiCategory>("api/trade/data/stats");

        foreach (var category in categories)
        {
            Stats.Add(category.Key, new List<Stat>());

            var entries = category.Value
                .Where(x => x.Label != "Pseudo")
                .SelectMany(x => x.Entries);

            // Add entries with options
            Stats[category.Key].AddRange(entries
                .Where(x => x.Option == null || x.Option.Options == null || !x.Option.Options.Any())
                .Select(x => new Stat()
                {
                    Category = x.Id?.Split('.')[0],
                    Id = x.Id,
                    Text = x.Text,
                }));

            // Add entries with options
            Stats[category.Key].AddRange(entries
                .Where(x => x.Option != null && x.Option.Options != null && x.Option.Options.Any())
                .SelectMany(x => x.Option?.Options?.Select(y => new
                {
                    Stat = x,
                    Option = y,
                }))
                .Select(x => new Stat()
                {
                    Category = x.Stat.Id?.Split('.')[0],
                    Id = x.Stat.Id,
                    Text = x.Stat.Text,
                    OptionId = x.Option.Id,
                    OptionText = x.Option.Text,
                }));

            await dataFileProvider.WriteJson($"Api/raw_stats.{category.Key}.json", category.Value);
            await dataFileProvider.WriteJson($"Api/stats.{category.Key}.json", Stats[category.Key]);
        }
    }
}
