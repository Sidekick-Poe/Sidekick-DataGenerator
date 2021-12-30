using Sidekick.Data.Common;
using Sidekick.Data.Game.StatDescriptions.Models;
using System.Text.RegularExpressions;

namespace Sidekick.Data.Game.StatDescriptions;

public class GameStatDescriptionProvider
{
    private readonly DataConfiguration configuration;
    private readonly DataFileProvider dataFileProvider;

    public GameStatDescriptionProvider(
        DataConfiguration configuration,
        DataFileProvider dataFileProvider)
    {
        this.configuration = configuration;
        this.dataFileProvider = dataFileProvider;
    }

    public Dictionary<string, List<StatDescription>> Descriptions { get; } = new();

    public async Task Build()
    {
        if (Descriptions.Any()) return;

        foreach (var language in DataConstants.Languages.Keys)
        {
            Descriptions.Add(language, new List<StatDescription>());
        }

        Read("game/advanced_mod_stat_descriptions.txt");
        Read("game/atlas_stat_descriptions.txt");
        Read("game/chest_stat_descriptions.txt");
        Read("game/expedition_relic_stat_descriptions.txt");
        Read("game/heist_equipment_stat_descriptions.txt");
        Read("game/leaguestone_stat_descriptions.txt");
        Read("game/map_stat_descriptions.txt");
        Read("game/monster_stat_descriptions.txt");
        Read("game/stat_descriptions.txt");

        foreach (var category in Descriptions)
        {
            await dataFileProvider.WriteJson($"Game/stat_descriptions.{category.Key}.json", category.Value);
        }
    }

    private Regex RegexDescriptionLine = new(@"^\s*description(.*)$", RegexOptions.Compiled);

    private void Read(string path)
    {
        using var reader = dataFileProvider.OpenRead(path);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (RegexDescriptionLine.IsMatch(line))
            {
                ReadDescription(reader);
            }
        }
    }

    private Regex RegexId = new(@"(?:(?:^\s\d+|\G)\s([^\s]+))", RegexOptions.Compiled);

    private void ReadDescription(StreamReader reader)
    {
        var idLine = reader.ReadLine();
        if (idLine == null)
        {
            Console.WriteLine($"Unexpected line is null in {nameof(GameStatDescriptionProvider)}.{nameof(ReadDescription)}..");
            return;
        }

        var matches = RegexId.Matches(idLine);
        if (!matches.Any())
        {
            Console.WriteLine($"Unexpected empty matches in {nameof(GameStatDescriptionProvider)}.{nameof(ReadDescription)}..");
            return;
        }

        var ids = new List<string>();

        foreach (Match match in matches)
        {
            if (match.Groups.Count != 2)
            {
                Console.WriteLine($"Unexpected number of groups in {nameof(GameStatDescriptionProvider)}.{nameof(ReadDescription)}.");
                continue;
            }

            var id = match.Groups[1].Value;

            ids.Add(id);
        }

        ReadLanguage(reader, ids);
    }

    private Regex RegexLangLine = new(@"^\s*lang\s""(.+)""$", RegexOptions.Compiled);
    private Regex RegexEmptyLine = new(@"^\s*$", RegexOptions.Compiled);

    private void ReadLanguage(StreamReader reader, List<string> ids)
    {
        ReadText(reader, ids, "en");

        while (true)
        {
            var position = reader.BaseStream.Position;
            var line = reader.ReadLine();

            if (line == null || RegexEmptyLine.IsMatch(line))
            {
                return;
            }
            else if (RegexDescriptionLine.IsMatch(line))
            {
                ReadDescription(reader);
                return;
            }

            var match = RegexLangLine.Match(line);
            if (!match.Success)
            {
                Console.WriteLine($"Unexpected language value in {nameof(GameStatDescriptionProvider)}.{nameof(ReadLanguage)}. {line}");
                return;
            }

            var language = DataConstants.Languages.Where(x => x.Value.GameName == match.Groups[1].Value).Select(x => x.Key).FirstOrDefault();
            if (language == default)
            {
                ReadText(reader, ids, null);
            }
            else
            {
                ReadText(reader, ids, language);
            }
        }
    }

    private Regex RegexCountLine = new(@"^\s*(\d+)$", RegexOptions.Compiled);
    private Regex RegexTextLine = new(@"^\s+([-!#|\s0-9]+?)\s+""(.*)""\s*(.*)$", RegexOptions.Compiled);
    private Regex RegexTokens = new(@"<(?:enchanted|nemesismod)>\{\{(.*)(?:\}\})?", RegexOptions.Compiled);
    private Regex RegexValues = new(@"{(\d*):?(.*?)}", RegexOptions.Compiled);

    private void ReadText(StreamReader reader, List<string> ids, string language)
    {
        var countLine = reader.ReadLine();
        if (countLine == null)
        {
            Console.WriteLine($"Unexpected count value in {nameof(GameStatDescriptionProvider)}.{nameof(ReadText)}. {countLine}");
            return;
        }

        var match = RegexCountLine.Match(countLine);
        if (!match.Success)
        {
            Console.WriteLine($"Unexpected count value in {nameof(GameStatDescriptionProvider)}.{nameof(ReadText)}. {countLine}");
            return;
        }

        var count = int.Parse(match.Groups[1].Value);
        for (var i = 0; i < count; i++)
        {
            var textLine = reader.ReadLine();
            if (textLine == null)
            {
                Console.WriteLine($"Unexpected text value in {nameof(GameStatDescriptionProvider)}.{nameof(ReadText)}. {textLine}");
                continue;
            }

            var textLineMatch = RegexTextLine.Match(textLine);
            if (!textLineMatch.Success)
            {
                Console.WriteLine($"Unexpected text value in {nameof(GameStatDescriptionProvider)}.{nameof(ReadText)}. {textLine}");
                continue;
            }

            var range = textLineMatch.Groups[1].Value;
            var text = textLineMatch.Groups[2].Value;
            var options = textLineMatch.Groups[3].Value;

            text = RegexTokens.Replace(text, (match) =>
            {
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }

                return match.Value;
            });

            text = RegexValues.Replace(text, (match) =>
            {
                if (match.Success)
                {
                    switch (match.Groups[2].Value)
                    {
                        case "d":
                        case null:
                        case "":
                            return "#";

                        case "+d":
                            return "+#";

                        default:
                            Console.WriteLine($"Unexpected format in {nameof(GameStatDescriptionProvider)}.{nameof(ReadText)}. {match.Groups[2].Value} - {textLineMatch.Groups[2].Value}");
                            return "";
                    }
                }

                return match.Value;
            });

            var description = new StatDescription()
            {
                Ids = ids,
                Text = text,
                Options = new()
                {
                    PassiveHash = options.Contains("passive_hash"),
                    ReminderConquered = options.Contains("ReminderTextConqueredPassives"),
                }
            };

            ReadRange(range, options, description);

            if (language != null)
            {
                Descriptions[language].Add(description);
            }
        }
    }

    private void ReadRange(string range, string options, StatDescription translationItem)
    {
        foreach (var value in range.Split(' ').Where(x => !string.IsNullOrEmpty(x)))
        {
            var min = value;
            var max = value;

            if (value.Contains('|'))
            {
                min = value.Split('|')[0];
                max = value.Split('|')[1];
            }

            var rangeModel = new StatDescriptionRange()
            {
                Maximum = null,
                Minimum = null,
            };

            if (min != "#" && min != "!0")
            {
                rangeModel.Minimum = int.Parse(min);
            }

            if (max != "#" && max != "!0")
            {
                rangeModel.Maximum = int.Parse(max);
            }

            translationItem.Ranges.Add(rangeModel);
        }
    }
}
