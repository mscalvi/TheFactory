using FurmaIdle.Helpers;
using FurmaIdle.Models;
using static FurmaIdle.Helpers.EffectHelper;
using static FurmaIdle.Helpers.PricingHelper;

namespace FurmaIdle.Services
{
    public interface ICostService
    {
        bool CanAfford(ItemHelper.ItemType type, string itemId, string stageId);
        (long costValue, string costId) ComputeCost(ItemHelper.ItemType type, string itemId, string stageId);

        bool CanAfford(ItemHelper.ItemType type, string itemId, string stageId, int quantity);

        (long totalCost, string costId) ComputeCost(
            ItemHelper.ItemType type,
            string itemId,
            string stageId,
            int quantity
        );

        (int maxQuantity, long totalCost, string costId) ComputeMaxAffordable(
            ItemHelper.ItemType type,
            string itemId,
            string stageId
        );
    }

    public sealed class CostService : ICostService
    {
        private readonly ICurrentGameService _game;
        private readonly ILocateService _locate;
        private readonly IUiLogService _log;
        private readonly IModifierService _modifier;

        public CostService(ICurrentGameService Game, IUiLogService Log, ILocateService Locate, IModifierService modifier)
        {
            _game = Game;
            _locate = Locate;
            _log = Log;
            _modifier = modifier;
        }

        #region CanAfford (x1 / xN)

        public bool CanAfford(ItemHelper.ItemType type, string itemId, string stageId)
        {
            var (costValue, costId) = ComputeCost(type, itemId, stageId);
            if (costValue <= 0 || string.IsNullOrEmpty(costId))
                return false;

            return HasEnough(costId, costValue, stageId);
        }

        public bool CanAfford(ItemHelper.ItemType type, string itemId, string stageId, int quantity)
        {
            if (quantity <= 0) return false;

            var (totalCost, costId) = ComputeCost(type, itemId, stageId, quantity);
            if (totalCost <= 0 || string.IsNullOrEmpty(costId))
                return false;

            return HasEnough(costId, totalCost, stageId);
        }

        private bool HasEnough(string costId, long needed, string stageId)
        {
            var game = _game.CurrentGame;
            var expansion = _locate.LocateExpansion(game, game.CurrentExpansionId);
            var stage = _locate.LocateStage(game, stageId);

            StatsModel stats;
            var group = costId[0];

            switch (group)
            {
                case 'm':
                    stats = stage.ExpeditionStats;
                    return needed <= GetOrZero(stats.Coins, costId);

                case 'r':
                    stats = expansion.ExpansionStats;
                    return needed <= GetOrZero(stats.Resources, costId);

                case 'k':
                    stats = expansion.ExpansionStats;
                    return needed <= GetOrZero(stats.Knowledge, costId);

                default:
                    return false;
            }
        }

        #endregion

        #region ComputeCost (x1)
        public (long costValue, string costId) ComputeCost(ItemHelper.ItemType type, string itemId, string stageId)
        {
            var game = _game.CurrentGame;
            var stage = _locate.LocateStage(game, stageId);

            int baseQty = 0;

            switch (type)
            {
                case ItemHelper.ItemType.Contract:
                    stage.ActiveContracts?.TryGetValue(itemId, out baseQty);
                    break;

                case ItemHelper.ItemType.Upgrade:
                    var up = _locate.LocateUpgrade(game, itemId);
                    baseQty = up.ActualBuy;
                    break;

                    // Specialty / Expansion / Tech não dependem de quantidade
            }

            return ComputeCostForStep(type, itemId, stageId, baseQty);
        }

        #endregion

        #region ComputeCost (xN)
        public (long totalCost, string costId) ComputeCost(
            ItemHelper.ItemType type,
            string itemId,
            string stageId,
            int quantity)
        {
            if (quantity <= 0)
                return (0, string.Empty);

            var game = _game.CurrentGame;
            var stage = _locate.LocateStage(game, stageId);

            int baseQty = 0;

            switch (type)
            {
                case ItemHelper.ItemType.Contract:
                    stage.ActiveContracts?.TryGetValue(itemId, out baseQty);
                    break;

                case ItemHelper.ItemType.Upgrade:
                    var up = _locate.LocateUpgrade(game, itemId);
                    baseQty = up.ActualBuy;
                    break;
            }

            long total = 0;
            string? costId = null;

            for (int i = 0; i < quantity; i++)
            {
                // absoluteIndex = quantas unidades tem ANTES da compra
                int absoluteIndex = baseQty + i;

                var (stepCost, stepId) = ComputeCostForStep(type, itemId, stageId, absoluteIndex);
                if (stepCost <= 0)
                    break;

                costId ??= stepId;
                total += stepCost;
            }

            return (total, costId ?? string.Empty);
        }

        #endregion

        #region ComputeMaxAffordable (xMax)
        public (int maxQuantity, long totalCost, string costId) ComputeMaxAffordable(
            ItemHelper.ItemType type,
            string itemId,
            string stageId)
        {
            var game = _game.CurrentGame;
            var stage = _locate.LocateStage(game, stageId);

            // Descobre costId
            var (singleCost, baseCostId) = ComputeCost(type, itemId, stageId);
            if (string.IsNullOrEmpty(baseCostId))
                return (0, 0, string.Empty);

            // Quanto temos disponível dessa moeda
            var expansion = _locate.LocateExpansion(game, game.CurrentExpansionId);
            long available = 0;
            StatsModel stats;
            char group = baseCostId[0];

            switch (group)
            {
                case 'm':
                    stats = stage.ExpeditionStats;
                    available = GetOrZero(stats.Coins, baseCostId);
                    break;

                case 'r':
                    stats = expansion.ExpansionStats;
                    available = GetOrZero(stats.Resources, baseCostId);
                    break;

                case 'k':
                    stats = expansion.ExpansionStats;
                    available = GetOrZero(stats.Knowledge, baseCostId);
                    break;

                default:
                    return (0, 0, string.Empty);
            }

            int baseQty = 0;

            switch (type)
            {
                case ItemHelper.ItemType.Contract:
                    stage.ActiveContracts?.TryGetValue(itemId, out baseQty);
                    break;

                case ItemHelper.ItemType.Upgrade:
                    var up = _locate.LocateUpgrade(game, itemId);
                    baseQty = up.ActualBuy;
                    break;
            }

            int qty = 0;
            long total = 0;

            while (true)
            {
                int absoluteIndex = baseQty + qty;
                var (stepCost, _) = ComputeCostForStep(type, itemId, stageId, absoluteIndex);

                if (stepCost <= 0)
                    break;

                if (total + stepCost > available)
                    break;

                total += stepCost;
                qty++;

                if (qty > 1_000_000)
                    break;
            }

            return (qty, total, baseCostId);
        }
        #endregion

        #region Núcleo de cálculo: ComputeCostForStep
        private (long costValue, string costId) ComputeCostForStep(
            ItemHelper.ItemType type,
            string itemId,
            string stageId,
            int absoluteIndex)
        {
            var game = _game.CurrentGame;

            string costId = string.Empty;
            long costValue = 0;
            long costBase = 0;
            double costCurve = 1;
            double costAddFactor = 0;
            double costMultFactor = 1;
            double costFactorValue = 1;

            var costModifiers = _modifier.GetModifiers(type, itemId, stageId, EffectSupertype.Cost);
            var entry = new PricingCost.Entry();
            double raw = 0;

            switch (type)
            {
                case ItemHelper.ItemType.Specialty:
                    {
                        var specialty = _locate.LocateSpecialty(game, itemId);

                        if (absoluteIndex > 0)
                            return (0, specialty.PricingId);

                        costAddFactor = costModifiers.AddMod;
                        costMultFactor = costModifiers.MultMod;

                        raw = (specialty.Cost + costAddFactor) * costMultFactor;
                        costValue = (long)Math.Ceiling(raw);

                        costId = specialty.PricingId;
                        break;
                    }

                case ItemHelper.ItemType.Upgrade:
                    {
                        var upgrade = _locate.LocateUpgrade(game, itemId);

                        entry = PricingCost.Get(upgrade.PricingId);

                        costId = entry.CostCoinId;
                        costBase = entry.CostBase;
                        costCurve = entry.CostCurve;

                        if (entry.CostFactor != PricingHelper.CostFactor.None)
                        {
                            costFactorValue = GetCostFactor(entry, stageId, itemId);
                            costMultFactor *= Math.Pow(costFactorValue, entry.CostFactorCurve);
                        }

                        costMultFactor *= costModifiers.MultMod;
                        costAddFactor += costModifiers.AddMod;

                        if (upgrade.MaxBuy == 1)
                        {
                            if (absoluteIndex > 0)
                                return (0, costId);

                            raw = (costBase + costAddFactor) * costMultFactor;
                        }
                        else
                        {
                            raw = (costBase + costAddFactor)
                                  * Math.Pow(absoluteIndex + 1, costCurve)
                                  * costMultFactor;
                        }

                        costValue = (long)Math.Ceiling(raw);
                        break;
                    }

                case ItemHelper.ItemType.Contract:
                    {
                        var contract = _locate.LocateContract(game, itemId);
                        var stage = _locate.LocateStage(game, stageId);

                        entry = PricingCost.Get(contract.PricingId);

                        costId = entry.CostCoinId;
                        costBase = entry.CostBase;
                        costCurve = entry.CostCurve;

                        costMultFactor *= costModifiers.MultMod;
                        costAddFactor += costModifiers.AddMod;

                        raw = (costBase + costAddFactor)
                              * Math.Pow(absoluteIndex + 1, costCurve)
                              * costMultFactor;

                        costValue = (long)Math.Ceiling(raw);
                        break;
                    }

                case ItemHelper.ItemType.Expansion:
                    {
                        var expansion = _locate.LocateExpansion(game, itemId);

                        if (absoluteIndex > 0 && expansion.State == UnlockHelper.State.Unlocked)
                            return (0, string.Empty);

                        entry = PricingCost.Get(expansion.PricingId);

                        costId = entry.CostCoinId;
                        costBase = entry.CostBase;
                        costCurve = entry.CostCurve;

                        costFactorValue = 0;

                        foreach (var activeExpansions in game.Expansions)
                        {
                            var previousexpansion = _locate.LocateExpansion(game, activeExpansions.Key);
                            if (previousexpansion.State == UnlockHelper.State.Unlocked)
                            {
                                costFactorValue++;
                            }
                        }

                        costMultFactor = Math.Pow(costFactorValue, entry.CostFactorCurve);
                        costMultFactor *= costModifiers.MultMod;
                        costAddFactor += costModifiers.AddMod;

                        raw = (costBase + costAddFactor) * costMultFactor;
                        costValue = (long)Math.Ceiling(raw);

                        break;
                    }

                case ItemHelper.ItemType.Tech:
                    {
                        var tech = _locate.LocateTech(game, itemId);

                        if (absoluteIndex > 0 && tech.State == UnlockHelper.State.Unlocked)
                            return (0, string.Empty);

                        entry = PricingCost.Get(tech.PricingId);

                        costId = entry.CostCoinId;
                        costBase = entry.CostBase;
                        costCurve = entry.CostCurve;

                        costFactorValue = 0;

                        costMultFactor = Math.Pow(costFactorValue, entry.CostFactorCurve);
                        costMultFactor *= costModifiers.MultMod;
                        costAddFactor += costModifiers.AddMod;

                        raw = (costBase + costAddFactor) * costMultFactor;
                        costValue = (long)Math.Ceiling(raw);

                        break;
                    }
            }

            return (costValue, costId);
        }
        #endregion

        #region Helpers
        private static long GetOrZero(Dictionary<string, long> dict, string id)
            => dict is not null && dict.TryGetValue(id, out var v) ? v : 0L;

        private int GetCostFactor(PricingCost.Entry entry, string stageId, string itemId)
        {
            var game = _game.CurrentGame;

            int costFactorValue = 1;

            UpgradeModel upgrade = new();
            TechModel tech = new();
            ContractModel contract = new();

            if (itemId.StartsWith("u"))
            {
                upgrade = _locate.LocateUpgrade(game, itemId);
            }
            if (itemId.StartsWith("t"))
            {
                tech = _locate.LocateTech(game, itemId);
            }
            if (itemId.StartsWith("c"))
            {
                contract = _locate.LocateContract(game, itemId);
            }

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

                case CostFactor.Level:
                    if (itemId.StartsWith("u"))
                    {
                        costFactorValue *= upgrade.Level;
                    }
                    if (itemId.StartsWith("t"))
                    {
                        costFactorValue *= tech.Level;
                    }
                    if (itemId.StartsWith("c"))
                    {
                        costFactorValue *= contract.Level;
                    }
                    break;

                default:
                    break;
            }

            return costFactorValue;
        }

        #endregion
    }
}
