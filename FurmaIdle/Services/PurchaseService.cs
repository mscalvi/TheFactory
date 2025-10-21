using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Resources;
using static FurmaIdle.Helpers.EffectHelper;
using static FurmaIdle.Helpers.PricingHelper;
using static FurmaIdle.Services.PurchaseService;

namespace FurmaIdle.Services
{
    public interface IPurchaseService
    {
        Task Purchase(ItemHelper.ItemType type, string itemId, string stageId, GameModel game);

        bool CanAfford(ItemHelper.ItemType type, string itemId, string stageId, GameModel game);
    }

    public sealed class PurchaseService : IPurchaseService
    {
        private readonly ICurrentGameService _game;
        private readonly IIncomeService _income;
        private readonly IUnlockService _unlock;
        private readonly ILocateService _locate;

        public sealed record CostLine(string CurrencyGroup, string CurrencyId, double Amount);

        public PurchaseService(ICurrentGameService Game, IIncomeService Income, IUnlockService Unlock, ILocateService Locate)
        {
            _game = Game;
            _income = Income;
            _unlock = Unlock;
            _locate = Locate;
        }

        // Purchase
        public async Task Purchase(ItemHelper.ItemType type, string itemId, string stageId, GameModel game)
        {
            var stage = _locate.LocateStage(_game.CurrentGame, stageId);

            await _game.Mutate(game =>
            {
                Console.WriteLine("[Purchase] Selecionando Custo");
                var cost = ComputeCost(type, itemId, stageId, game);

                Console.WriteLine($"[Purchase] Custo: {cost.costValue} {cost.costId}");
                // Pagar o Custo
                ApplyDebit(game.ExpeditionStats, cost.costValue, cost.costId);
                ApplyStats(game.ExpansionStats, game.GameStats, cost.costValue, cost.costId);

                Console.WriteLine($"[Purchase] Aplicando Compra");
                // Aplica o Efeito
                ApplyPurchaseEffect(type, itemId, stageId, game);
                
            }, save: true);
        }
        public bool CanAfford(ItemHelper.ItemType type, string itemId, string stageId, GameModel game)
        {
            var stats = game.ExpeditionStats ?? throw new InvalidOperationException("ExpeditionStats indisponível.");

            var cost = ComputeCost(type, itemId, stageId, game);

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

        private async Task ApplyPurchaseEffect(ItemHelper.ItemType type, string itemId, string stageId, GameModel game)
        {
            if (type == ItemHelper.ItemType.Upgrade)
            {
                Console.WriteLine("[Purchase] Aplicanto Efeitos do Update");
                var upgrade = _locate.LocateUpgrade(game, itemId);

                await _unlock.UnlockUpgrade(upgrade.Id);
                
                switch (upgrade.EffectType)
                {
                    case EffectHelper.EffectType.CharacterUnlock:
                        await _unlock.UnlockCharacter(upgrade.TargetId);
                        break;
                    case EffectHelper.EffectType.ContractUnlock:
                        await _unlock.UnlockContract(upgrade.TargetId);
                        break;
                    case EffectHelper.EffectType.ExpansionUnlock:
                        await _unlock.UnlockExpansion(upgrade.TargetId);
                        break;
                    case EffectHelper.EffectType.KnowledgeUnlock:
                        await _unlock.UnlockKnowledge(upgrade.TargetId);
                        break;
                    case EffectHelper.EffectType.LocalUnlock:
                        await _unlock.UnlockLocal(upgrade.TargetId);
                        break;
                    case EffectHelper.EffectType.ResourceUnlock:
                        await _unlock.UnlockResource(upgrade.TargetId);
                        break;
                    case EffectHelper.EffectType.StageUnlock:
                        await _unlock.UnlockStage(upgrade.TargetId);
                        break;
                    case EffectHelper.EffectType.TechUnlock:
                        await _unlock.UnlockTech(upgrade.TargetId);
                        break;
                }
            }
            if (type == ItemHelper.ItemType.Contract)
            {
                Console.WriteLine("[Purchase] Aplicanto Efeitos do Contrato");
            }
            if (type == ItemHelper.ItemType.Expedition)
            {
                Console.WriteLine("[Purchase] Aplicanto Efeitos da Expedição");
            }
            if (type == ItemHelper.ItemType.Expansion)
            {
                Console.WriteLine("[Purchase] Aplicanto Efeitos da Expansão");
            }
            if (type == ItemHelper.ItemType.Specialty)
            {
                Console.WriteLine("[Purchase] Aplicanto Efeitos da Especialidade");
            }
        }

        private (long costValue, string costId) ComputeCost(ItemHelper.ItemType type, string itemId, string stageId, GameModel game)
        {
            long costValue = 0;
            string costId = string.Empty;
            long costBase = 0;
            double costCurve = 0;
            double? costAddFactor = 0;
            double? costMultFactor = 1;
            double costFactorValue = 1;

            switch (type)
            {
                case ItemHelper.ItemType.Specialty:
                    {
                        var specialty = _locate.LocateSpecialty(_game.CurrentGame, itemId);
                        costValue = specialty.Cost;
                        costId = specialty.PricingId;
                        break;
                    }

                case ItemHelper.ItemType.Upgrade:
                    {
                        var up = _locate.LocateUpgrade(_game.CurrentGame, itemId);

                        var entry = PricingCost.Get(up.PricingId);

                        costId = entry.CostCoinId;

                        costBase = entry.CostBase;
                        costCurve = entry.CostCurve;

                        if (entry.CostFactor != null)
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
                                costAddFactor = entry.CostFactorCurve * costFactorValue;
                                costMultFactor = 1;
                            }

                            if (entry.CostFactorType == PricingHelper.CostFactorType.Multiplicative)
                            {
                                costAddFactor = 0;
                                costMultFactor = entry.CostFactorCurve * costFactorValue;
                            }
                        }

                        double baseVal = costBase;
                        double curve = costCurve;
                        double addF = costAddFactor ?? 0d;
                        double multF = costMultFactor ?? 1d;

                        double raw = (baseVal + addF) * curve * multF;
                        costValue = (long)Math.Ceiling(raw);

                        break;
                    }

                case ItemHelper.ItemType.Contract:
                    {
                        var contract = _locate.LocateContract(_game.CurrentGame, itemId);

                        var entry = PricingCost.Get(contract.PricingId);

                        costId = entry.CostCoinId;

                        costBase = entry.CostBase;
                        costCurve = entry.CostCurve;

                        double baseVal = costBase;
                        double curve = costCurve;

                        double raw = baseVal * curve;
                        costValue = (long)Math.Ceiling(raw);

                        break;
                    }

                default:
                    break;
            }

            return (costValue, costId);
        }
        private static void AddOrSet(Dictionary<string, long> dict, string id, long delta)
        {
            if (!dict.TryGetValue(id, out var v)) v = 0L;
            var nv = v + delta;
            if (nv < 0) nv = 0; // proteção mínima
            dict[id] = nv;
        }
        private static long GetOrZero(Dictionary<string, long> dict, string id)
                    => dict is not null && dict.TryGetValue(id, out var v) ? v : 0L;
    }
}