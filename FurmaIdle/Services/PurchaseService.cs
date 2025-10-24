using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Diagnostics.Contracts;
using System.Resources;
using static FurmaIdle.Helpers.EffectHelper;
using static FurmaIdle.Helpers.PricingHelper;
using static FurmaIdle.Services.PurchaseService;

namespace FurmaIdle.Services
{
    public interface IPurchaseService
    {
        Task Purchase(ItemHelper.ItemType type, string itemId, string stageId);

        bool CanAfford(ItemHelper.ItemType type, string itemId, string stageId);
    }

    public sealed class PurchaseService : IPurchaseService
    {
        private readonly ICurrentGameService _game;
        private readonly ILocateService _locate;
        private readonly IUiLogService _log;
        private readonly IEffectService _effect;

        public sealed record CostLine(string CurrencyGroup, string CurrencyId, double Amount);

        public PurchaseService(ICurrentGameService Game, IUiLogService Log, ILocateService Locate, IExpeditionService expedition, IEffectService effect)
        {
            _game = Game;
            _locate = Locate;
            _log = Log;
            _effect = effect;
        }

        // Purchase
        public async Task Purchase(ItemHelper.ItemType type, string itemId, string stageId)
        {
            var game = _game.CurrentGame;
            var stage = _locate.LocateStage(game, stageId);

            var cost = ComputeCost(type, itemId, stageId);

            await _game.Mutate(game =>
            {
                Console.WriteLine($"[Purchase] {itemId} Custo: {cost.costValue} {cost.costId}");
                ApplyDebit(game.ExpeditionStats, cost.costValue, cost.costId);
                ApplyStats(game.ExpansionStats, game.GameStats, cost.costValue, cost.costId);

            }, save: true);

            await _effect.ApplyEffect(type, itemId, stageId);            
        }
        public bool CanAfford(ItemHelper.ItemType type, string itemId, string stageId)
        {
            var game = _game.CurrentGame;
            var stats = game.ExpeditionStats ?? throw new InvalidOperationException("ExpeditionStats indisponível.");

            var cost = ComputeCost(type, itemId, stageId);

            char costGroup = cost.costId?[0] ?? '\0';

            switch (costGroup)
            {
                case 'm':
                    var haveCoins = GetOrZero(stats.Coins, cost.costId);
                    if (haveCoins < cost.costValue)
                    {
                        return false;
                    }
                    return true;

                case 'r':
                    var haveResources = GetOrZero(stats.Resources, cost.costId);
                    if (haveResources < cost.costValue)
                    {
                        return false;
                    }
                    return true;

                case 'k':
                    var haveKnowledge = GetOrZero(stats.Knowledge, cost.costId);
                    if (haveKnowledge < cost.costValue)
                    {
                        return false;
                    }
                    return true;

                default:
                    return false;
            }
        }
        private static void ApplyDebit(StatsModel stats, long cost, string costId)
        {
            char costGroup = costId?[0] ?? '\0';

            switch (costGroup)
            {
                case 'm':
                    AddOrSet(stats.Coins, costId, -cost);
                    AddOrSet(stats.CoinsSpent, costId, +cost);
                    break;

                case 'r':
                    AddOrSet(stats.Resources, costId, -cost);
                    AddOrSet(stats.ResourcesSpent, costId, +cost);
                    break;

                case 'k':
                    AddOrSet(stats.Knowledge, costId, -cost);
                    AddOrSet(stats.KnowledgeSpent, costId, +cost);
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

        private (long costValue, string costId) ComputeCost(ItemHelper.ItemType type, string itemId, string stageId)
        {
            var game = _game.CurrentGame;

            long costValue = 0;
            string costId = string.Empty;
            long costBase = 0;
            double costCurve = 1;
            double costAddFactor = 0;
            double costMultFactor = 1;
            double costFactorValue = 1;

            switch (type)
            {
                case ItemHelper.ItemType.Specialty:
                    {
                        var specialty = _locate.LocateSpecialty(game, itemId);
                        double raw = specialty.Cost * specialty.PriceFactor;
                        costValue = (long)Math.Ceiling(raw);
                        costId = specialty.PricingId;
                        break;
                    }

                case ItemHelper.ItemType.Upgrade:
                    {
                        var up = _locate.LocateUpgrade(game, itemId);

                        var entry = PricingCost.Get(up.PricingId);

                        costId = entry.CostCoinId;

                        costBase = entry.CostBase;
                        costCurve = entry.CostCurve;

                        if (entry.CostFactor != PricingHelper.CostFactor.None)
                        {
                            switch (entry.CostFactor)
                            {
                                case CostFactor.CharactersUnlocked:
                                    foreach (var activeCharacter in game.Characters)
                                    {
                                        var character = _locate.LocateCharacter(game, activeCharacter.Key);
                                        if (character.State == UnlockHelper.State.Unlocked)
                                        {
                                            costFactorValue++;
                                        }
                                    }
                                    break;

                                case CostFactor.KnowledgesUnlocked:
                                    foreach (var activeKnowledge in game.Knowledges)
                                    {
                                        var knowledge = _locate.LocateKnowledge(game, activeKnowledge.Key);
                                        if (knowledge.State == UnlockHelper.State.Unlocked)
                                        {
                                            costFactorValue++;
                                        }
                                    }
                                    break;

                                case CostFactor.ResourcesUnlocked:
                                    foreach (var activeResource in game.Resources)
                                    {
                                        var resource = _locate.LocateResource(game, activeResource.Key);
                                        if (resource.State == UnlockHelper.State.Unlocked)
                                        {
                                            costFactorValue++;
                                        }
                                    }
                                    break;

                                case CostFactor.LocalsUnlocked:
                                    foreach (var activeLocal in game.Locals)
                                    {
                                        var local = _locate.LocateLocal(game, activeLocal.Key);
                                        if (local.State == UnlockHelper.State.Unlocked)
                                        {
                                            costFactorValue++;
                                        }
                                    }
                                    break;

                                case CostFactor.ExpansionsUnlocked:
                                    foreach (var activeExpansions in game.Expansions)
                                    {
                                        var expansion = _locate.LocateExpansion(game, activeExpansions.Key);
                                        if (expansion.State == UnlockHelper.State.Unlocked)
                                        {
                                            costFactorValue++;
                                        }
                                    }
                                    break;

                                case CostFactor.PartySize:
                                    var stage = _locate.LocateStage(game, stageId);
                                    costFactorValue = stage?.PartySizeActual ?? 0;
                                    break;

                                default:
                                    break;
                            }

                            if (entry.CostFactorType == PricingHelper.CostFactorType.Additive)
                            {
                                costAddFactor = Math.Pow(entry.CostFactorCurve + costFactorValue, up.ActualBuy);
                                costMultFactor = 1;
                            }

                            if (entry.CostFactorType == PricingHelper.CostFactorType.Multiplicative)
                            {
                                costAddFactor = 0;
                                costMultFactor = Math.Pow(entry.CostFactorCurve * costFactorValue, up.ActualBuy) ;
                            }
                        }

                        double baseVal = costBase;
                        double curve = costCurve;
                        double addF = costAddFactor;
                        double multF = costMultFactor;

                        double raw = (baseVal + addF) * Math.Pow(curve, up.ActualBuy) * multF;
                        costValue = (long)Math.Ceiling(raw);

                        break;
                    }

                case ItemHelper.ItemType.Contract:
                    {
                        var contract = _locate.LocateContract(game, itemId);

                        var entry = PricingCost.Get(contract.PricingId);

                        var stage = _locate.LocateStage(game, stageId);

                        stage.ActiveContracts.TryGetValue(itemId, out var quantity);

                        costId = entry.CostCoinId;

                        costBase = entry.CostBase;
                        costCurve = entry.CostCurve;

                        if (entry.CostFactor != PricingHelper.CostFactor.None)
                        {
                            if (entry.CostFactorType == PricingHelper.CostFactorType.Additive)
                            {
                                costAddFactor = Math.Pow(entry.CostFactorCurve + costFactorValue + contract.PriceFactor, quantity);
                                costMultFactor = 1;
                            }

                            if (entry.CostFactorType == PricingHelper.CostFactorType.Multiplicative)
                            {
                                costAddFactor = 0;
                                costMultFactor = Math.Pow(entry.CostFactorCurve * costFactorValue * contract.PriceFactor, quantity);
                            }
                        }

                        double baseVal = costBase;
                        double curve = costCurve;
                        double addF = costAddFactor;
                        double multF = costMultFactor;

                        double raw = (baseVal + addF) * Math.Pow(curve, quantity) * multF;
                        costValue = (long)Math.Ceiling(raw);

                        break;
                    }
            }

            return (costValue, costId);
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