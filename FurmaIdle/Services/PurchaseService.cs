using FurmaIdle.Data;
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
        bool CanAfford(ItemHelper.ItemType type, string itemId, string stageId);
        (long costValue, string costId) GetCurrentCost(ItemHelper.ItemType type, string itemId, string stageId);


        Task PurchaseTech(string itemId);
        bool CanAffordTech(string itemId);
    }

    public sealed class PurchaseService : IPurchaseService
    {
        private readonly ICurrentGameService _game;
        private readonly ILocateService _locate;
        private readonly IUiLogService _log;
        private readonly IEffectService _effect;
        private readonly IUiService _ui;

        public sealed record CostLine(string CurrencyGroup, string CurrencyId, double Amount);

        public PurchaseService(ICurrentGameService Game, IUiLogService Log, ILocateService Locate, IEffectService effect, IUiService ui)
        {
            _game = Game;
            _locate = Locate;
            _log = Log;
            _effect = effect;
            _ui = ui;
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
                if(type != ItemHelper.ItemType.Knowledge)
                {
                    ApplyDebit(stage.ExpeditionStats, cost.costValue, cost.costId);
                } else
                {
                    ApplyDebit(game.ExpansionStats, cost.costValue, cost.costId);
                }
                ApplyStats(game.ExpansionStats, game.GameStats, cost.costValue, cost.costId);

            }, save: true);

            if (type == ItemHelper.ItemType.Contract)
            {
                var contract = _locate.LocateContract(game, itemId);

                await _game.Mutate(game =>
                {
                    contract.UseState = UnlockHelper.ContractState.InUse;
                    stage.ActiveContracts ??= new Dictionary<string, int>(StringComparer.Ordinal);
                    stage.ActiveContracts[contract.Id] = (stage.ActiveContracts.TryGetValue(contract.Id, out var q) ? q : 0) + 1;

                    stage.lockedContracts.Add(contract.Level);
                }, save: true);

                // Invocar effect para desbloquear melhorias relacionadas a número de contratos ativos?
            }

            await _effect.ApplyEffect(type, itemId, stageId);

            _ui.NavMenuControl(itemId);
        }
        public (long costValue, string costId) GetCurrentCost(ItemHelper.ItemType type, string itemId, string stageId)
            => ComputeCost(type, itemId, stageId);

        public bool CanAfford(ItemHelper.ItemType type, string itemId, string stageId)
        {
            var game = _game.CurrentGame;
            var stage = _locate.LocateStage(game, stageId);
            var stats = stage.ExpeditionStats ?? throw new InvalidOperationException("ExpeditionStats indisponível.");

            var cost = ComputeCost(type, itemId, stageId);

            char costGroup = cost.costId?[0] ?? '\0';

            switch (costGroup)
            {
                case 'm':
                    var haveCoins = GetOrZero(stats.Coins, cost.costId);
                    if (cost.costValue > haveCoins)
                    {
                        return false;
                    }
                    return true;

                case 'r':
                    var haveResources = GetOrZero(stats.Resources, cost.costId);
                    if (cost.costValue > haveResources)
                    {
                        return false;
                    }
                    return true;

                case 'k':
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

            string costId = string.Empty;
            long costValue = 0;
            long costBase = 0;
            double costCurve = 1;
            double costAddFactor = 0;
            double costMultFactor = 1;
            double costFactorValue = 1;

            var modifiers = GetCostModifiers(game, type, itemId);

            switch (type)
            {
                case ItemHelper.ItemType.Specialty:
                    {
                        var specialty = _locate.LocateSpecialty(game, itemId);

                        costAddFactor = modifiers.AddMod;
                        costMultFactor = modifiers.MultMod;

                        double raw = (specialty.Cost + costAddFactor) * costMultFactor;

                        costValue = (long)Math.Ceiling(raw);

                        costId = specialty.PricingId;
                        break;
                    }

                case ItemHelper.ItemType.Upgrade:
                    {
                        var upgrade = _locate.LocateUpgrade(game, itemId);

                        var entry = PricingCost.Get(upgrade.PricingId);

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
                                    costFactorValue = stage.StartPartySize;
                                    foreach(var modifier in stage.Modifiers)
                                    {
                                        if(modifier.Type == EffectType.PartyCapSize)
                                        {
                                            costFactorValue += modifier.Value;
                                        }
                                    }
                                    break;

                                default:
                                    break;
                            }

                            if (entry.CostFactorType == PricingHelper.CostFactorType.Additive)
                            {
                                costAddFactor = Math.Pow(costFactorValue, entry.CostFactorCurve);
                            }

                            if (entry.CostFactorType == PricingHelper.CostFactorType.Multiplicative)
                            {
                                costMultFactor = Math.Pow(costFactorValue, entry.CostFactorCurve) ;
                            }
                        }

                        costMultFactor *= modifiers.MultMod;
                        costAddFactor += modifiers.AddMod;

                        double raw = 0;

                        if(upgrade.MaxBuy == 1)
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
                        var entry = PricingCost.Get(contract.PricingId);

                        stage.ActiveContracts.TryGetValue(itemId, out var quantity);

                        var nextQnt = quantity + 1;
                        costId = entry.CostCoinId;
                        costBase = entry.CostBase;
                        costCurve = entry.CostCurve;

                        if (entry.CostFactor != PricingHelper.CostFactor.None)
                        {
                            if (entry.CostFactorType == PricingHelper.CostFactorType.Additive)
                            {
                                costAddFactor = Math.Pow(costFactorValue, entry.CostFactorCurve);
                            }

                            if (entry.CostFactorType == PricingHelper.CostFactorType.Multiplicative)
                            {
                                costMultFactor = Math.Pow(costFactorValue, entry.CostFactorCurve);
                            }
                        }

                        costMultFactor *= modifiers.MultMod;
                        costAddFactor += modifiers.AddMod;

                        double raw = (costBase + costAddFactor) * Math.Pow(nextQnt, costCurve) * costMultFactor;
                        costValue = (long)Math.Ceiling(raw);

                        break;
                    }

                case ItemHelper.ItemType.Expansion:
                    {
                        var expansion = _locate.LocateExpansion(game, itemId);

                        var entry = PricingCost.Get(expansion.PricingId);

                        costId = entry.CostCoinId;

                        costBase = entry.CostBase;
                        costCurve = entry.CostCurve;

                        costFactorValue = -1;

                        foreach (var activeExpansions in game.Expansions)
                        {
                            var previousexpansion = _locate.LocateExpansion(game, activeExpansions.Key);
                            if (expansion.State == UnlockHelper.State.Unlocked)
                            {
                                costFactorValue++;
                            }
                        }

                        costMultFactor = Math.Pow(costFactorValue, entry.CostFactorCurve);

                        costMultFactor *= modifiers.MultMod;
                        costAddFactor += modifiers.AddMod;

                        double raw = (costBase + costAddFactor) * costMultFactor;
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
        public (double AddMod, double MultMod) GetCostModifiers(GameModel game, ItemHelper.ItemType itemType, string itemId)
        {
            double AddMod = 0;
            double MultMod = 1;

            switch (itemType)
            {
                case ItemHelper.ItemType.Character:
                    var character = _locate.LocateCharacter(game, itemId);
                    foreach (var modifier in character.Modifiers)
                    {
                        if (modifier.Type == EffectType.CharacterCost)
                        {
                            if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                            {
                                AddMod += modifier.Value;
                            }
                            if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                            { 
                                MultMod *= modifier.Value;
                            }
                        }
                    }
                    return (AddMod, MultMod);

                case ItemHelper.ItemType.Contract:
                    var contract = _locate.LocateContract(game, itemId);
                    foreach (var modifier in contract.Modifiers)
                    {
                        if (modifier.Type == EffectType.ContractCost)
                        {
                            if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                            {
                                AddMod += modifier.Value;
                            }
                            if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                            {
                                MultMod *= modifier.Value;
                            }
                        }
                    }
                    return (AddMod, MultMod);

                case ItemHelper.ItemType.Upgrade:
                    var upgrade = _locate.LocateUpgrade(game, itemId);
                    string upgradeKind = upgrade.Id.Length >= 2
                        ? upgrade.Id.Substring(0, 2)
                        : upgrade.Id;

                    if (upgrade.EffectOp == EffectOperation.Unlock)
                    {
                        switch (upgradeKind)
                        {
                            case "uk":
                                var knowledgeupgrade = _locate.LocateKnowledge(game, upgrade.TargetId);
                                foreach (var modifier in knowledgeupgrade.Modifiers)
                                {
                                    if (modifier.Type == EffectType.KnowledgeCost)
                                    {
                                        if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                                        {
                                            AddMod += modifier.Value;
                                        }
                                        if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                                        {
                                            MultMod *= modifier.Value;
                                        }
                                    }
                                }
                                break;
                            case "up":
                                var characterupgrade = _locate.LocateCharacter(game, upgrade.TargetId);
                                foreach (var modifier in characterupgrade.Modifiers)
                                {
                                    if (modifier.Type == EffectType.CharacterCost)
                                    {
                                        if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                                        {
                                            AddMod += modifier.Value;
                                        }
                                        if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                                        {
                                            MultMod *= modifier.Value;
                                        }
                                    }
                                }
                                break;
                        }
                    } else
                    {
                        foreach (var modifier in upgrade.Modifiers)
                        {
                            if (modifier.Type == EffectType.UpgradeCost)
                            {
                                if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                                {
                                    AddMod += modifier.Value;
                                }
                                if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                                {
                                    MultMod *= modifier.Value;
                                }
                            }
                        }
                    }
                    return (AddMod, MultMod);

                case ItemHelper.ItemType.Expedition:
                    var expedition = _locate.LocateExpedition(game, itemId);
                    foreach (var modifier in expedition.Modifiers)
                    {
                        if (modifier.Type == EffectType.ExpeditionCost)
                        {
                            if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                            {
                                AddMod += modifier.Value;
                            }
                            if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                            {
                                MultMod *= modifier.Value;
                            }
                        }
                    }
                    return (AddMod, MultMod);

                case ItemHelper.ItemType.Expansion:
                    var expansion = _locate.LocateExpansion(game, itemId);
                    foreach (var modifier in expansion.Modifiers)
                    {
                        if (modifier.Type == EffectType.ExpansionCost)
                        {
                            if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                            {
                                AddMod += modifier.Value;
                            }
                            if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                            {
                                MultMod *= modifier.Value;
                            }
                        }
                    }
                    return (AddMod, MultMod);

                case ItemHelper.ItemType.Specialty:
                    var specialty = _locate.LocateSpecialty(game, itemId);
                    var stage = _locate.LocateStage(game, game.SelectedStageId);
                    foreach (var characterId in stage.Expedition.PartyIds)
                    {
                        var characterTarget = _locate.LocateCharacter(game, characterId);
                        if(characterTarget.SpecialtyId == specialty.Id)
                        {
                            foreach (var modifier in characterTarget.Modifiers)
                            {
                                if (modifier.Type == EffectType.SpecialtyCost)
                                {
                                    if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value;
                                    }
                                    if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        MultMod *= modifier.Value;
                                    }
                                }
                            }
                        }
                    }
                    foreach (var modifier in specialty.Modifiers)
                    {
                        if (modifier.Type == EffectType.SpecialtyCost)
                        {
                            if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                            {
                                AddMod += modifier.Value;
                            }
                            if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                            {
                                MultMod *= modifier.Value;
                            }
                        }
                    }
                    return (AddMod, MultMod);

                case ItemHelper.ItemType.Knowledge:
                    var knowledge = _locate.LocateKnowledge(game, itemId);
                    foreach (var modifier in knowledge.Modifiers)
                    {
                        if (modifier.Type == EffectType.UpgradeCost)
                        {
                            if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                            {
                                AddMod += modifier.Value;
                            }
                            if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                            {
                                MultMod *= modifier.Value;
                            }
                        }
                    }
                    return (AddMod, MultMod);
            }

            return (AddMod, MultMod);
        }

        // Tech Purchase
        public async Task PurchaseTech(string itemId)
        {
            var game = _game.CurrentGame;

            var cost = ComputeTechCost(itemId);

            await _game.Mutate(game =>
            {
                Console.WriteLine($"[Purchase] {itemId} Custo: {cost.costValue} {cost.costId}");
                ApplyDebit(game.ExpansionStats, cost.costValue, cost.costId);
                ApplyStats(game.ExpansionStats, game.GameStats, cost.costValue, cost.costId);

            }, save: true);

            await _effect.ApplyEffect(ItemHelper.ItemType.Upgrade, itemId, "s00");

            _ui.NavMenuControl(itemId);
        }
        private (long costValue, string costId) ComputeTechCost(string itemId)
        {
            var game = _game.CurrentGame;

            string costId = string.Empty;
            long costValue = 0;
            long costBase = 0;
            double costCurve = 1;
            double costAddFactor = 0;
            double costMultFactor = 1;

            var upgrade = _locate.LocateUpgrade(game, itemId);
            var tech = _locate.LocateTech(game, upgrade.TargetId);

            var techModifiers = GetTechCostModifiers(tech);

            var entry = PricingCost.Get(upgrade.PricingId);

            costId = entry.CostCoinId;

            costBase = entry.CostBase;
            costCurve = entry.CostCurve;

            costMultFactor *= techModifiers.MultMod;
            costAddFactor += techModifiers.AddMod;

            double raw = (costBase + costAddFactor) * Math.Pow(tech.Level, costCurve) * costMultFactor;
            costValue = (long)Math.Ceiling(raw);

            return (costValue, costId);
        }
        public (double AddMod, double MultMod) GetTechCostModifiers(TechModel tech)
        {
            double AddMod = 0;
            double MultMod = 1;

            foreach (var modifier in tech.Modifiers)
            {
                if (modifier.Type == EffectType.CharacterCost)
                {
                    if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                    {
                        AddMod += modifier.Value;
                    }
                    if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                    {
                        MultMod *= modifier.Value;
                    }
                }
            }
            return (AddMod, MultMod);
        }
        public bool CanAffordTech(string itemId)
        {
            var game = _game.CurrentGame;
            var stats = game.ExpansionStats ?? throw new InvalidOperationException("ExpeditionStats indisponível.");

            var cost = ComputeTechCost(itemId);

            var haveKnowledge = GetOrZero(stats.Knowledge, cost.costId);
            if (haveKnowledge < cost.costValue)
            {
                return false;
            }
            return true;
        }
    }
}