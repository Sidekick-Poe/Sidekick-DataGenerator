namespace Sidekick.Data.Api.Client;

internal static class Languages
{
    // Set base url for the apis
    public static Dictionary<string, string> BaseUrls = new()
    {
        { "de", "https://de.pathofexile.com/" },
        { "en", "https://www.pathofexile.com/" },
        { "es", "https://es.pathofexile.com/" },
        { "fr", "https://fr.pathofexile.com/" },
        { "kr", "https://poe.game.daum.net/" },
        { "pt", "https://br.pathofexile.com/" },
        { "ru", "https://ru.pathofexile.com/" },
        { "th", "https://th.pathofexile.com/" },
        { "zh", "http://web.poe.garena.tw/" },
    };

    // Set base cdn for the apis
    public static Dictionary<string, string> BaseCdns = new()
    {
        { "de", "https://web.poecdn.com" },
        { "en", "https://web.poecdn.com" },
        { "es", "https://web.poecdn.com" },
        { "fr", "https://web.poecdn.com" },
        { "kr", "https://web.poecdn.com" },
        { "pt", "https://web.poecdn.com" },
        { "ru", "https://web.poecdn.com" },
        { "th", "https://web.poecdn.com" },
        { "zh", "https://web.poecdn.com" },
    };
}
