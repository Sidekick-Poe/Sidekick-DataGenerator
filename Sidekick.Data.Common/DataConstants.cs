namespace Sidekick.Data.Common;

public static class DataConstants
{
    public static Dictionary<string, Language> Languages = new()
    {
        {
            "en",
            new Language()
            {
                GameName = "English",
                ApiLocalSuffix = "(Local)",
            }
        },
        {
            "de",
            new Language()
            {
                GameName = "German",
                ApiLocalSuffix = "(Local)",
            }
        },
        {
            "es",
            new Language()
            {
                GameName = "Spanish",
                ApiLocalSuffix = "(Local)",
            }
        },
        {
            "fr",
            new Language()
            {
                GameName = "French",
                ApiLocalSuffix = "(Local)",
            }
        },
        {
            "kr",
            new Language()
            {
                GameName = "Korean",
                ApiLocalSuffix = "(Local)",
            }
        },
        {
            "pt",
            new Language()
            {
                GameName = "Portuguese",
                ApiLocalSuffix = "(Local)",
            }
        },
        {
            "ru",
            new Language()
            {
                GameName = "Russian",
                ApiLocalSuffix = "(Local)",
            }
        },
        {
            "th",
            new Language()
            {
                GameName = "Thai",
                ApiLocalSuffix = "(Local)",
            }
        },
        {
            "zh",
            new Language()
            {
                GameName = "Traditional Chinese",
                ApiLocalSuffix = "(Local)",
            }
        },
    };
}
