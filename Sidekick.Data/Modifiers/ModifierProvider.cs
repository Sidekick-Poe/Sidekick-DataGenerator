using Sidekick.Data.Api.Stats;
using Sidekick.Data.Common;
using Sidekick.Data.Game.StatDescriptions;
using Sidekick.Data.Game.Stats;
using System.Text.RegularExpressions;

namespace Sidekick.Data.Modifiers;

public class ModifierProvider
{
    private readonly GameStatDescriptionProvider gameStatDescriptionProvider;
    private readonly ApiStatProvider apiStatProvider;
    private readonly GameStatProvider gameStatProvider;
    private readonly DataFileProvider dataFileProvider;

    public ModifierProvider(
        GameStatDescriptionProvider gameStatDescriptionProvider,
        ApiStatProvider apiStatProvider,
        GameStatProvider gameStatProvider,
        DataFileProvider dataFileProvider)
    {
        this.gameStatDescriptionProvider = gameStatDescriptionProvider;
        this.apiStatProvider = apiStatProvider;
        this.gameStatProvider = gameStatProvider;
        this.dataFileProvider = dataFileProvider;
    }

    public Dictionary<string, List<Modifier>> Modifiers { get; } = new();

    public async Task Build()
    {
        if (Modifiers.Any()) return;

        foreach (var language in DataConstants.Languages.Keys)
        {
            Modifiers.Add(language, new List<Modifier>());
        }

        BuildModifiers();

        foreach (var keyValue in Modifiers)
        {
            await dataFileProvider.WriteJson($"Sidekick/modifiers.{keyValue.Key}.json", keyValue.Value);
        }

        ValidateApiModifiers();
    }

    private void BuildModifiers()
    {
        foreach (var description in gameStatDescriptionProvider.Descriptions["en"])
        {
            if (description.Ids.Any(x => GameIdToApiText.ContainsKey(x)))
            {
                BuildFromGameIdToApiText(description);
            }
            else
            {
                BuildFromApiModifier(description);
            }
        }
    }

    private Dictionary<string, string> GameIdToApiText { get; } = new()
    {
        { "mod_granted_passive_hash_2", "Allocates # (Second)" },
        { "mod_granted_passive_hash_3", "Allocates # (Third)" },
        { "mod_granted_passive_hash_4", "Allocates # (Fourth)" },
    };

    private void BuildFromGameIdToApiText(Game.StatDescriptions.Models.StatDescription description)
    {
        foreach (var id in description.Ids)
        {
            var apiText = GameIdToApiText[id];

            var apiModifiers = apiStatProvider.Stats["en"].Where(x => x.Text == apiText);

            AddModifiers(description, apiModifiers);
        }
    }

    private Regex PlusHashPattern = new Regex(@"\\\+\\\#", RegexOptions.Compiled);
    private Regex IgnorePattern = new Regex(@" \((?:Local|Shields|Maps|Legacy|Staves)\)$", RegexOptions.Compiled);

    private void BuildFromApiModifier(Game.StatDescriptions.Models.StatDescription description)
    {
        var patternValue = Regex.Escape(description.Text);
        patternValue = PlusHashPattern.Replace(patternValue, @"\+?\#");

        var regex = new Regex($"^{patternValue}$", RegexOptions.IgnoreCase);

        var apiModifiers = apiStatProvider.Stats["en"].Where(x => regex.IsMatch(x.Text));

        AddModifiers(description, apiModifiers);
    }

    private void AddModifiers(Game.StatDescriptions.Models.StatDescription description, IEnumerable<Api.Stats.Models.Stat> apiModifiers)
    {
        if (!apiModifiers.Any())
        {
            Modifiers["en"].Add(new Modifier()
            {
                GameIds = description.Ids,
                GameText = description.Text,
            });
        }

        foreach (var apiModifier in apiModifiers)
        {
            Modifiers["en"].Add(new Modifier()
            {
                ApiCategory = apiModifier.Category,
                ApiId = apiModifier.Id,
                ApiText = apiModifier.Text,
                ApiOptionId = apiModifier.OptionId,
                ApiOptionText = apiModifier.OptionText,

                GameIds = description.Ids,
                GameText = description.Text,
            });
        }
    }

    private void ValidateApiModifiers()
    {
        var error = 0;
        var count = 0;

        foreach (var stat in apiStatProvider.Stats["en"])
        {
            count++;

            if (!Modifiers["en"].Any(x => x.ApiId == stat.Id && x.ApiOptionId == stat.OptionId))
            {
                error++;
                Console.WriteLine($"Unexpected missing API stat in {nameof(ModifierProvider)}.{nameof(ValidateApiModifiers)}. {stat.Id} ({stat.OptionId}) - {stat.Text} ({stat.OptionText})");
            }
        }

        if (error != 0)
        {
            Console.WriteLine($"Api modifier validation - Missing {error}/{count}");
        }
    }
}
