using Sidekick.Data.Common;

namespace Sidekick.Data.Game.Stats;

public class GameStatProvider
{
    private readonly DataFileProvider dataFileProvider;

    public GameStatProvider(
        DataFileProvider dataFileProvider)
    {
        this.dataFileProvider = dataFileProvider;
    }

    public Dictionary<string, Dictionary<string, Stat>> Stats { get; } = new();

    public void Build()
    {
        foreach (var language in DataConstants.Languages.Keys)
        {
            var stats = dataFileProvider.ReadCsv<Stat>($"Game/stats.{language}.csv");
            Stats.Add(language, stats.ToDictionary(x => x.Id ?? ""));
        }
    }
}
