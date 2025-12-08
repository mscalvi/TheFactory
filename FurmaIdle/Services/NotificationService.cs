using FurmaIdle.Helpers;
using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public interface INotificationService
    {
        void UpdateVisibleUpgrades(string tabId, int visibleCount);
        bool IsUpgradeVisible(GameModel game, StageModel stage, UpgradeModel up);
        int GetContractNeeded(EffectHelper.EffectType effectType, int nextBuy);
        void NotificationAtualize(string upgradeId, string stageId);
    }

    public sealed class NotificationService : INotificationService
    {
        private readonly ICurrentGameService _game;
        private readonly IUiService _ui;
        private readonly ILocateService _locate;
        private readonly IContractsService _contracts;
        private readonly ICostService _cost;

        public NotificationService(
            ICurrentGameService game,
            IUiService ui,
            ILocateService locate,
            IContractsService contracts,
            ICostService cost)
        {
            _game = game;
            _ui = ui;
            _locate = locate;
            _contracts = contracts;
            _cost = cost;
        }

        public void UpdateVisibleUpgrades(string tabId, int visibleCount)
        {
            var g = _game.CurrentGame;
            if (g is null) return;

            g.Ui ??= new UiState();
            g.Ui.VisibleUpgradesByTab ??= new(StringComparer.OrdinalIgnoreCase);

            g.Ui.VisibleUpgradesByTab.TryGetValue(tabId, out var oldCount);

            if (visibleCount > oldCount)
            {
                _ui.SetNotificationTab(tabId);
            }

            g.Ui.VisibleUpgradesByTab[tabId] = visibleCount;
        }

        public int GetContractNeeded(EffectHelper.EffectType effectType, int nextBuy)
        {
            return effectType switch
            {
                EffectHelper.EffectType.ContractCost => 25 * nextBuy,
                EffectHelper.EffectType.ContractGain => 10 * nextBuy - 5,
                EffectHelper.EffectType.ContractTime => 10 * nextBuy,
                _ => 0
            };
        }
        public void NotificationAtualize(string upgradeId, string stageId)
        {
            var g = _game.CurrentGame;
            if (g is null) return;

            if (!g.Stages.TryGetValue(stageId, out var stage) || stage is null)
                return;

            var up = _locate.LocateUpgrade(g, upgradeId);
            if (up is null || string.IsNullOrWhiteSpace(up.TabId))
                return;

            var uiTabId = GetUiTabIdFromPanelKey(up.TabId);
            if (string.IsNullOrWhiteSpace(uiTabId))
                return;

            RebuildMinPriceForTabInternal(g, stage, uiTabId);
        }


        // ------------- helpers -------------

        public bool IsUpgradeVisible(GameModel game, StageModel stage, UpgradeModel up)
        {
            if (up.Id.StartsWith("uc", StringComparison.OrdinalIgnoreCase))
            {
                return VisibleUpgradeContract(game, stage, up);
            }

            if (up.Id.StartsWith("uu", StringComparison.OrdinalIgnoreCase))
            {
                return VisibleUnlockContract(game, stage, up);
            }

            return true;
        }

        private bool VisibleUpgradeContract(GameModel game, StageModel stage, UpgradeModel up)
        {
            if (!up.Id.StartsWith("uc", StringComparison.OrdinalIgnoreCase))
                return true;

            if (stage.ActiveContracts.TryGetValue(up.TargetId, out var qty))
            {
                if (qty > 0)
                {
                    var nextBuy = Math.Max(1, (up.ActualBuy) + 1);
                    var maxBuy = up.MaxBuy <= 0 ? int.MaxValue : up.MaxBuy;

                    var needed = GetContractNeeded(up.EffectType, nextBuy);
                    if (needed <= 0) return true;

                    return qty >= needed;
                }
            }
            return false;
        }

        private bool VisibleUnlockContract(GameModel game, StageModel stage, UpgradeModel up)
        {
            bool inStage = false;

            foreach (var character in game.Characters.Values)
            {
                if (character.InStageId == stage.Id)
                {
                    if (character.ContractsIds.Contains(up.TargetId))
                    {
                        inStage = true;
                    }
                }
            }

            if (inStage)
            {
                var stageInfo = _contracts.GetStageInfo(game, stage.Id);
                if (up.Level > stageInfo.ContractsLevel)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return inStage;
        }

        private static string GetUiTabIdFromPanelKey(string panelKey)
        {
            // se você usou TabId "guild-hall-permanent", "guild-hall-expansion"
            // dá pra pegar só "guild-hall":
            var idx = panelKey.LastIndexOf('-');
            if (idx <= 0) return panelKey;
            return panelKey[..idx];
        }
        private IEnumerable<string> GetAllUiTabIds(GameModel g)
        {
            return g.Upgrades.Values
                .Where(u => !string.IsNullOrWhiteSpace(u.TabId))
                .Select(u => GetUiTabIdFromPanelKey(u.TabId))
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }
        private void RebuildMinPriceForAllTabs(GameModel g, StageModel stage)
        {
            g.Ui ??= new UiState();
            g.Ui.MinPriceByTab ??= new(StringComparer.OrdinalIgnoreCase);
            g.Ui.MinPriceUpgradeByTab ??= new(StringComparer.OrdinalIgnoreCase);

            g.Ui.MinPriceByTab.Clear();
            g.Ui.MinPriceUpgradeByTab.Clear();

            foreach (var uiTabId in GetAllUiTabIds(g))
            {
                RebuildMinPriceForTabInternal(g, stage, uiTabId);
            }
        }
        private void RebuildMinPriceForTabInternal(GameModel g, StageModel stage, string uiTabId)
        {
            g.Ui ??= new UiState();
            g.Ui.MinPriceByTab ??= new(StringComparer.OrdinalIgnoreCase);
            g.Ui.MinPriceUpgradeByTab ??= new(StringComparer.OrdinalIgnoreCase);

            long? minPrice = null;
            string? minUpId = null;

            foreach (var up in g.Upgrades.Values)
            {
                if (up.State != UnlockHelper.State.Available)
                    continue;

                if (string.IsNullOrWhiteSpace(up.TabId))
                    continue;

                // converte TabId do upgrade em tab pai
                var tabParent = GetUiTabIdFromPanelKey(up.TabId);
                if (!string.Equals(tabParent, uiTabId, StringComparison.OrdinalIgnoreCase))
                    continue;

                // visibilidade (uu/uc/etc.)
                if (!IsUpgradeVisible(g, stage, up))
                    continue;

                // filtro de stage
                if (up.StageId != "all" && up.StageId != stage.Id)
                    continue;

                var price = _cost.ComputeCost(ItemHelper.ItemType.Upgrade, up.Id, stage.Id); 

                if (minPrice is null || price.costValue < minPrice.Value)
                {
                    minPrice = price.costValue;
                    minUpId = up.Id;
                }
            }

            if (minPrice is null || minUpId is null)
            {
                g.Ui.MinPriceByTab.Remove(uiTabId);
                g.Ui.MinPriceUpgradeByTab.Remove(uiTabId);
                return;
            }

            g.Ui.MinPriceByTab[uiTabId] = minPrice.Value;
            g.Ui.MinPriceUpgradeByTab[uiTabId] = minUpId;

            if (_cost.CanAfford(ItemHelper.ItemType.Upgrade, minUpId, stage.Id))
            {
                _ui.SetNotificationTab(uiTabId);
            }
        }
    }

}
