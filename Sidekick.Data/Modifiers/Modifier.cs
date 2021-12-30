namespace Sidekick.Data.Modifiers;

public class Modifier
{
    public List<string> GameIds { get; set; } = new();
    public string GameText { get; set; }

    public string ApiId { get; set; }
    public string ApiCategory { get; set; }
    public string ApiText { get; set; }

    public int? ApiOptionId { get; set; }
    public string ApiOptionText { get; set; }

    public string Regex { get; set; }

    public List<string> ValidClassCategories { get; set; } = new();
}
