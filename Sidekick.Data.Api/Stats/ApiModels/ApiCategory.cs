namespace Sidekick.Data.Api.Stats.ApiModels;

/// <summary>
/// Pseudo, Explicit, Implicit, etc.
/// </summary>
public class ApiCategory
{
    public string? Label { get; set; }
    public List<ApiStat>? Entries { get; set; }
}
