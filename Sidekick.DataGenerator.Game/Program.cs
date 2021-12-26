// See https://aka.ms/new-console-template for more information
using LibGGPK2;
using Sidekick.Data.Common;
using Sidekick.DataGenerator.Game;

Console.WriteLine("Sidekick Game Data Extraction Tool");

var configuration = new DataConfiguration();
var outputPath = Path.Combine(configuration.DataPath, "Game/");

// Make sure the output directory is empty and exists
if (Directory.Exists(outputPath))
{
    Directory.Delete(outputPath, true);
}
Directory.CreateDirectory(outputPath);

Console.WriteLine("Opening GGPK File");

var ggpkContainer = new GGPKContainer(configuration.GgpkPath);

Console.WriteLine("GGPK File is opened");

var paths = new List<string>() {
    // Data
    "Bundles2/Data/{LANGUAGE}/ItemClasses.dat",
    "Bundles2/Data/{LANGUAGE}/Stats.dat",
    "Bundles2/Data/{LANGUAGE}/BaseItemTypes.dat",
    "Bundles2/Data/{LANGUAGE}/WeaponTypes.dat",
    "Bundles2/Data/{LANGUAGE}/ArmourTypes.dat",
    "Bundles2/Data/{LANGUAGE}/ShieldTypes.dat",
    "Bundles2/Data/{LANGUAGE}/Flasks.dat",
    "Bundles2/Data/{LANGUAGE}/ComponentCharges.dat",
    "Bundles2/Data/{LANGUAGE}/ComponentAttributeRequirements.dat",
    "Bundles2/Data/{LANGUAGE}/Mods.dat",
    "Bundles2/Data/{LANGUAGE}/ModType.dat",
    "Bundles2/Data/{LANGUAGE}/ModDomains.dat",
    "Bundles2/Data/{LANGUAGE}/ModGenerationType.dat",
    "Bundles2/Data/{LANGUAGE}/ModFamily.dat",
    "Bundles2/Data/{LANGUAGE}/ModAuraFlags.dat",
    "Bundles2/Data/{LANGUAGE}/ActiveSkills.dat",
    "Bundles2/Data/{LANGUAGE}/ActiveSkillTargetTypes.dat",
    "Bundles2/Data/{LANGUAGE}/ActiveSkillType.dat",
    "Bundles2/Data/{LANGUAGE}/ClientStrings.dat",
    "Bundles2/Data/{LANGUAGE}/ItemClasses.dat",
    "Bundles2/Data/{LANGUAGE}/SkillTotems.dat",
    "Bundles2/Data/{LANGUAGE}/SkillTotemVariations.dat",
    "Bundles2/Data/{LANGUAGE}/SkillMines.dat",
    "Bundles2/Data/{LANGUAGE}/Essences.dat",
    "Bundles2/Data/{LANGUAGE}/EssenceType.dat",
    "Bundles2/Data/{LANGUAGE}/Characters.dat",
    "Bundles2/Data/{LANGUAGE}/BuffDefinitions.dat",
    "Bundles2/Data/{LANGUAGE}/BuffCategories.dat",
    "Bundles2/Data/{LANGUAGE}/BuffVisuals.dat",
    "Bundles2/Data/{LANGUAGE}/CraftingBenchOptions.dat",
    "Bundles2/Data/{LANGUAGE}/CraftingItemClassCategories.dat",
    "Bundles2/Data/{LANGUAGE}/CraftingBenchUnlockCategories.dat",
    "Bundles2/Data/{LANGUAGE}/MonsterVarieties.dat",
    "Bundles2/Data/{LANGUAGE}/MonsterResistances.dat",
    "Bundles2/Data/{LANGUAGE}/MonsterTypes.dat",
    "Bundles2/Data/{LANGUAGE}/DefaultMonsterStats.dat",
    "Bundles2/Data/{LANGUAGE}/SkillGems.dat",
    "Bundles2/Data/{LANGUAGE}/GrantedEffects.dat",
    "Bundles2/Data/{LANGUAGE}/GrantedEffectsPerLevel.dat",
    "Bundles2/Data/{LANGUAGE}/ItemExperiencePerLevel.dat",
    "Bundles2/Data/{LANGUAGE}/EffectivenessCostConstants.dat",
    "Bundles2/Data/{LANGUAGE}/StatInterpolationTypes.dat",
    "Bundles2/Data/{LANGUAGE}/Tags.dat",
    "Bundles2/Data/{LANGUAGE}/GemTags.dat",
    "Bundles2/Data/{LANGUAGE}/ItemVisualIdentity.dat",
    "Bundles2/Data/{LANGUAGE}/AchievementItems.dat",
    "Bundles2/Data/{LANGUAGE}/MultiPartAchievements.dat",
    "Bundles2/Data/{LANGUAGE}/AegisVariations.dat",
    "Bundles2/Data/{LANGUAGE}/CostTypes.dat",
    "Bundles2/Data/{LANGUAGE}/PassiveJewelRadii.dat",

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

foreach (var ggpkPath in paths)
{
    if (ggpkPath.Contains("{LANGUAGE}"))
    {
        foreach (var lang in Languages.LanguagePaths)
        {
            var langPath = ggpkPath.Replace("{LANGUAGE}/", lang.Value);

            Console.WriteLine($"Starting exporting - {langPath}");

            var fileGenerator = new GgpkFileGenerator(ggpkContainer, langPath, outputPath, lang.Key);
            await fileGenerator.Write();

            Console.WriteLine($"Finished exporting - {langPath}");
        }
    }
    else
    {
        Console.WriteLine($"Starting exporting - {ggpkPath}");

        var fileGenerator = new GgpkFileGenerator(ggpkContainer, ggpkPath, outputPath, null);
        await fileGenerator.Write();

        Console.WriteLine($"Finished exporting - {ggpkPath}");
    }
}

Console.WriteLine("Done! This program will close in 5 seconds.");
_ = Task.Run(async () =>
{
    await Task.Delay(5000);
    Environment.Exit(0);
});
Console.ReadKey();
