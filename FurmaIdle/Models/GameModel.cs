namespace FurmaIdle.Models
{
    public class GameModel
    {
        public int SchemaVersion { get; set; }

        // Ativo
        public DateTime StartTime { get; set; }
        public DateTime LastTick { get; set; }
        public DateTimeOffset LastTickUtc { get; set; } = DateTimeOffset.UtcNow;
        public string SelectedStageId { get; set; } = "s00";
        public string CurrentExpansionId { get; set; } = "x00";

        // Ui
        public HashSet<string> UnlockedMenus { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        // Total
        public StatsModel NoExpeditionStats { get; set; } = new();
        public StatsModel GameStats { get; set; } = new();
        public Dictionary<string, CoinModel> Coins { get; set; } = new();
        public Dictionary<string, ClickModel> Clicks { get; set; } = new();
        public Dictionary<string, StageModel> Stages { get; set; } = new();
        public Dictionary<string, LocalModel> Locals { get; set; } = new();
        public Dictionary<string, TechModel> Techs { get; set; } = new();
        public Dictionary<string, UpgradeModel> Upgrades { get; set; } = new();
        public Dictionary<string, ResourceModel> Resources { get; set; } = new();
        public Dictionary<string, CharacterModel> Characters { get; set; } = new();
        public Dictionary<string, ContractModel> Contracts { get; set; } = new();
        public Dictionary<string, KnowledgeModel> Knowledges { get; set; } = new();
        public Dictionary<string, ExpansionModel> Expansions { get; set; } = new();
        public Dictionary<string, SpecialtyModel> Specialties { get; set; } = new();
        public Dictionary<string, TraitModel> Traits { get; set; } = new();
    }
}
