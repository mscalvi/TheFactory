using FurmaIdle.Helpers;
using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public interface INotificationService
    {
        Dictionary<string, List<UpgradeModel>> TabsUpgrades { get; }
        void InitialTabs (GameModel game);
        bool UpdateVisibleUpgrades(string tabId);
        void RefreshAffordableNotifications();

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

        public void InitialTabs(GameModel game)
        {
            TabsUpgrades = new Dictionary<string, List<UpgradeModel>>(StringComparer.OrdinalIgnoreCase);

            TabsListsFormer();

            game.Ui ??= new UiState();
            game.Ui.VisibleUpgradesByTab ??= new(StringComparer.OrdinalIgnoreCase);

            game.Ui.VisibleUpgradesByTab = TabsUpgrades;

            game.Ui.MinPriceByTab ??= new(StringComparer.OrdinalIgnoreCase);

            foreach (var coin in game.Coins)
            {
                if (coin.Value.State == UnlockHelper.State.Unlocked)
                {
                    SetMinPrice(game, "guild-contracts", coin.Value.Id);
                    SetMinPrice(game, "guild-hall", coin.Value.Id);
                    SetMinPrice(game, "guild-lab", coin.Value.Id);
                    SetMinPrice(game, "stage-dock", coin.Value.Id);
                    SetMinPrice(game, "stage-locals", coin.Value.Id);
                }
            }

            foreach (var know in game.Knowledges)
            {
                if (know.Value.State == UnlockHelper.State.Unlocked)
                {
                    SetMinPrice(game, "guild-contracts", know.Value.Id);
                    SetMinPrice(game, "guild-hall", know.Value.Id);
                    SetMinPrice(game, "guild-lab", know.Value.Id);
                    SetMinPrice(game, "stage-dock", know.Value.Id);
                    SetMinPrice(game, "stage-locals", know.Value.Id);
                }
            }
        }
        public bool UpdateVisibleUpgrades(string tabId)
        {
            var game = _game.CurrentGame;
            if (game is null) return false;

            game.Ui ??= new UiState();
            game.Ui.VisibleUpgradesByTab ??= new(StringComparer.OrdinalIgnoreCase);
            game.Ui.MinPriceByTab ??= new(StringComparer.OrdinalIgnoreCase);

            // 1) "all" despacha pras tabs principais
            if (tabId == "all")
            {
                bool any = false;

                string[] tabs =
                {
            "guild-contracts",
            "guild-hall",
            "guild-lab",
            "stage-locals",
            "stage-dock"
        };

                foreach (var t in tabs)
                {
                    any |= UpdateVisibleUpgrades(t);
                }

                return any;
            }

            // 2) Uma tab específica

            // Seed inicial (save antigo / tab nova) – sem notificar, só sincroniza
            if (!game.Ui.VisibleUpgradesByTab.TryGetValue(tabId, out var seenUps) || seenUps is null)
            {
                var seedLists = TabsListsActualize(tabId);
                if (!seedLists.TryGetValue(tabId, out seenUps) || seenUps is null)
                    return false;

                game.Ui.VisibleUpgradesByTab[tabId] = new List<UpgradeModel>(seenUps);

                // Mesmo sem item novo, já calcula min prices e affordable
                RebuildMinPricesForTab(game, tabId);
                UpdateAffordableNotificationForTab(game, tabId);
                return false;
            }

            var actualLists = TabsListsActualize(tabId);
            if (!actualLists.TryGetValue(tabId, out var actualUpgrades) || actualUpgrades is null)
                return false;

            var hasNew = false;

            foreach (var upgrade in actualUpgrades)
            {
                if (string.IsNullOrWhiteSpace(upgrade.Id))
                    continue;

                if (!seenUps.Contains(upgrade))
                {
                    seenUps.Add(upgrade);
                    hasNew = true;
                }
            }

            if (hasNew)
            {
                game.Ui.VisibleUpgradesByTab[tabId] = seenUps;
                _ui.SetNotificationTab(tabId, NotificationKind.NewItem);
            }

            // 3) Sempre recalc min prices depois de atualizar visíveis
            RebuildMinPricesForTab(game, tabId);

            // 4) E em seguida tenta promover para Affordable, se couber
            UpdateAffordableNotificationForTab(game, tabId);

            return hasNew;
        }
        public void RefreshAffordableNotifications()
        {
            var game = _game.CurrentGame;
            if (game is null) return;
            if (game.Ui?.MinPriceByTab is null) return;

            string[] tabs =
            {
                "guild-contracts",
                "guild-hall",
                "guild-lab",
                "stage-locals",
                "stage-dock"
            };

            foreach (var tabId in tabs)
            {
                UpdateAffordableNotificationForTab(game, tabId);
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
                bool inStage = false;

                foreach (var characterId in stage.Expedition.PartyIds)
                {
                    var character = _locate.LocateCharacter(game, characterId);

                    if (character.ContractsIds.Contains(upgrade.TargetId))
                    {
                        inStage = true;
                    }
                }

                return inStage;
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
        private void SetMinPrice(GameModel game, string tabId, string costId)
        {
            game.Ui ??= new UiState();
            game.Ui.MinPriceByTab ??= new(StringComparer.OrdinalIgnoreCase);

            // garante dicionário interno para a tab
            if (!game.Ui.MinPriceByTab.TryGetValue(tabId, out var tabMap) || tabMap is null)
            {
                tabMap = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
                game.Ui.MinPriceByTab[tabId] = tabMap;
            }

            // calcula o "mínimo" de fato pra essa tab/moeda
            var minPrice = TabListsPricing(tabId, costId);

            // se não encontrou nada, nem registra
            if (minPrice <= 0)
                return;

            // se já existe um valor, guarda o menor entre eles
            if (tabMap.TryGetValue(costId, out var existing))
            {
                if (minPrice < existing)
                    tabMap[costId] = minPrice;
            }
            else
            {
                tabMap[costId] = minPrice;
            }
        }
        private void RebuildMinPricesForTab(GameModel game, string tabId)
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
        private void UpdateAffordableNotificationForTab(GameModel game, string tabId)
        {
            if (game.Ui?.MinPriceByTab is null)
                return;

            if (!game.Ui.MinPriceByTab.TryGetValue(tabId, out var prices) ||
                prices is null || prices.Count == 0)
                return;

            foreach (var kv in prices)
            {
                var costId = kv.Key;
                var minPrice = kv.Value;

                if (HasFundsFor(game, costId, minPrice))
                {
                    _ui.SetNotificationTab(tabId, NotificationKind.Affordable);
                    return;
                }
            }
        }

    }
}
