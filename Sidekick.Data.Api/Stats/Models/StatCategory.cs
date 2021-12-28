namespace Sidekick.Data.Api.Stats.Models;

/// <summary>
/// Pseudo, Explicit, Implicit, etc.
/// </summary>
public class StatCategory
{
    public string? Label { get; set; }
    public List<StatModifier>? Entries { get; set; }
}
