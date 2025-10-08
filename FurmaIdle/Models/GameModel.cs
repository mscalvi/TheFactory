using static FurmaIdle.Models.CharacterModel;

namespace FurmaIdle.Models
{
    public class GameModel
    {
        public int SchemaVersion { get; set; }
        public DateTime LastTick { get; set; }
        public string SelectedStageId { get; set; }
        public List<string> ActiveStagesIds { get; set; }
        public ExpansionModel Expansion { get; set; } = new();
        public GuildModel Guild { get; set; } = new();
        public Dictionary<string, ClickModel> Clicks { get; set; } = new();
        public Dictionary<string, StageModel> Stages { get; set; } = new();
        public Dictionary<string, DestinationModel> Destinations { get; set; } = new();
        public Dictionary<string, TechModel> Technologies { get; set; } = new();
        public Dictionary<string, UpgradeModel> Upgrades { get; set; } = new();
        public Dictionary<string, ResourceModel> Resources { get; set; } = new();
        public Dictionary<string, CharacterModel> Characters { get; set; } = new();
        public Dictionary<string, ContractModel> Contracts { get; set; } = new();
    }
}
