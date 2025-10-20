namespace FurmaIdle.Models
{
    public class StatsModel
    {
        // Actual Stats
        public Dictionary<string, long> Coins { get; set; } = new(StringComparer.Ordinal);
        public Dictionary<string, long> Resources { get; set; } = new(StringComparer.Ordinal);
        public Dictionary<string, long> Knowledge { get; set; } = new(StringComparer.Ordinal);

        public Dictionary<string, double> CoinsFrac { get; set; } = new(StringComparer.Ordinal);
        public Dictionary<string, double> ResourcesFrac { get; set; } = new(StringComparer.Ordinal);
        public Dictionary<string, double> KnowledgeFrac { get; set; } = new(StringComparer.Ordinal);

        // Total Stats Gain
        public Dictionary<string, long> CoinsGain { get; set; } = new(StringComparer.Ordinal);
        public Dictionary<string, long> ResourcesGain { get; set; } = new(StringComparer.Ordinal);
        public Dictionary<string, long> KnowledgeGain { get; set; } = new(StringComparer.Ordinal);

        // Total Stats Use
        public Dictionary<string, long> CoinsSpent { get; set; } = new(StringComparer.Ordinal);
        public Dictionary<string, long> ResourcesSpent { get; set; } = new(StringComparer.Ordinal);
        public Dictionary<string, long> KnowledgeSpent { get; set; } = new(StringComparer.Ordinal);

        // Other Stats
        public Dictionary<string, long> SpecialtiesUsed { get; set; } = new(StringComparer.Ordinal);
        public Dictionary<string, long> ContractsMade { get; set; } = new(StringComparer.Ordinal);
        public Dictionary<string, long> ClicksMade { get; set; } = new(StringComparer.Ordinal);

        public int UpgradesUnlocked { get; set; }
        public int UniqueUpgradesUnlocked { get; set; }

        public int TechUnlocked { get; set; }
        public int UniqueTechUnlocked { get; set; }

        public int DestinationUnlocked { get; set; }
        public int UniqueDestinationUnlocked { get; set; }

        public int CharactersUnlocked { get; set; }
        public int UniqueCharactersUnlocked { get; set; }

        public TimeOnly TimeSpent { get; set; }
    }
}
