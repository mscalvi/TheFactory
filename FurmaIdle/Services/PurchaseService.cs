using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Reflection.Metadata.Ecma335;
using System.Xml;
using static FurmaIdle.Helpers.EffectHelper;
using static FurmaIdle.Helpers.PricingHelper;

namespace FurmaIdle.Services
{
    public interface IPurchaseService
    {
        Task Purchase(ItemHelper.ItemType type, string itemId, string stageId);
    }

    public sealed class PurchaseService : IPurchaseService
    {
        private readonly ICurrentGameService _game;
        private readonly ILocateService _locate;
        private readonly IUiLogService _log;
        private readonly IEffectService _effect;
        private readonly IUiService _ui;
        private readonly ICostService _cost;
        private readonly ILoreService _lore;

        public PurchaseService(ICurrentGameService Game, IUiLogService Log, ILocateService Locate, IEffectService effect, IUiService ui, ICostService cost, ILoreService lore)
        {
            _game = Game;
            _locate = Locate;
            _log = Log;
            _effect = effect;
            _ui = ui;
            _cost = cost;
            _lore = lore;
        }

        private int contractBuy = 0;
        private bool busy = false;

        // Purchase
        public async Task Purchase(ItemHelper.ItemType type, string itemId, string stageId)
        {
            if (busy)
            {
                return;
            }

            busy = true;

            var game = _game.CurrentGame;
            var expansion = _locate.LocateExpansion(game, game.CurrentExpansionId);
            var stage = _locate.LocateStage(game, stageId);

            var cost = _cost.ComputeCost(type, itemId, stageId);
            var coinCost = new CoinModel();
            var resourceCost = new ResourceModel();
            var knowledgeCost = new KnowledgeModel();

            bool hasFunds = cost.costId[0] switch
            {
                'm' => GetOrZero(stage.ExpeditionStats.Coins, cost.costId) >= cost.costValue,
                'r' => GetOrZero(expansion.ExpansionStats.Resources, cost.costId) >= cost.costValue,
                'k' => GetOrZero(expansion.ExpansionStats.Knowledge, cost.costId) >= cost.costValue,
                _ => false
            };

            switch (cost.costId[0])
            {
                case 'm':
                    coinCost = _locate.LocateCoin(game, cost.costId);
                    break;
                case 'r':
                    resourceCost = _locate.LocateResource(game, cost.costId);
                    break;
                case 'k':
                    knowledgeCost = _locate.LocateKnowledge(game, cost.costId);
                    break;
            }

            if (!hasFunds)
            {
                return;
            }

            await _game.Mutate(game =>
            {
                if (cost.costId[0] != 'm')
                {
                    ApplyDebit(expansion.ExpansionStats, cost.costValue, cost.costId);
                }
                else
                {
                    ApplyDebit(stage.ExpeditionStats, cost.costValue, cost.costId);
                }

                switch (type)
                {
                    case ItemHelper.ItemType.Upgrade:
                        //var upgrade = _locate.LocateUpgrade(game, itemId);
                        break;

                    case ItemHelper.ItemType.Contract:
                        var contract = _locate.LocateContract(game, itemId);

                        contract.GameUseState = UnlockHelper.ContractState.InUse;
                        stage.ActiveContracts ??= new Dictionary<string, int>(StringComparer.Ordinal);
                        stage.ActiveContracts[contract.Id] = (stage.ActiveContracts.TryGetValue(contract.Id, out var q) ? q : 0) + 1;

                        contractBuy = q + 1;

                        stage.lockedContractLevel.Add(contract.Level);
                        if (!expansion.inUseContracts.Contains(contract.Id))
                        {
                            expansion.inUseContracts.Add(contract.Id);
                        }
                        break;

                    case ItemHelper.ItemType.Specialty:
                        // var spec = _locate.LocateSpecialty(game, itemId);
                        break;
                }

                ApplyStats(expansion.ExpansionStats, game.GameStats, cost.costValue, cost.costId);

            }, save: false, ui: false);

            // Aplica Efeito
            await _effect.ApplyEffect(type, itemId, stageId);

            // Avisa UI e Salva
            await _game.Mutate(game =>
            {
                if (game.GameStats.CharactersUnlocked == 2 && itemId.StartsWith("up"))
                {
                    _ui.NavMenuControl("FirstCharacterPurchase");
                    _lore.LoreTrigger("FirstCharacterPurchase");
                    _lore.LoreTrigger(itemId);
                }
                else if (game.GameStats.KnowledgesUnlocked == 1 && itemId.StartsWith("uk"))
                {
                    _ui.NavMenuControl("FirstKnowledgePurchase");
                    _lore.LoreTrigger("FirstKnowledgePurchase");
                    _lore.LoreTrigger(itemId);
                }
                else if (game.GameStats.TechUnlocked == 1 && itemId.StartsWith("uh"))
                {
                    _ui.NavMenuControl("FirstTechPurchase");
                    _lore.LoreTrigger("FirstTechPurchase");
                    _lore.LoreTrigger(itemId);
                }
                else if (itemId.StartsWith("c"))
                {
                    _ui.NavMenuControl(itemId, contractBuy.ToString());
                    _lore.LoreTrigger(itemId, contractBuy.ToString());
                }
                else
                {
                    _ui.NavMenuControl(itemId);
                    _lore.LoreTrigger(itemId);
                }

            }, save: true, ui: true);

            busy = false;
        }

        private static void ApplyDebit(StatsModel stats, long cost, string costId)
        {
            char costGroup = costId?[0] ?? '\0';

            switch (costGroup)
            {
                case 'm':
                    AddOrSet(stats.Coins, costId, -cost);
                    break;

                case 'r':
                    AddOrSet(stats.Resources, costId, -cost);
                    break;

                case 'k':
                    AddOrSet(stats.Knowledge, costId, -cost);
                    break;

                default:
                    break;
            }
        }
        private static void ApplyStats(StatsModel expansion, StatsModel game, long cost, string costId)
        {
            char costGroup = costId?[0] ?? '\0';

            switch (costGroup)
            {
                case 'm':
                    AddOrSet(expansion.CoinsSpent, costId, +cost);
                    AddOrSet(game.CoinsSpent, costId, +cost);
                    break;

                case 'r':
                    AddOrSet(expansion.ResourcesSpent, costId, +cost);
                    AddOrSet(game.ResourcesSpent, costId, +cost);
                    break;

                case 'k':
                    AddOrSet(expansion.KnowledgeSpent, costId, +cost);
                    AddOrSet(game.KnowledgeSpent, costId, +cost);
                    break;

                default:
                    break;
            }
        }
        private static void AddOrSet(Dictionary<string, long> dict, string id, long delta)
        {
            if (!dict.TryGetValue(id, out var v)) v = 0L;
            var nv = v + delta;
            if (nv < 0) nv = 0;
            dict[id] = nv;
        }

        private static long GetOrZero(Dictionary<string, long> dict, string id)
                    => dict is not null && dict.TryGetValue(id, out var v) ? v : 0L;
    }
}