using Sidekick.Data.Common;
using Sidekick.Data.Game.StatDescriptions.Models;
using System.Text.RegularExpressions;

namespace Sidekick.Data.Game.StatDescriptions;

public class StatDescriptionProvider
{
    private readonly DataConfiguration configuration;
    private readonly DataFileProvider dataFileProvider;

    public StatDescriptionProvider(
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

        using var reader = dataFileProvider.OpenRead("Game/stat_descriptions.txt");

        foreach (var language in DataConstants.Languages.Keys)
        {
            Descriptions.Add(language, new List<StatDescription>());
        }

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line == "description")
            {
                ReadDescription(reader);
            }
        }

        foreach (var description in Descriptions)
        {
            await dataFileProvider.WriteJson($"Sidekick/stat_descriptions.{description.Key}.json", description.Value);
        }
    }

    private Regex RegexId = new(@"(?:(?:^\s\d+|\G)\s([^\s]+))", RegexOptions.Compiled);

    private void ReadDescription(StreamReader reader)
    {
        var idLine = reader.ReadLine();
        if (idLine == null)
        {
            Console.WriteLine($"Unexpected line is null in {nameof(StatDescriptionProvider)}.{nameof(ReadDescription)}..");
            return;
        }

        var matches = RegexId.Matches(idLine);
        if (!matches.Any())
        {
            Console.WriteLine($"Unexpected empty matches in {nameof(StatDescriptionProvider)}.{nameof(ReadDescription)}..");
            return;
        }

        var ids = new List<string>();

        foreach (Match match in matches)
        {
            if (match.Groups.Count != 2)
            {
                Console.WriteLine($"Unexpected number of groups in {nameof(StatDescriptionProvider)}.{nameof(ReadDescription)}.");
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
        ReadText(reader, ids, Descriptions["en"]);

        while (true)
        {
            var position = reader.BaseStream.Position;
            var langLine = reader.ReadLine();

            if (RegexEmptyLine.IsMatch(langLine))
            {
                return;
            }

            if (langLine == "description")
            {
                ReadDescription(reader);
                return;
            }

            var match = RegexLangLine.Match(langLine);
            if (!match.Success)
            {
                Console.WriteLine($"Unexpected language value in {nameof(StatDescriptionProvider)}.{nameof(ReadLanguage)}. {langLine}");
                return;
            }

            var language = DataConstants.Languages.First(x => x.Value == match.Groups[1].Value).Key;
            ReadText(reader, ids, Descriptions[language]);
        }
    }

    private Regex RegexCountLine = new(@"^\s*(\d+)$", RegexOptions.Compiled);
    private Regex RegexTextLine = new(@"^\s+([-!#|\s0-9]+?)\s+""(.*)""\s*(.*)$", RegexOptions.Compiled);

    private void ReadText(StreamReader reader, List<string> ids, List<StatDescription> translations)
    {
        var countLine = reader.ReadLine();
        var match = RegexCountLine.Match(countLine);
        if (!match.Success)
        {
            Console.WriteLine($"Unexpected count value in {nameof(StatDescriptionProvider)}.{nameof(ReadText)}. {countLine}");
            return;
        }

        var translation = new StatDescription()
        {
            Ids = ids,
        };

        var count = int.Parse(match.Groups[1].Value);
        for (var i = 0; i < count; i++)
        {
            var textLine = reader.ReadLine();
            var textLineMatch = RegexTextLine.Match(textLine);
            if (!textLineMatch.Success)
            {
                Console.WriteLine($"Unexpected text value in {nameof(StatDescriptionProvider)}.{nameof(ReadText)}. {textLine}");
                continue;
            }

            var range = textLineMatch.Groups[1].Value;
            var options = textLineMatch.Groups[3].Value;

            var text = new StatDescriptionText()
            {
                Text = textLineMatch.Groups[2].Value,
            };

            ReadRange(range, options, text);

            translation.Texts.Add(text);
        }

        translations.Add(translation);
    }

    private void ReadRange(string range, string options, StatDescriptionText translationItem)
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
