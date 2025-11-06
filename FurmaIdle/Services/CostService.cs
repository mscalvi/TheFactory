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

        public bool CanAfford(ItemHelper.ItemType type, string itemId, string stageId)
        {
            var game = _game.CurrentGame;
            var expansion = _locate.LocateExpansion(game, game.CurrentExpansionId);
            var stage = _locate.LocateStage(game, stageId);
            var stats = new StatsModel();

            var cost = ComputeCost(type, itemId, stageId);

            char costGroup = cost.costId?[0] ?? '\0';

            switch (costGroup)
            {
                case 'm':
                    stats = stage.ExpeditionStats;
                    var haveCoins = GetOrZero(stats.Coins, cost.costId);
                    if (cost.costValue > haveCoins)
                    {
                        return false;
                    }
                    return true;

                case 'r':
                    stats = expansion.ExpansionStats;
                    var haveResources = GetOrZero(stats.Resources, cost.costId);
                    if (cost.costValue > haveResources)
                    {
                        return false;
                    }
                    return true;

                case 'k':
                    stats = expansion.ExpansionStats;
                    var haveKnowledge = GetOrZero(stats.Knowledge, cost.costId);
                    if (cost.costValue > haveKnowledge)
                    {
                        return false;
                    }
                    return true;

                default:
                    return false;
            }
        }

        public (long costValue, string costId) ComputeCost(ItemHelper.ItemType type, string itemId, string stageId)
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

                        raw = 0;

                        if (upgrade.MaxBuy == 1)
                        {
                            raw = (costBase + costAddFactor) * costMultFactor;
                        }
                        else
                        {
                            raw = (costBase + costAddFactor) * Math.Pow(upgrade.ActualBuy + 1, costCurve) * costMultFactor;
                        }

                        costValue = (long)Math.Ceiling(raw);

                        break;
                    }

                case ItemHelper.ItemType.Contract:
                    {
                        var contract = _locate.LocateContract(game, itemId);
                        var stage = _locate.LocateStage(game, stageId);
                        entry = PricingCost.Get(contract.PricingId);

                        stage.ActiveContracts.TryGetValue(itemId, out var quantity);

                        costId = entry.CostCoinId;
                        costBase = entry.CostBase;
                        costCurve = entry.CostCurve;

                        costMultFactor *= costModifiers.MultMod;
                        costAddFactor += costModifiers.AddMod;

                        raw = (costBase + costAddFactor) * Math.Pow(quantity + 1, costCurve) * costMultFactor;
                        costValue = (long)Math.Ceiling(raw);

                        break;
                    }

                case ItemHelper.ItemType.Expansion:
                    {
                        var expansion = _locate.LocateExpansion(game, itemId);

                        entry = PricingCost.Get(expansion.PricingId);

                        costId = entry.CostCoinId;

                        costBase = entry.CostBase;
                        costCurve = entry.CostCurve;

                        costFactorValue = 0;

                        foreach (var activeExpansions in game.Expansions)
                        {
                            var previousexpansion = _locate.LocateExpansion(game, activeExpansions.Key);
                            if (expansion.State == UnlockHelper.State.Unlocked)
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
                    var tech = _locate.LocateTech(game, itemId);

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

            return (costValue, costId);
        }

        private static long GetOrZero(Dictionary<string, long> dict, string id)
                    => dict is not null && dict.TryGetValue(id, out var v) ? v : 0L;

        private int GetCostFactor(PricingCost.Entry entry, string stageId, string itemId)
        {
            var game = _game.CurrentGame;

            int costFactorValue = 1;

            UpgradeModel upgrade = new UpgradeModel();
            TechModel tech = new TechModel();
            ContractModel contract = new ContractModel();

            if (itemId.StartsWith("u"))
            {
                upgrade = _locate.LocateUpgrade(game, itemId);
            }
            if (itemId.StartsWith("t"))
            {
                tech = _locate.LocateTech(game, itemId);
            }
            if (itemId.StartsWith("r"))
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
    }
}
