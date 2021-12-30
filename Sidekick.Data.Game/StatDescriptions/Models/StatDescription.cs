namespace Sidekick.Data.Game.StatDescriptions.Models
{
    public class StatDescription
    {
        public List<string> Ids { get; set; } = new();

        public string Text { get; set; }

        public List<StatDescriptionRange> Ranges { get; set; } = new();

        public StatDescriptionOption Options { get; set; }
    }
}