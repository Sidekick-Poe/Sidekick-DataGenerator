namespace Sidekick.Data.Game.StatDescriptions.Models;

public class StatDescriptionText
{
    public List<StatDescriptionRange> Ranges { get; set; } = new();

    public string? Text { get; set; }
}
