using FurmaIdle.Helpers;
using FurmaIdle.Models;
using static FurmaIdle.Helpers.UnlockHelper;

namespace FurmaIdle.Services
{
    public interface INotificationService
    {
        Dictionary<string, List<UpgradeModel>> TabsUpgrades { get; }
        void InitialTabs (GameModel game);
        bool UpdateVisibleUpgrades(string tabId);
        void AllTabsAffordables();

        // Helpers
        int ContractUpgradeAvailable(EffectHelper.EffectType type, int nextBuy);
    }

    public sealed class NotificationService : INotificationService
    {
        private readonly ICurrentGameService _game;
        private readonly IUiService _ui;
        private readonly ILocateService _locate;
        private readonly ICostService _cost;

        public NotificationService(ICurrentGameService game, IUiService ui, ILocateService locate, ICostService cost)
        {
            _game = game;
            _ui = ui;
            _locate = locate;
            _cost = cost;
        }

        public Dictionary<string, List<UpgradeModel>> TabsUpgrades { get; set; } = new Dictionary<string, List<UpgradeModel>>(StringComparer.OrdinalIgnoreCase);

        string[] tabs =
        {
                "guild-contracts",
                "guild-hall",
                "guild-lab",
                "stage-locals",
                "stage-dock",
                "world-expansion",
                "world-map",
        };

        public void InitialTabs(GameModel game)
        {
            TabsUpgrades = TabsListsFormer();

            game.Ui ??= new UiState();
            game.Ui.VisibleUpgradesByTab ??= new(StringComparer.OrdinalIgnoreCase);
            game.Ui.MinPriceByTab ??= new(StringComparer.OrdinalIgnoreCase);

            foreach (var kvp in TabsUpgrades)
            {
                game.Ui.VisibleUpgradesByTab[kvp.Key] = new List<UpgradeModel>(kvp.Value);
            }

            foreach (var tabId in tabs)
            {
                MinPriceForTab(game, tabId);
            }
        }

        public bool UpdateVisibleUpgrades(string tabId)
        {
            var game = _game.CurrentGame;
            if (game is null) return false;

            game.Ui ??= new UiState();
            game.Ui.VisibleUpgradesByTab ??= new(StringComparer.OrdinalIgnoreCase);
            game.Ui.MinPriceByTab ??= new(StringComparer.OrdinalIgnoreCase);

            // 1) "all" despacha pras principais
            if (tabId == "all")
            {
                bool any = false;
                foreach (var t in tabs)
                    any |= UpdateVisibleUpgrades(t);
                return any;
            }

            // 2) carrega o "antes" (ou vazio, se ainda não existia)
            game.Ui.VisibleUpgradesByTab.TryGetValue(tabId, out var previousList);
            var prevIds = previousList is null
                ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                : new HashSet<string>(
                    previousList.Where(u => !string.IsNullOrWhiteSpace(u.Id))
                                .Select(u => u.Id),
                    StringComparer.OrdinalIgnoreCase);

            // 3) recalcula upgrades visíveis da tab
            var actualLists = TabsListsActualize(tabId);
            if (!actualLists.TryGetValue(tabId, out var actualUpgrades) || actualUpgrades is null)
                return false;

            // 4) detecta se apareceu algum Id que não tinha antes
            bool hasNew = false;
            foreach (var up in actualUpgrades)
            {
                if (string.IsNullOrWhiteSpace(up.Id)) continue;
                if (!prevIds.Contains(up.Id))
                {
                    hasNew = true;
                    break;
                }
            }

            // 5) salva a lista nova como "visível"
            game.Ui.VisibleUpgradesByTab[tabId] = new List<UpgradeModel>(actualUpgrades);

            // 6) notificação de item novo
            if (hasNew)
                _ui.SetNotificationTab(tabId, NotificationKind.NewItem);

            // 7) recalcula preços mínimos + Affordable
            MinPriceForTab(game, tabId);
            NotifyAffordables(game, tabId);

            return hasNew;
        }

        public void AllTabsAffordables()
        {
            var game = _game.CurrentGame;
            if (game is null) return;
            if (game.Ui?.MinPriceByTab is null) return;

            foreach (var tabId in tabs)
            {
                NotifyAffordables(game, tabId);
            }
        }


        // Helpers
        public int ContractUpgradeAvailable(EffectHelper.EffectType type, int nextBuy)
        {
            var game = _game.CurrentGame;

            return type switch
            {
                EffectHelper.EffectType.ContractCost => 25 * nextBuy,
                EffectHelper.EffectType.ContractGain => 10 * nextBuy - 5,
                EffectHelper.EffectType.ContractTime => 10 * nextBuy,
                _ => 0
            };
        }
        private bool IsUpgradeVisible(GameModel game, StageModel stage, UpgradeModel upgrade)
        {
            if (upgrade.Id == null) return false;

            if (upgrade.State != UnlockHelper.State.Available) return false;

            if (upgrade.StageId != "all" && upgrade.StageId != stage.Id) return false;

            if (upgrade.Id.StartsWith("uc"))
            {
                if (stage.ActiveContracts is not null)
                {
                    if (!stage.ActiveContracts.ContainsKey(upgrade.TargetId)) return false;

                    var nextBuy = Math.Max(1, upgrade.ActualBuy + 1);
                    var maxBuy = upgrade.MaxBuy <= 0 ? int.MaxValue : upgrade.MaxBuy;
                    if (nextBuy > maxBuy) return false;

                    stage.ActiveContracts.TryGetValue(upgrade.TargetId, out var qty);
                    if (qty <= 0) return false;

                    var needed = ContractUpgradeAvailable(upgrade.EffectType, nextBuy);
                    if (needed > 0 && qty < needed) return false;

                    return true;
                }
            }

            if (upgrade.Id.StartsWith("uu"))
            {
                foreach (var characterId in stage.Expedition.PartyIds)
                {
                    var character = _locate.LocateCharacter(game, characterId);

                    if (character.ContractsIds.Contains(upgrade.TargetId))
                    {
                        var contract = _locate.LocateContract(game, upgrade.TargetId);
                        if (contract.Level <= stage.ActualContractLevel)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            if (upgrade.Id.StartsWith("ut"))
            {
                if (upgrade.TargetId.StartsWith("c") && stage.ActiveContracts.ContainsKey(upgrade.TargetId))
                {
                    return true;
                } else
                {
                    return true;
                }
            }

            return true;
        }
        private Dictionary<string, List<UpgradeModel>> TabsListsFormer()
        {
            Dictionary<string, List<UpgradeModel>> TabLists = new Dictionary<string, List<UpgradeModel>>();

            var game = _game.CurrentGame;
            var stage = _locate.LocateStage(game, game.SelectedStageId);

            var all = game.Upgrades.Values;

            List<UpgradeModel> _contractsExpedition = new();
            List<UpgradeModel> _contractsExpansion = new();
            List<UpgradeModel> _contractsTutorial = new();

            List<UpgradeModel> _hallPermanents = new();
            List<UpgradeModel> _hallExpansion = new();

            List<UpgradeModel> _labKnowledge = new();
            List<UpgradeModel> _labTech = new();
            List<UpgradeModel> _labUpgrades = new();

            List<UpgradeModel> _stageLocals = new();
            List<UpgradeModel> _stageLockedLocals = new();

            List<UpgradeModel> _stageShips = new();
            List<UpgradeModel> _stageRoutes = new();

            List<UpgradeModel> _worldExpansion = new();
            List<UpgradeModel> _worldStages = new();

            _contractsExpedition = all
                .Where(u =>
                    u.State == UnlockHelper.State.Available &&
                    u.TabId == "guild-contracts-fast" &&
                    IsUpgradeVisible(game, stage, u) &&
                    (u.StageId == stage.Id || u.StageId == "all"))
                .ToList();

            _contractsExpansion = all
                .Where(u =>
                    u.State == UnlockHelper.State.Available &&
                    u.TabId == "guild-contracts-long" &&
                    IsUpgradeVisible(game, stage, u) &&
                    (u.StageId == stage.Id || u.StageId == "all"))
                .ToList();

            _contractsTutorial = all
                .Where(u =>
                    u.State == UnlockHelper.State.Available &&
                    u.TabId == "tutorial-end" &&
                    IsUpgradeVisible(game, stage, u) &&
                    (u.StageId == stage.Id || u.StageId == "all"))
                .ToList();

            var _contractsTab = _contractsExpedition
                .Concat(_contractsExpansion)
                .Concat(_contractsTutorial)
                .ToList();

            _hallPermanents = all.Where(u =>
                    u.State == UnlockHelper.State.Available &&
                    (u.TabId == "guild-hall-permanent") &&
                    IsUpgradeVisible(game, stage, u) &&
                    (u.StageId == stage.Id || u.StageId == "all"))
                .ToList();

            _hallExpansion = all.Where(u =>
                    u.State == UnlockHelper.State.Available &&
                    (u.TabId == "guild-hall-expansion") &&
                    IsUpgradeVisible(game, stage, u) &&
                    (u.StageId == stage.Id || u.StageId == "all"))
                .ToList();

            var _hallTab = _hallExpansion
                .Concat(_hallPermanents)
                .ToList();

            _labKnowledge = all
                .Where(u =>
                    u.State == UnlockHelper.State.Available &&
                    u.TabId == "guild-lab-knowledge" &&
                    IsUpgradeVisible(game, stage, u) &&
                    (u.StageId == stage.Id || u.StageId == "all"))
                .ToList();

            _labTech = all
                .Where(u =>
                    u.State == UnlockHelper.State.Available &&
                    u.TabId == "guild-lab-techs" &&
                    IsUpgradeVisible(game, stage, u) &&
                    (u.StageId == stage.Id || u.StageId == "all"))
                .ToList();

            _labUpgrades = all
                .Where(u =>
                    u.State == UnlockHelper.State.Available &&
                    u.TabId == "guild-lab-upgrades" &&
                    IsUpgradeVisible(game, stage, u) &&
                    (u.StageId == stage.Id || u.StageId == "all"))
                .ToList();

            var _labTab = _labKnowledge
                .Concat(_labTech)
                .Concat(_labUpgrades)
                .ToList();

            _stageLocals = all
                .Where(u =>
                    u?.Id != null &&
                    u.TabId == "stage-locals" &&
                    u.State == UnlockHelper.State.Available &&
                    IsUpgradeVisible(game, stage, u) &&
                    (u.StageId == stage.Id || u.StageId == "all"))
                .ToList();

            _stageLockedLocals = all
                .Where(u =>
                    u?.Id != null &&
                    u.TabId == "stage-locals" &&
                    u.State == UnlockHelper.State.Blocked &&
                    u.StageId == stage.Id &&
                    IsUpgradeVisible(game, stage, u))
                .ToList();

            var _localsTab = _stageLockedLocals
                .Concat(_stageLocals)
                .ToList();

            _stageShips = all
                .Where(u =>
                    u?.Id != null &&
                    u.TabId == "stage-dock-ships" &&
                    u.State == UnlockHelper.State.Available &&
                    IsUpgradeVisible(game, stage, u))
                .ToList();

            _stageRoutes = all
                .Where(u =>
                    u?.Id != null &&
                    u.TabId == "stage-dock-routes" &&
                    u.State == UnlockHelper.State.Available &&
                    IsUpgradeVisible(game, stage, u))
                .ToList();

            var _dockTab = _stageShips
                .Concat(_stageRoutes)
                .ToList();

            _worldExpansion = all
                .Where(u => u.State == UnlockHelper.State.Available 
                && u.TabId == "expansion").ToList();

            var _expansionTab = _worldExpansion;            

            _worldStages = all
                .Where(u => u.State == UnlockHelper.State.Available
                && u.TabId == "world-map-stages").ToList();

            var _mapTab = _worldStages;

            TabLists.Add("contractsExpedition", _contractsExpedition);
            TabLists.Add("contractsExpansion", _contractsExpansion);
            TabLists.Add("contractsTutorial", _contractsTutorial);
            TabLists.Add("guild-contracts", _contractsTab);

            TabLists.Add("hallPermanents", _hallPermanents);
            TabLists.Add("hallExpansion", _hallExpansion);
            TabLists.Add("guild-hall", _hallTab);

            TabLists.Add("labKnowledge", _labKnowledge);
            TabLists.Add("labTech", _labTech);
            TabLists.Add("labUpgrades", _labUpgrades);
            TabLists.Add("guild-lab", _labTab);

            TabLists.Add("stageLocals", _stageLocals);
            TabLists.Add("stageLockedLocals", _stageLockedLocals);
            TabLists.Add("stage-locals", _localsTab);

            TabLists.Add("stageShips", _stageShips);
            TabLists.Add("stageRoutes", _stageRoutes);
            TabLists.Add("stage-dock", _dockTab);

            TabLists.Add("worldExpansion", _worldExpansion);
            TabLists.Add("world-expansion", _expansionTab);

            TabLists.Add("worldStages", _worldStages);
            TabLists.Add("world-map", _mapTab);

            TabsUpgrades = TabLists;

            return TabLists;
        }
        private Dictionary<string, List<UpgradeModel>> TabsListsActualize(string tabId)
        {
            Dictionary<string, List<UpgradeModel>> TabLists = new Dictionary<string, List<UpgradeModel>>();

            var game = _game.CurrentGame;
            var stage = _locate.LocateStage(game, game.SelectedStageId);

            var all = game.Upgrades.Values;

            switch (tabId) 
            {
                case "guild-contracts":
                    List<UpgradeModel> _contractsExpedition = new();
                    List<UpgradeModel> _contractsExpansion = new();
                    List<UpgradeModel> _contractsTutorial = new();

                    _contractsExpedition = all
                        .Where(u =>
                            u.State == UnlockHelper.State.Available &&
                            u.TabId == "guild-contracts-fast" &&
                            IsUpgradeVisible(game, stage, u) &&
                            (u.StageId == stage.Id || u.StageId == "all"))
                        .ToList();

                    _contractsExpansion = all
                        .Where(u =>
                            u.State == UnlockHelper.State.Available &&
                            u.TabId == "guild-contracts-long" &&
                            IsUpgradeVisible(game, stage, u) &&
                            (u.StageId == stage.Id || u.StageId == "all"))
                        .ToList();

                    _contractsTutorial = all
                        .Where(u =>
                            u.State == UnlockHelper.State.Available &&
                            u.TabId == "tutorial-end" &&
                            IsUpgradeVisible(game, stage, u) &&
                            (u.StageId == stage.Id || u.StageId == "all"))
                        .ToList();

                    var _contractsTab = _contractsExpedition
                        .Concat(_contractsExpansion)
                        .Concat(_contractsTutorial)
                        .ToList();

                    TabLists.Add("contractsExpedition", _contractsExpedition);
                    TabLists.Add("contractsExpansion", _contractsExpansion);
                    TabLists.Add("contractsTutorial", _contractsTutorial);
                    TabLists.Add("guild-contracts", _contractsTab);
                    break;
                case "guild-hall":
                    List<UpgradeModel> _hallPermanents = new();
                    List<UpgradeModel> _hallExpansion = new();
                    _hallPermanents = all.Where(u =>
                            u.State == UnlockHelper.State.Available &&
                            (u.TabId == "guild-hall-permanent") &&
                            IsUpgradeVisible(game, stage, u) &&
                            (u.StageId == stage.Id || u.StageId == "all"))
                        .ToList();

                    _hallExpansion = all.Where(u =>
                            u.State == UnlockHelper.State.Available &&
                            (u.TabId == "guild-hall-expansion") &&
                            IsUpgradeVisible(game, stage, u) &&
                            (u.StageId == stage.Id || u.StageId == "all"))
                        .ToList();

                    var _hallTab = _hallExpansion
                        .Concat(_hallPermanents)
                        .ToList();

                    TabLists.Add("hallPermanents", _hallPermanents);
                    TabLists.Add("hallExpansion", _hallExpansion);
                    TabLists.Add("guild-hall", _hallTab);
                    break;
                case "guild-lab":
                    List<UpgradeModel> _labKnowledge = new();
                    List<UpgradeModel> _labTech = new();
                    List<UpgradeModel> _labUpgrades = new();

                    _labKnowledge = all
                        .Where(u =>
                            u.State == UnlockHelper.State.Available &&
                            u.TabId == "guild-lab-knowledge" &&
                            IsUpgradeVisible(game, stage, u) &&
                            (u.StageId == stage.Id || u.StageId == "all"))
                        .ToList();

                    _labTech = all
                        .Where(u =>
                            u.State == UnlockHelper.State.Available &&
                            u.TabId == "guild-lab-techs" &&
                            IsUpgradeVisible(game, stage, u) &&
                            (u.StageId == stage.Id || u.StageId == "all"))
                        .ToList();

                    _labUpgrades = all
                        .Where(u =>
                            u.State == UnlockHelper.State.Available &&
                            u.TabId == "guild-lab-upgrades" &&
                            IsUpgradeVisible(game, stage, u) &&
                            (u.StageId == stage.Id || u.StageId == "all"))
                        .ToList();

                    var _labTab = _labKnowledge
                        .Concat(_labTech)
                        .Concat(_labUpgrades)
                        .ToList();

                    TabLists.Add("labKnowledge", _labKnowledge);
                    TabLists.Add("labTech", _labTech);
                    TabLists.Add("labUpgrades", _labUpgrades);
                    TabLists.Add("guild-lab", _labTab);
                    break;
                case "stage-locals":
                    List<UpgradeModel> _stageLocals = new();
                    List<UpgradeModel> _stageLockedLocals = new();

                    _stageLocals = all
                        .Where(u =>
                            u?.Id != null &&
                            u.TabId == "stage-locals" &&
                            u.State == UnlockHelper.State.Available &&
                            IsUpgradeVisible(game, stage, u) &&
                            (u.StageId == stage.Id || u.StageId == "all"))
                        .ToList();

                    _stageLockedLocals = all
                        .Where(u =>
                            u?.Id != null &&
                            u.TabId == "stage-locals" &&
                            u.State == UnlockHelper.State.Blocked &&
                            u.StageId == stage.Id &&
                            IsUpgradeVisible(game, stage, u))
                        .ToList();

                    var _localsTab = _stageLockedLocals
                        .Concat(_stageLocals)
                        .ToList();

                    TabLists.Add("stageLocals", _stageLocals);
                    TabLists.Add("stageLockedLocals", _stageLockedLocals);
                    TabLists.Add("stage-locals", _localsTab);
                    break;
                case "stage-dock":
                    List<UpgradeModel> _stageShips = new();
                    List<UpgradeModel> _stageRoutes = new();

                    _stageShips = all
                        .Where(u =>
                            u?.Id != null &&
                            u.TabId == "stage-dock-ships" &&
                            u.State == UnlockHelper.State.Available &&
                            IsUpgradeVisible(game, stage, u))
                        .ToList();

                    _stageRoutes = all
                        .Where(u =>
                            u?.Id != null &&
                            u.TabId == "stage-dock-routes" &&
                            u.State == UnlockHelper.State.Available &&
                            IsUpgradeVisible(game, stage, u))
                        .ToList();

                    var _dockTab = _stageShips
                        .Concat(_stageRoutes)
                        .ToList();

                    TabLists.Add("stageShips", _stageShips);
                    TabLists.Add("stageRoutes", _stageRoutes);
                    TabLists.Add("stage-dock", _dockTab);
                    break;
                case "world-expansion":
                    List<UpgradeModel> _worldExpansion = new();

                    _worldExpansion = all
                        .Where(u => u.State == UnlockHelper.State.Available
                        && u.TabId == "expansion").ToList();

                    var _expansionTab = _worldExpansion;
                    TabLists.Add("worldExpansion", _worldExpansion);
                    TabLists.Add("world-expansion", _expansionTab);
                    break;
                case "world-map":
                    List<UpgradeModel> _worldStages = new();

                    _worldStages = all
                        .Where(u => u.State == UnlockHelper.State.Available
                        && u.TabId == "world-map-stages").ToList();

                    var _mapTab = _worldStages;

                    TabLists.Add("worldStages", _worldStages);
                    TabLists.Add("world-map", _mapTab);
                    break;
            }

            TabsUpgrades = TabLists;

            return TabLists;
        }


        // Price Checker
        private long TabListsPricing(string tabId, string costId)
        {
            var game = _game.CurrentGame;
            var stage = _locate.LocateStage(game, game.SelectedStageId);

            if (game.Ui?.VisibleUpgradesByTab is null)
                return 0;

            if (!game.Ui.VisibleUpgradesByTab.TryGetValue(tabId, out var visibleUpgrades) ||
                visibleUpgrades is null || visibleUpgrades.Count == 0)
                return 0;

            long minPrice = long.MaxValue;
            var found = false;

            foreach (var upgrade in visibleUpgrades)
            {
                var upgradeCost = _cost.ComputeCost(ItemHelper.ItemType.Upgrade, upgrade.Id, stage.Id);

                if (upgradeCost.costId == costId)
                {
                    found = true;
                    if (upgradeCost.costValue < minPrice)
                    {
                        minPrice = upgradeCost.costValue;
                    }
                }
            }

            return found ? minPrice : 0;
        }
        private void MinPriceForTab(GameModel game, string tabId)
        {
            game.Ui ??= new UiState();
            game.Ui.MinPriceByTab ??= new(StringComparer.OrdinalIgnoreCase);

            var tabPrices = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

            // Moedas desbloqueadas
            foreach (var kv in game.Coins)
            {
                var coin = kv.Value;
                if (coin.State != UnlockHelper.State.Unlocked)
                    continue;

                var min = TabListsPricing(tabId, coin.Id);
                if (min > 0)
                    tabPrices[coin.Id] = min;
            }

            // Knowledges desbloqueados
            foreach (var kv in game.Knowledges)
            {
                var know = kv.Value;
                if (know.State != UnlockHelper.State.Unlocked)
                    continue;

                var min = TabListsPricing(tabId, know.Id);
                if (min > 0)
                    tabPrices[know.Id] = min;
            }

            game.Ui.MinPriceByTab[tabId] = tabPrices;
        }
        private static long GetOrZero(Dictionary<string, long> dict, string id)
            => dict is not null && dict.TryGetValue(id, out var v) ? v : 0L;
        private bool HasFundsFor(GameModel game, string costId, long minPrice)
        {
            if (minPrice <= 0)
                return false;

            var stage = _locate.LocateStage(game, game.SelectedStageId);
            var expansion = _locate.LocateExpansion(game, game.CurrentExpansionId);

            // Mesma regra do PurchaseService
            return costId[0] switch
            {
                'm' => GetOrZero(stage.ExpeditionStats.Coins, costId) >= minPrice,
                'r' => GetOrZero(expansion.ExpansionStats.Resources, costId) >= minPrice,
                'k' => GetOrZero(expansion.ExpansionStats.Knowledge, costId) >= minPrice,
                _ => false
            };
        }
        private void NotifyAffordables(GameModel game, string tabId)
        {
            if (game.Ui?.MinPriceByTab is null)
                return;

            if (!game.Ui.MinPriceByTab.TryGetValue(tabId, out var prices) ||
                prices is null)
                return;

            game.Ui.TabsNotificationKind.TryGetValue(tabId, out var kind);

            if (prices.Count == 0)
            {
                if (kind == NotificationKind.Affordable)
                {
                    _ui.ClearNotificationTab(tabId);
                }
                return;
            }

            bool anyAffordable = false;

            foreach (var kv in prices)
            {
                var costId = kv.Key;
                var minPrice = kv.Value;

                if (HasFundsFor(game, costId, minPrice))
                {
                    anyAffordable = true;
                    break;
                }
            }

            if (anyAffordable)
            {
                if (kind != NotificationKind.Affordable)
                {
                    _ui.SetNotificationTab(tabId, NotificationKind.Affordable);
                }
            }
            else
            {
                if (kind == NotificationKind.Affordable)
                {
                    _ui.ClearNotificationTab(tabId);
                }
            }
        }

    }
}
