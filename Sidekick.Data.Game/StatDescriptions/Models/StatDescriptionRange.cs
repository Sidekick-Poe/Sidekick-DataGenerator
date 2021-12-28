namespace Sidekick.Data.Game.StatDescriptions.Models;

public class StatDescriptionRange
{
    public int? Minimum { get; set; }

    public int? Maximum { get; set; }

    public bool Negate => Maximum.HasValue && Maximum < 0;
}
