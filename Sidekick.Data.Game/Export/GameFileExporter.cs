using LibDat2;
using LibGGPK2;
using LibGGPK2.Records;
using Sidekick.Data.Common;
using System.Text;

namespace Sidekick.Data.Game.Export;

public class GameFileExporter
{
    private readonly DataConfiguration configuration;
    private readonly DataFileProvider dataFileProvider;

    public GameFileExporter(
        DataConfiguration configuration,
        DataFileProvider dataFileProvider)
    {
        this.configuration = configuration;
        this.dataFileProvider = dataFileProvider;
    }

    private static Dictionary<string, string> Languages = new()
    {
        { "en", "" }, // English
        { "de", "German/" },
        { "es", "Spanish/" },
        { "fr", "French/" },
        { "kr", "Korean/" },
        { "pt", "Portuguese/" },
        { "ru", "Russian/" },
        { "th", "Thai/" },
        { "zh", "Traditional Chinese/" },
    };

    private static List<string> Records = new()
    {
        // Data
        "Bundles2/Data/{LANGUAGE}/ItemClasses.dat",
        "Bundles2/Data/{LANGUAGE}/Stats.dat",
        "Bundles2/Data/{LANGUAGE}/BaseItemTypes.dat",
        "Bundles2/Data/{LANGUAGE}/Mods.dat",

        // Metadata
        "Bundles2/Metadata/StatDescriptions/passive_skill_aura_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/passive_skill_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/active_skill_gem_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/advanced_mod_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/aura_skill_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/banner_aura_skill_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/beam_skill_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/brand_skill_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/buff_skill_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/curse_skill_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/debuff_skill_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/secondary_debuff_skill_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/gem_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/minion_attack_skill_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/minion_skill_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/minion_spell_skill_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/minion_spell_damage_skill_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/monster_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/offering_skill_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/single_minion_spell_skill_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/skillpopup_stat_filters.txt",
        "Bundles2/Metadata/StatDescriptions/skill_stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/stat_descriptions.txt",
        "Bundles2/Metadata/StatDescriptions/variable_duration_skill_stat_descriptions.txt",
    };

    public async Task WriteFiles()
    {
        var ggpkContainer = new GGPKContainer(configuration.GgpkPath);

        foreach (var record in Records)
        {
            if (record.Contains("{LANGUAGE}"))
            {
                foreach (var lang in Languages)
                {
                    var langPath = record.Replace("{LANGUAGE}/", lang.Value);

                    Console.WriteLine($"GGPK Exporting {langPath}");

                    await Write(ggpkContainer, langPath, lang.Key);
                }
            }
            else
            {
                Console.WriteLine($"GGPK Exporting {record}");

                await Write(ggpkContainer, record, null);
            }
        }
    }

    public async Task Write(GGPKContainer ggpkContainer, string record, string? language)
    {
        var ggpkRecord = ggpkContainer.FindRecord(record);

        if (ggpkRecord != null && ggpkRecord is IFileRecord fileRecord)
        {
            switch (fileRecord.DataFormat)
            {
                case IFileRecord.DataFormats.Dat:
                    var dat = new DatContainer(fileRecord.ReadFileContent(ggpkContainer.fileStream), ggpkRecord.Name);

                    var datPath = $"Game/{Path.GetFileNameWithoutExtension(ggpkRecord.Name)}.csv";
                    if (!string.IsNullOrEmpty(language))
                    {
                        datPath = datPath.Replace(".csv", $".{language}.csv");
                    }

                    await dataFileProvider.WriteRaw(datPath, dat.ToCsv());
                    break;

                case IFileRecord.DataFormats.Unicode:
                    var unicode = Encoding.Unicode.GetString(fileRecord.ReadFileContent(ggpkContainer.fileStream));

                    var unicodePath = $"Game/{Path.GetFileNameWithoutExtension(ggpkRecord.Name)}{Path.GetExtension(ggpkRecord.Name)}";
                    if (!string.IsNullOrEmpty(language))
                    {
                        unicodePath = unicodePath.Replace(Path.GetExtension(ggpkRecord.Name), $".{language}{Path.GetExtension(ggpkRecord.Name)}");
                    }

                    await dataFileProvider.WriteRaw(unicodePath, unicode);
                    break;
            }
        }
    }
}
