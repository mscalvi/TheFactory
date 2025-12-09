using FurmaIdle.Services;

namespace FurmaIdle.Models
{
    public class GameModel
    {
        public int SchemaVersion { get; set; }
        public int GameVersion { get; set; } = 1;
        public string? BuildVersion { get; set; }

        // Ativo
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset LastTick { get; set; }
        public string SelectedStageId { get; set; } = "s00";
        public string CurrentExpansionId { get; set; } = "x000";

        // Ui
        public UiState Ui { get; set; } = new();
        public Dictionary<string, bool> LoreTriggers { get; set; } = new();

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
        public Dictionary<string, ShipModel> Ships { get; set; } = new();
        public Dictionary<string, RouteModel> Routes { get; set; } = new();
    }
    public sealed class UiState
    {
        public HashSet<string> UnlockedMenus { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> UnlockedTabs { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> TabsWithNotification { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> HiddenPanels { get; set; } = new(StringComparer.Ordinal);
        public string? OpenMenuId { get; set; }
        public string? OpenTabId { get; set; }

        public List<UiLogMessage> LogBuffer { get; set; } = new();
        public const int LogMax = 200;

        // Notification
        public Dictionary<string, List<UpgradeModel>> VisibleUpgradesByTab { get; set; }
            = new(StringComparer.OrdinalIgnoreCase); // TabId, VisibleUpgrades
        public Dictionary<string, Dictionary<string, long>> MinPriceByTab { get; set; }
            = new(StringComparer.OrdinalIgnoreCase); // TabId, CoinId, Ammount
    }
}
