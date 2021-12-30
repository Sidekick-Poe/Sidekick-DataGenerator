using CsvHelper.Configuration.Attributes;

namespace Sidekick.Data.Game.Stats
{
    public class Stat
    {
        [Name("Id")]
        public string Id { get; set; }

        [Name("IsLocal")]
        public bool IsLocal { get; set; }

        [Name("IsWeaponLocal")]
        public bool IsWeaponLocal { get; set; }
    }
}
