namespace FurmaIdle.Models
{
    public class StatsModel
    {
        // Stats for this Game

        public Dictionary<string, long> Coins { get; set; }
        public Dictionary<string, long> Resources { get; set; }
        public Dictionary<string, long> Knowledge { get; set; }


        // Stats for Archievement
        public Dictionary<string, long> CoinsGain { get; set; }
        public Dictionary<string, long> CoinsSpent { get; set; }

        public Dictionary<string, long> ResourcesGain { get; set; }
        public Dictionary<string, long> ResourcesSpent { get; set; }

        public Dictionary<string, long> KnowledgeGain { get; set; }
        public Dictionary<string, long> KnowledgeSpent { get; set; }

        public Dictionary<string, long> SpecialtiesUsed { get; set; }

        public Dictionary<string, long> ContractsMade { get; set; }

        public Dictionary<string, long> ClicksMade { get; set; }

        public int UpgradesUnlocked { get; set; }
        public int TechUnlocked { get; set; }
        public int DestinationUnlocked { get; set; }
        public int CharactersUnlocked { get; set; }

        public TimeOnly TimeSpent { get; set; }
    }
}
