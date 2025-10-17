namespace FurmaIdle.Models
{
    public class GameModel
    {
        public int SchemaVersion { get; set; }

        // Ativo
        public DateTime LastTick { get; set; }
        public DateTimeOffset LastTickUtc { get; set; } = DateTimeOffset.UtcNow;
        public string SelectedStageId { get; set; } = "s00";
        public List<string> ActiveStagesIds { get; set; }
        public bool Started { get; set; } = false;
        public bool On { get; set; } = false;

        // public GuildModel Guild { get; set; } = new();
        // public RuntimeModel Runtime { get; set; } = new RuntimeModel();

        // Total
        public StatsModel Stats { get; set; } = new();
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
