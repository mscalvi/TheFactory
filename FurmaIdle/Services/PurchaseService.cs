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

        public PurchaseService(ICurrentGameService Game, IUiLogService Log, ILocateService Locate, IEffectService effect, IUiService ui, ICostService cost)
        {
            _game = Game;
            _locate = Locate;
            _log = Log;
            _effect = effect;
            _ui = ui;
            _cost = cost;
        }

        // Purchase
        public async Task Purchase(ItemHelper.ItemType type, string itemId, string stageId)
        {
            var game = _game.CurrentGame;
            var expansion = _locate.LocateExpansion(game, game.CurrentExpansionId);
            var stage = _locate.LocateStage(game, stageId);

            await _game.Mutate(game =>
            {
                var cost = _cost.ComputeCost(type, itemId, stageId);

                bool hasFunds = cost.costId[0] switch
                {
                    'm' => GetOrZero(stage.ExpeditionStats.Coins, cost.costId) >= cost.costValue,
                    'r' => GetOrZero(expansion.ExpansionStats.Resources, cost.costId) >= cost.costValue,
                    'k' => GetOrZero(expansion.ExpansionStats.Knowledge, cost.costId) >= cost.costValue,
                    _ => false
                };
                if (!hasFunds) return;

                if(cost.costId[0] != 'm')
                {
                    ApplyDebit(expansion.ExpansionStats, cost.costValue, cost.costId);
                } else
                {
                    ApplyDebit(stage.ExpeditionStats, cost.costValue, cost.costId);
                }

                ApplyStats(expansion.ExpansionStats, game.GameStats, cost.costValue, cost.costId);

                Console.WriteLine($"[Purchase] {itemId} Custo: {cost.costValue} {cost.costId}");

            }, save: true);

            if (type == ItemHelper.ItemType.Contract)
            {
                var contract = _locate.LocateContract(game, itemId);

                await _game.Mutate(game =>
                {
                    contract.UseState = UnlockHelper.ContractState.InUse;
                    stage.ActiveContracts ??= new Dictionary<string, int>(StringComparer.Ordinal);
                    stage.ActiveContracts[contract.Id] = (stage.ActiveContracts.TryGetValue(contract.Id, out var q) ? q : 0) + 1;

                    stage.lockedContractLevel.Add(contract.Level);
                    expansion.inUseContracts.Add(contract.Id);

                }, save: true);
            }

            await _effect.ApplyEffect(type, itemId, stageId);

            _ui.NavMenuControl(itemId);
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