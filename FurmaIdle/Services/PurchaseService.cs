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
        Task Purchase(ItemHelper.ItemType type, string itemId, string stageId);
        Task ApplyEffect(ItemHelper.ItemType type, string itemId, string stageId);

        bool CanAfford(ItemHelper.ItemType type, string itemId, string stageId);
    }

    public sealed class PurchaseService : IPurchaseService
    {
        private readonly ICurrentGameService _game;
        private readonly IIncomeService _income;
        private readonly IUnlockService _unlock;
        private readonly ILocateService _locate;
        private readonly IExpeditionService _expedition;

        public sealed record CostLine(string CurrencyGroup, string CurrencyId, double Amount);

        public PurchaseService(ICurrentGameService Game, IIncomeService Income, IUnlockService Unlock, ILocateService Locate, IExpeditionService expedition)
        {
            _game = Game;
            _income = Income;
            _unlock = Unlock;
            _locate = Locate;
            _expedition = expedition;
        }

        // Purchase
        public async Task Purchase(ItemHelper.ItemType type, string itemId, string stageId)
        {
            var game = _game.CurrentGame;
            var stage = _locate.LocateStage(game, stageId);

            var cost = ComputeCost(type, itemId, stageId);

            await _game.Mutate(game =>
            {
                Console.WriteLine($"[Purchase] Custo: {cost.costValue} {cost.costId}");
                ApplyDebit(game.ExpeditionStats, cost.costValue, cost.costId);
                ApplyStats(game.ExpansionStats, game.GameStats, cost.costValue, cost.costId);

            }, save: true);

            await ApplyEffect(type, itemId, stageId);            
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
        public async Task ApplyEffect(ItemHelper.ItemType type, string itemId, string stageId)
        {
            if (type == ItemHelper.ItemType.Upgrade)
            {
                Console.WriteLine("[Purchase] Aplicando Efeitos do Update");
                var upgrade = _locate.LocateUpgrade(_game.CurrentGame, itemId);
                var game = _game.CurrentGame;

                bool hasStages = true;

                upgrade.ActualBuy++;
                Console.WriteLine($"[Purhcase] Compra: {upgrade.ActualBuy} - Máximo: {upgrade.MaxBuy}");

                if (upgrade.ActualBuy == upgrade.MaxBuy)
                {
                    hasStages = false;
                }

                if (!hasStages)
                {
                    await _unlock.UnlockUpgrade(upgrade.Id);
                }

                await _game.Mutate(g =>
                {
                    var stage = _locate.LocateStage(g, stageId);

                    switch (upgrade.EffectType)
                    {

                        case EffectHelper.EffectType.ContractLevelUnlock:
                            stage.ActualContractLevel += (int)upgrade.EffectValue;
                            break;
                        case EffectHelper.EffectType.ContractCapUnlock:
                            if (upgrade.TargetId == "aCharacters")
                            {
                                foreach (var character in g.Characters)
                                {
                                    if (upgrade.EffectOp == EffectOperation.Additive)
                                    {
                                        character.Value.ContractCap += (int)upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                    {
                                        character.Value.ContractCap *= (int)upgrade.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var character = _locate.LocateCharacter(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectOperation.Additive)
                                {
                                    character.ContractCap += (int)upgrade.EffectValue;
                                }
                                if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                {
                                    character.ContractCap *= (int)upgrade.EffectValue;
                                }
                            }
                            break;
                        case EffectHelper.EffectType.PartySize:
                            stage.PartySizeActual += (int)upgrade.EffectValue;
                            break;

                        // Gains
                        case EffectHelper.EffectType.CoinGain:
                            if (upgrade.TargetId == "aCoins")
                            {
                                foreach (var coin in g.Coins)
                                {
                                    if (upgrade.EffectOp == EffectOperation.Additive)
                                    {
                                        coin.Value.AddMod += (int)upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                    {
                                        coin.Value.MultMod *= (int)upgrade.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var coin = _locate.LocateCoin(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectOperation.Additive)
                                {
                                    coin.AddMod += upgrade.EffectValue;
                                }
                                if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                {
                                    coin.MultMod *= upgrade.EffectValue;
                                }
                            }
                            break;
                        case EffectHelper.EffectType.KnowledgeGain:
                            if (upgrade.TargetId == "aKnowledges")
                            {
                                foreach (var know in g.Knowledges)
                                {
                                    if (upgrade.EffectOp == EffectOperation.Additive)
                                    {
                                        know.Value.AddMod += (int)upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                    {
                                        know.Value.MultMod *= (int)upgrade.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var know = _locate.LocateKnowledge(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectOperation.Additive)
                                {
                                    know.AddMod += upgrade.EffectValue;
                                }
                                if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                {
                                    know.MultMod *= upgrade.EffectValue;
                                }
                            }
                            break;
                        case EffectHelper.EffectType.ResourceGain:
                            if (upgrade.TargetId == "aResources")
                            {
                                foreach (var resource in g.Resources)
                                {
                                    if (upgrade.EffectOp == EffectOperation.Additive)
                                    {
                                        resource.Value.AddMod += (int)upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                    {
                                        resource.Value.MultMod *= (int)upgrade.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var resource = _locate.LocateResource(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectOperation.Additive)
                                {
                                    resource.AddMod += upgrade.EffectValue;
                                }
                                if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                {
                                    resource.MultMod *= upgrade.EffectValue;
                                }
                            }
                            break;
                        case EffectHelper.EffectType.ClickGain:
                            if (upgrade.TargetId == "aClicks")
                            {
                                foreach (var click in g.Clicks)
                                {
                                    if (upgrade.EffectOp == EffectOperation.Additive)
                                    {
                                        click.Value.AddMod += (int)upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                    {
                                        click.Value.MultMod *= (int)upgrade.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var click = _locate.LocateStageClick(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectOperation.Additive)
                                {
                                    click.AddMod += upgrade.EffectValue;
                                }
                                if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                {
                                    click.MultMod *= upgrade.EffectValue;
                                }
                            }
                            break;
                        case EffectHelper.EffectType.ContractGain:
                            if (upgrade.TargetId == "aContracts")
                            {
                                foreach (var contract in g.Contracts)
                                {
                                    if (upgrade.EffectOp == EffectOperation.Additive)
                                    {
                                        contract.Value.AddMod += (int)upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                    {
                                        contract.Value.MultMod *= (int)upgrade.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var contract = _locate.LocateContract(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectOperation.Additive)
                                {
                                    contract.AddMod += upgrade.EffectValue;
                                }
                                if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                {
                                    contract.MultMod *= upgrade.EffectValue;
                                }
                            }
                            break;

                        // Modifiers
                        case EffectHelper.EffectType.CharacterCost:
                            if (upgrade.TargetId == "aCharacters")
                            {
                                foreach (var kv in g.Characters)
                                {
                                    var c = kv.Value;
                                    if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                        c.PriceFactor *= upgrade.EffectValue;
                                    else if (upgrade.EffectOp == EffectOperation.Additive)
                                        c.PriceFactor += upgrade.EffectValue;
                                    else if (upgrade.EffectOp == EffectOperation.Override)
                                        c.PriceFactor = upgrade.EffectValue;
                                }
                            }
                            else
                            {
                                var c = _locate.LocateCharacter(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                    c.PriceFactor *= upgrade.EffectValue;
                                else if (upgrade.EffectOp == EffectOperation.Additive)
                                    c.PriceFactor += upgrade.EffectValue;
                                else if (upgrade.EffectOp == EffectOperation.Override)
                                    c.PriceFactor = upgrade.EffectValue;
                            }
                            break;
                        case EffectHelper.EffectType.SpecialtyCost:
                            if (upgrade.TargetId == "aSpecialities")
                            {
                                foreach (var kv in g.Specialties)
                                {
                                    var c = kv.Value;
                                    if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                        c.PriceFactor *= upgrade.EffectValue;
                                    else if (upgrade.EffectOp == EffectOperation.Additive)
                                        c.PriceFactor += upgrade.EffectValue;
                                    else if (upgrade.EffectOp == EffectOperation.Override)
                                        c.PriceFactor = upgrade.EffectValue;
                                }
                            }
                            else
                            {
                                var c = _locate.LocateSpecialty(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                    c.PriceFactor *= upgrade.EffectValue;
                                else if (upgrade.EffectOp == EffectOperation.Additive)
                                    c.PriceFactor += upgrade.EffectValue;
                                else if (upgrade.EffectOp == EffectOperation.Override)
                                    c.PriceFactor = upgrade.EffectValue;
                            }
                            break;
                        case EffectHelper.EffectType.ContractCost:
                            if (upgrade.TargetId == "aContracts")
                            {
                                foreach (var kv in g.Contracts)
                                {
                                    var c = kv.Value;
                                    if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                        c.PriceFactor *= upgrade.EffectValue;
                                    else if (upgrade.EffectOp == EffectOperation.Additive)
                                        c.PriceFactor += upgrade.EffectValue;
                                    else if (upgrade.EffectOp == EffectOperation.Override)
                                        c.PriceFactor = upgrade.EffectValue;
                                }
                            }
                            else
                            {
                                var c = _locate.LocateContract(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                    c.PriceFactor *= upgrade.EffectValue;
                                else if (upgrade.EffectOp == EffectOperation.Additive)
                                    c.PriceFactor += upgrade.EffectValue;
                                else if (upgrade.EffectOp == EffectOperation.Override)
                                    c.PriceFactor = upgrade.EffectValue;
                            }
                            break;
                        case EffectHelper.EffectType.ContractTime:
                            if (upgrade.TargetId == "aContracts")
                            {
                                foreach (var kv in g.Contracts)
                                {
                                    var c = kv.Value;
                                    if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                        c.TimeFactor *= upgrade.EffectValue;
                                    else if (upgrade.EffectOp == EffectOperation.Additive)
                                        c.TimeFactor += upgrade.EffectValue;
                                    else if (upgrade.EffectOp == EffectOperation.Override)
                                        c.TimeFactor = upgrade.EffectValue;
                                }
                            }
                            else
                            {
                                var c = _locate.LocateContract(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectOperation.Multiplicative)
                                    c.TimeFactor *= upgrade.EffectValue;
                                else if (upgrade.EffectOp == EffectOperation.Additive)
                                    c.TimeFactor += upgrade.EffectValue;
                                else if (upgrade.EffectOp == EffectOperation.Override)
                                    c.TimeFactor = upgrade.EffectValue;
                            }
                            break;
                    }

                }, save: true);
            }
            if (type == ItemHelper.ItemType.Expedition)
            {
                Console.WriteLine("[Purchase] Aplicando Reset da Expedição");

                await _game.Mutate(g => { _expedition.EndExpedition(g, stageId); }, save: true);

                // Reaplica permanentes corretamente (via UnlockService + efeitos numéricos)
                await _expedition.ReapplyAfterResetAsync(stageId);
            }
            if (type == ItemHelper.ItemType.Expansion)
            {
                // Hard Reset
                Console.WriteLine("[Purchase] Aplicando Efeitos da Expansão");
            }
            if (type == ItemHelper.ItemType.Specialty)
            {
                // Uso de Habilidade
                Console.WriteLine("[Purchase] Aplicando Efeitos da Especialidade");
            }
        }

        private (long costValue, string costId) ComputeCost(ItemHelper.ItemType type, string itemId, string stageId)
        {
            var game = _game.CurrentGame;

            long costValue = 0;
            string costId = string.Empty;
            long costBase = 0;
            double costCurve = 0;
            double costAddFactor = 0;
            double costMultFactor = 1;
            double costFactorValue = 1;

            switch (type)
            {
                case ItemHelper.ItemType.Specialty:
                    {
                        var specialty = _locate.LocateSpecialty(game, itemId);
                        costValue = specialty.Cost;
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
                                costAddFactor = Math.Pow(entry.CostFactorCurve, up.ActualBuy) * costFactorValue;
                                costMultFactor = 1;
                            }

                            if (entry.CostFactorType == PricingHelper.CostFactorType.Multiplicative)
                            {
                                costAddFactor = 0;
                                costMultFactor = Math.Pow(entry.CostFactorCurve, up.ActualBuy) * costFactorValue;
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
                                costAddFactor = Math.Pow(entry.CostFactorCurve, quantity) * costFactorValue;
                                costMultFactor = 1;
                            }

                            if (entry.CostFactorType == PricingHelper.CostFactorType.Multiplicative)
                            {
                                costAddFactor = 0;
                                costMultFactor = Math.Pow(entry.CostFactorCurve, quantity) * costFactorValue;
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
            if (nv < 0) nv = 0; // proteção mínima
            dict[id] = nv;
        }
        private static long GetOrZero(Dictionary<string, long> dict, string id)
                    => dict is not null && dict.TryGetValue(id, out var v) ? v : 0L;
    }
}