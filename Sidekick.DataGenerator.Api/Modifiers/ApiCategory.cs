namespace Sidekick.Data.Api.Modifiers;

/// <summary>
/// Pseudo, Explicit, Implicit, etc.
/// </summary>
public class ApiCategory
{
    public string? Label { get; set; }
    public List<ApiModifier>? Entries { get; set; }
}
