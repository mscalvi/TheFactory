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

        // Unlock Stats
        public int CharactersUnlocked { get; set; } = 0;
        public int CoinsUnlocked { get; set; } = 0;
        public int ContractsUnlocked { get; set; } = 0;
        public int ExpansionsUnlocked { get; set; } = 0;
        public int KnowledgesUnlocked { get; set; } = 0;
        public int LocalsUnlocked { get; set; } = 0;
        public int StagesUnlocked { get; set; } = 0;
        public int TechUnlocked { get; set; } = 0;
        public int ResourcesUnlocked { get; set; } = 0;
        public int UpgradesUnlocked { get; set; } = 0;


        public TimeOnly TimeSpent { get; set; }
    }
}
