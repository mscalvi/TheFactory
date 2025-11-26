using FurmaIdle.Services;

namespace FurmaIdle.Models
{
    public class GameModel
    {
        public int SchemaVersion { get; set; }
        public string? BuildVersion { get; set; }

        // Ativo
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset LastTick { get; set; }
        public string SelectedStageId { get; set; } = "s01";
        public string CurrentExpansionId { get; set; } = "x10";

        // Ui
        public UiState Ui { get; set; } = new();

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
    public sealed class UiState
    {
        public HashSet<string> UnlockedMenus { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> HiddenPanels { get; set; } = new(StringComparer.Ordinal);
        public string? OpenMenuId { get; set; }


        // guarde um buffer curto pra não inchar o save
        public List<UiLogMessage> LogBuffer { get; set; } = new();
        public const int LogMax = 200;
    }
}
