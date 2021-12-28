namespace Sidekick.Data.Game.StatDescriptions.Models;

public class StatDescription
{
    public List<string> Ids { get; set; } = new();

    public List<StatDescriptionText> Texts { get; set; } = new();
}
