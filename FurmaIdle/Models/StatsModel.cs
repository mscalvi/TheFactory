namespace FurmaIdle.Models
{
    public class StatsModel
    {
        public Dictionary<string, int> CoinsGain { get; set; }
        public Dictionary<string, int> ResourcesGain { get; set; }
        public Dictionary<string, int> CoinsSpent { get; set; }
        public Dictionary<string, int> ResourcesSpent { get; set; }
        public Dictionary<string, int> SpecialtiesUsed { get; set; }
        public Dictionary<string, int> ContractsMade { get; set; }
        public int UpgradesUnlocked { get; set; }
        public int TechUnlocked { get; set; }
        public int DestinationUnlocked { get; set; }
        public int CharactersUnlocked { get; set; }
        public TimeOnly TimeSpent { get; set; }
    }
}
