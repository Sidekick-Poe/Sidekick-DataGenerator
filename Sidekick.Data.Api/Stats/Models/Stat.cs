namespace Sidekick.Data.Api.Stats.Models;

public class Stat
{
    public string Id { get; set; }
    public string Category { get; set; }
    public string Text { get; set; }

    public int? OptionId { get; set; }
    public string OptionText { get; set; }
}
