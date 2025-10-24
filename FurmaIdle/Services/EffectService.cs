using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Services;
using System.Diagnostics.Contracts;
using System.Threading.Channels;
using System.Xml.Linq;
using static FurmaIdle.Helpers.ItemHelper;
using static FurmaIdle.Helpers.LogHelper;

namespace FurmaIdle.Services
{
    public interface IEffectService
    {
        Task ApplyEffect(ItemHelper.ItemType type, string itemId, string stageId);
        
        void OnExpeditionStarted(GameModel g, ExpeditionModel ex);
        void OnExpeditionEnded(GameModel g, ExpeditionModel ex);

        (double Actual, double Total) GetSpecialtyTimer(SpecialtyModel spec);
    }

    public sealed class EffectService : IEffectService
    {
        private readonly ICurrentGameService _game;
        private readonly IUnlockService _unlock;
        private readonly ILocateService _locate;
        private readonly IUiLogService _log;

        public EffectService(ICurrentGameService Game, IUnlockService Unlock, IUiLogService Log, ILocateService Locate)
        {
            _game = Game;
            _unlock = Unlock;
            _locate = Locate;
            _log = Log;
        }

        // Timers de Specialties: specialtyId -> (EndsAt, TotalSec)
        private readonly Dictionary<string, (DateTimeOffset endsAt, double totalSec)> _specTimers
            = new(StringComparer.Ordinal);

        public async Task ApplyEffect(ItemHelper.ItemType type, string itemId, string stageId)
        {
            if (type == ItemHelper.ItemType.Upgrade)
            {
                var upgrade = _locate.LocateUpgrade(_game.CurrentGame, itemId);
                var game = _game.CurrentGame;

                bool hasStages = true;

                upgrade.ActualBuy++;

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
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                    {
                                        character.Value.ContractCap += (int)upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        character.Value.ContractCap *= (int)upgrade.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var character = _locate.LocateCharacter(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                {
                                    character.ContractCap += (int)upgrade.EffectValue;
                                }
                                if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
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
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                    {
                                        coin.Value.AddMod += upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        coin.Value.MultMod *= upgrade.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var coin = _locate.LocateCoin(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                {
                                    coin.AddMod += upgrade.EffectValue;
                                }
                                if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
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
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                    {
                                        know.Value.AddMod += upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        know.Value.MultMod *= upgrade.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var know = _locate.LocateKnowledge(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                {
                                    know.AddMod += upgrade.EffectValue;
                                }
                                if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
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
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                    {
                                        resource.Value.AddMod += upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        resource.Value.MultMod *= upgrade.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var resource = _locate.LocateResource(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                {
                                    resource.AddMod += upgrade.EffectValue;
                                }
                                if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
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
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                    {
                                        click.Value.AddMod += upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        click.Value.MultMod *= upgrade.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var click = _locate.LocateStageClick(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                {
                                    click.AddMod += upgrade.EffectValue;
                                }
                                if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
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
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                    {
                                        contract.Value.AddMod += upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        contract.Value.MultMod *= upgrade.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var contract = _locate.LocateContract(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                {
                                    contract.AddMod += upgrade.EffectValue;
                                }
                                if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
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
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                        c.PriceFactor *= upgrade.EffectValue;
                                    else if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                        c.PriceFactor += upgrade.EffectValue;
                                    else if (upgrade.EffectOp == EffectHelper.EffectOperation.Override)
                                        c.PriceFactor = upgrade.EffectValue;
                                }
                            }
                            else
                            {
                                var c = _locate.LocateCharacter(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                    c.PriceFactor *= upgrade.EffectValue;
                                else if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                    c.PriceFactor += upgrade.EffectValue;
                                else if (upgrade.EffectOp == EffectHelper.EffectOperation.Override)
                                    c.PriceFactor = upgrade.EffectValue;
                            }
                            break;
                        case EffectHelper.EffectType.SpecialtyCost:
                            if (upgrade.TargetId == "aSpecialities")
                            {
                                foreach (var kv in g.Specialties)
                                {
                                    var c = kv.Value;
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                        c.PriceFactor *= upgrade.EffectValue;
                                    else if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                        c.PriceFactor += upgrade.EffectValue;
                                    else if (upgrade.EffectOp == EffectHelper.EffectOperation.Override)
                                        c.PriceFactor = upgrade.EffectValue;
                                }
                            }
                            else
                            {
                                var c = _locate.LocateSpecialty(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                    c.PriceFactor *= upgrade.EffectValue;
                                else if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                    c.PriceFactor += upgrade.EffectValue;
                                else if (upgrade.EffectOp == EffectHelper.EffectOperation.Override)
                                    c.PriceFactor = upgrade.EffectValue;
                            }
                            break;
                        case EffectHelper.EffectType.ContractCost:
                            if (upgrade.TargetId == "aContracts")
                            {
                                foreach (var kv in g.Contracts)
                                {
                                    var c = kv.Value;
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                        c.PriceFactor *= upgrade.EffectValue;
                                    else if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                        c.PriceFactor += upgrade.EffectValue;
                                    else if (upgrade.EffectOp == EffectHelper.EffectOperation.Override)
                                        c.PriceFactor = upgrade.EffectValue;
                                }
                            }
                            else
                            {
                                var c = _locate.LocateContract(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                    c.PriceFactor *= upgrade.EffectValue;
                                else if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                    c.PriceFactor += upgrade.EffectValue;
                                else if (upgrade.EffectOp == EffectHelper.EffectOperation.Override)
                                    c.PriceFactor = upgrade.EffectValue;
                            }
                            break;
                        case EffectHelper.EffectType.ContractTime:
                            if (upgrade.TargetId == "aContracts")
                            {
                                foreach (var kv in g.Contracts)
                                {
                                    var c = kv.Value;
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                        c.TimeFactor *= upgrade.EffectValue;
                                    else if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                        c.TimeFactor += upgrade.EffectValue;
                                    else if (upgrade.EffectOp == EffectHelper.EffectOperation.Override)
                                        c.TimeFactor = upgrade.EffectValue;
                                }
                            }
                            else
                            {
                                var c = _locate.LocateContract(g, upgrade.TargetId);
                                if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                    c.TimeFactor *= upgrade.EffectValue;
                                else if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                                    c.TimeFactor += upgrade.EffectValue;
                                else if (upgrade.EffectOp == EffectHelper.EffectOperation.Override)
                                    c.TimeFactor = upgrade.EffectValue;
                            }
                            break;
                    }

                }, save: true);
            }
            if (type == ItemHelper.ItemType.Expansion)
            {
                // Hard Reset
                Console.WriteLine("[Purchase] Aplicando Efeitos da Expansão");
            }
            if (type == ItemHelper.ItemType.Specialty)
            {
                var spec = _locate.LocateSpecialty(_game.CurrentGame, itemId);
                var stage = _locate.LocateStage(_game.CurrentGame, stageId);
                var Game = _game.CurrentGame;
                if (spec is null) return;

                await _game.Mutate(g =>
                {
                    var dur = Math.Max(0.001, spec.Duration);
                    var now = DateTimeOffset.UtcNow;

                    switch (spec.EffectType)
                    {
                        // Gains
                        case EffectHelper.EffectType.CoinGain:
                            if (spec.TargetId == "aCoins")
                            {
                                foreach (var coin in g.Coins)
                                {
                                    if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    {
                                        coin.Value.AddMod += spec.EffectValue;
                                    }
                                    if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        coin.Value.MultMod *= spec.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var coin = _locate.LocateCoin(g, spec.TargetId);
                                if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                {
                                    coin.AddMod += spec.EffectValue;
                                }
                                if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                {
                                    coin.MultMod *= spec.EffectValue;
                                }
                            }
                            break;
                        case EffectHelper.EffectType.KnowledgeGain:
                            if (spec.TargetId == "aKnowledges")
                            {
                                foreach (var know in g.Knowledges)
                                {
                                    if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    {
                                        know.Value.AddMod += spec.EffectValue;
                                    }
                                    if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        know.Value.MultMod *= spec.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var know = _locate.LocateKnowledge(g, spec.TargetId);
                                if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                {
                                    know.AddMod += spec.EffectValue;
                                }
                                if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                {
                                    know.MultMod *= spec.EffectValue;
                                }
                            }
                            break;
                        case EffectHelper.EffectType.ResourceGain:
                            if (spec.TargetId == "aResources")
                            {
                                foreach (var resource in g.Resources)
                                {
                                    if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    {
                                        resource.Value.AddMod += spec.EffectValue;
                                    }
                                    if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        resource.Value.MultMod *= spec.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var resource = _locate.LocateResource(g, spec.TargetId);
                                if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                {
                                    resource.AddMod += spec.EffectValue;
                                }
                                if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                {
                                    resource.MultMod *= spec.EffectValue;
                                }
                            }
                            break;
                        case EffectHelper.EffectType.ClickGain:
                            if (spec.TargetId == "aClicks")
                            {
                                foreach (var click in g.Clicks)
                                {
                                    if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    {
                                        click.Value.AddMod += spec.EffectValue;
                                    }
                                    if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        click.Value.MultMod *= spec.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var click = _locate.LocateStageClick(g, spec.TargetId);
                                if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                {
                                    click.AddMod += spec.EffectValue;
                                }
                                if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                {
                                    click.MultMod *= spec.EffectValue;
                                }
                            }
                            break;
                        case EffectHelper.EffectType.ContractGain:
                            if (spec.TargetId == "aContracts")
                            {
                                foreach (var contract in g.Contracts)
                                {
                                    if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    {
                                        contract.Value.AddMod += spec.EffectValue;
                                    }
                                    if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        contract.Value.MultMod *= spec.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var contract = _locate.LocateContract(g, spec.TargetId);
                                if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                {
                                    contract.AddMod += spec.EffectValue;
                                }
                                if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                {
                                    contract.MultMod *= spec.EffectValue;
                                }
                            }
                            break;

                        // Modifiers
                        case EffectHelper.EffectType.CharacterCost:
                            if (spec.TargetId == "aCharacters")
                            {
                                foreach (var kv in g.Characters)
                                {
                                    var c = kv.Value;
                                    if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                        c.PriceFactor *= spec.EffectValue;
                                    else if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                        c.PriceFactor += spec.EffectValue;
                                    else if (spec.EffectOperation == EffectHelper.EffectOperation.Override)
                                        c.PriceFactor = spec.EffectValue;
                                }
                            }
                            else
                            {
                                var c = _locate.LocateCharacter(g, spec.TargetId);
                                if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    c.PriceFactor *= spec.EffectValue;
                                else if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    c.PriceFactor += spec.EffectValue;
                                else if (spec.EffectOperation == EffectHelper.EffectOperation.Override)
                                    c.PriceFactor = spec.EffectValue;
                            }
                            break;
                        case EffectHelper.EffectType.SpecialtyCost:
                            if (spec.TargetId == "aSpecialities")
                            {
                                foreach (var kv in g.Specialties)
                                {
                                    var c = kv.Value;
                                    if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                        c.PriceFactor *= spec.EffectValue;
                                    else if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                        c.PriceFactor += spec.EffectValue;
                                    else if (spec.EffectOperation == EffectHelper.EffectOperation.Override)
                                        c.PriceFactor = spec.EffectValue;
                                }
                            }
                            else
                            {
                                var c = _locate.LocateSpecialty(g, spec.TargetId);
                                if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    c.PriceFactor *= spec.EffectValue;
                                else if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    c.PriceFactor += spec.EffectValue;
                                else if (spec.EffectOperation == EffectHelper.EffectOperation.Override)
                                    c.PriceFactor = spec.EffectValue;
                            }
                            break;
                        case EffectHelper.EffectType.ContractCost:
                            if (spec.TargetId == "aContracts")
                            {
                                foreach (var kv in g.Contracts)
                                {
                                    var c = kv.Value;
                                    if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                        c.PriceFactor *= spec.EffectValue;
                                    else if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                        c.PriceFactor += spec.EffectValue;
                                    else if (spec.EffectOperation == EffectHelper.EffectOperation.Override)
                                        c.PriceFactor = spec.EffectValue;
                                }
                            }
                            else
                            {
                                var c = _locate.LocateContract(g, spec.TargetId);
                                if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    c.PriceFactor *= spec.EffectValue;
                                else if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    c.PriceFactor += spec.EffectValue;
                                else if (spec.EffectOperation == EffectHelper.EffectOperation.Override)
                                    c.PriceFactor = spec.EffectValue;
                            }
                            break;
                        case EffectHelper.EffectType.ContractTime:
                            if (spec.TargetId == "aContracts")
                            {
                                foreach (var kv in g.Contracts)
                                {
                                    var c = kv.Value;
                                    if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                        c.TimeFactor *= spec.EffectValue;
                                    else if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                        c.TimeFactor += spec.EffectValue;
                                    else if (spec.EffectOperation == EffectHelper.EffectOperation.Override)
                                        c.TimeFactor = spec.EffectValue;
                                }
                            }
                            else
                            {
                                var c = _locate.LocateContract(g, spec.TargetId);
                                if (spec.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    c.TimeFactor *= spec.EffectValue;
                                else if (spec.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    c.TimeFactor += spec.EffectValue;
                                else if (spec.EffectOperation == EffectHelper.EffectOperation.Override)
                                    c.TimeFactor = spec.EffectValue;
                            }
                            break;
                    }

                    _specTimers[itemId] = (now.AddSeconds(dur), dur);

                    Console.WriteLine($"[Purchase] Specialty {itemId} ativa por {dur:0.##}s");

                }, save: true);
            }

            if (type == ItemHelper.ItemType.Trait)
            {
                var g = _game.CurrentGame;

                await _game.Mutate(g =>
                {
                    var trait = _locate.LocateTrait(_game.CurrentGame, itemId);

                    switch (trait.EffectType)
                    {
                        // Gains
                        case EffectHelper.EffectType.CoinGain:
                            if (trait.TargetId == "aCoins")
                            {
                                foreach (var coin in g.Coins)
                                {
                                    if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    {
                                        coin.Value.AddMod += trait.EffectValue;
                                    }
                                    if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        coin.Value.MultMod *= trait.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var coin = _locate.LocateCoin(g, trait.TargetId);
                                if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                {
                                    coin.AddMod += trait.EffectValue;
                                }
                                if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                {
                                    coin.MultMod *= trait.EffectValue;
                                }
                            }
                            break;
                        case EffectHelper.EffectType.KnowledgeGain:
                            if (trait.TargetId == "aKnowledges")
                            {
                                foreach (var know in g.Knowledges)
                                {
                                    if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    {
                                        know.Value.AddMod += trait.EffectValue;
                                    }
                                    if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        know.Value.MultMod *= trait.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var know = _locate.LocateKnowledge(g, trait.TargetId);
                                if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                {
                                    know.AddMod += trait.EffectValue;
                                }
                                if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                {
                                    know.MultMod *= trait.EffectValue;
                                }
                            }
                            break;
                        case EffectHelper.EffectType.ResourceGain:
                            if (trait.TargetId == "aResources")
                            {
                                foreach (var resource in g.Resources)
                                {
                                    if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    {
                                        resource.Value.AddMod += trait.EffectValue;
                                    }
                                    if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        resource.Value.MultMod *= trait.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var resource = _locate.LocateResource(g, trait.TargetId);
                                if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                {
                                    resource.AddMod += trait.EffectValue;
                                }
                                if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                {
                                    resource.MultMod *= trait.EffectValue;
                                }
                            }
                            break;
                        case EffectHelper.EffectType.ClickGain:
                            if (trait.TargetId == "aClicks")
                            {
                                foreach (var click in g.Clicks)
                                {
                                    if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    {
                                        click.Value.AddMod += trait.EffectValue;
                                    }
                                    if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        click.Value.MultMod *= trait.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var click = _locate.LocateStageClick(g, trait.TargetId);
                                if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                {
                                    click.AddMod += trait.EffectValue;
                                }
                                if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                {
                                    click.MultMod *= trait.EffectValue;
                                }
                            }
                            break;
                        case EffectHelper.EffectType.ContractGain:
                            if (trait.TargetId == "aContracts")
                            {
                                foreach (var contract in g.Contracts)
                                {
                                    if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    {
                                        contract.Value.AddMod += trait.EffectValue;
                                    }
                                    if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        contract.Value.MultMod *= trait.EffectValue;
                                    }
                                }
                            }
                            else
                            {
                                var contract = _locate.LocateContract(g, trait.TargetId);
                                if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                {
                                    contract.AddMod += trait.EffectValue;
                                }
                                if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                {
                                    contract.MultMod *= trait.EffectValue;
                                }
                            }
                            break;

                        // Modifiers
                        case EffectHelper.EffectType.CharacterCost:
                            if (trait.TargetId == "aCharacters")
                            {
                                foreach (var kv in g.Characters)
                                {
                                    var c = kv.Value;
                                    if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                        c.PriceFactor *= trait.EffectValue;
                                    else if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                        c.PriceFactor += trait.EffectValue;
                                    else if (trait.EffectOperation == EffectHelper.EffectOperation.Override)
                                        c.PriceFactor = trait.EffectValue;
                                }
                            }
                            else
                            {
                                var c = _locate.LocateCharacter(g, trait.TargetId);
                                if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    c.PriceFactor *= trait.EffectValue;
                                else if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    c.PriceFactor += trait.EffectValue;
                                else if (trait.EffectOperation == EffectHelper.EffectOperation.Override)
                                    c.PriceFactor = trait.EffectValue;
                            }
                            break;
                        case EffectHelper.EffectType.SpecialtyCost:
                            if (trait.TargetId == "aSpecialities")
                            {
                                foreach (var kv in g.Specialties)
                                {
                                    var c = kv.Value;
                                    if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                        c.PriceFactor *= trait.EffectValue;
                                    else if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                        c.PriceFactor += trait.EffectValue;
                                    else if (trait.EffectOperation == EffectHelper.EffectOperation.Override)
                                        c.PriceFactor = trait.EffectValue;
                                }
                            }
                            else
                            {
                                var c = _locate.LocateSpecialty(g, trait.TargetId);
                                if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    c.PriceFactor *= trait.EffectValue;
                                else if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    c.PriceFactor += trait.EffectValue;
                                else if (trait.EffectOperation == EffectHelper.EffectOperation.Override)
                                    c.PriceFactor = trait.EffectValue;
                            }
                            break;
                        case EffectHelper.EffectType.ContractCost:
                            if (trait.TargetId == "aContracts")
                            {
                                foreach (var kv in g.Contracts)
                                {
                                    var c = kv.Value;
                                    if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                        c.PriceFactor *= trait.EffectValue;
                                    else if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                        c.PriceFactor += trait.EffectValue;
                                    else if (trait.EffectOperation == EffectHelper.EffectOperation.Override)
                                        c.PriceFactor = trait.EffectValue;
                                }
                            }
                            else
                            {
                                var c = _locate.LocateContract(g, trait.TargetId);
                                if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    c.PriceFactor *= trait.EffectValue;
                                else if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    c.PriceFactor += trait.EffectValue;
                                else if (trait.EffectOperation == EffectHelper.EffectOperation.Override)
                                    c.PriceFactor = trait.EffectValue;
                            }
                            break;
                        case EffectHelper.EffectType.ContractTime:
                            if (trait.TargetId == "aContracts")
                            {
                                foreach (var kv in g.Contracts)
                                {
                                    var c = kv.Value;
                                    if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                        c.TimeFactor *= trait.EffectValue;
                                    else if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                        c.TimeFactor += trait.EffectValue;
                                    else if (trait.EffectOperation == EffectHelper.EffectOperation.Override)
                                        c.TimeFactor = trait.EffectValue;
                                }
                            }
                            else
                            {
                                var c = _locate.LocateContract(g, trait.TargetId);
                                if (trait.EffectOperation == EffectHelper.EffectOperation.Multiplicative)
                                    c.TimeFactor *= trait.EffectValue;
                                else if (trait.EffectOperation == EffectHelper.EffectOperation.Additive)
                                    c.TimeFactor += trait.EffectValue;
                                else if (trait.EffectOperation == EffectHelper.EffectOperation.Override)
                                    c.TimeFactor = trait.EffectValue;
                            }
                            break;
                    }

                }, save: true);
            }
        }

        // Expedition
        public void OnExpeditionStarted(GameModel g, ExpeditionModel ex)
        {
            if (g is null || ex is null) return;

            foreach (var charId in ex.PartyIds ?? Enumerable.Empty<string>())
            {
                var ch = _locate.LocateCharacter(g, charId);
                if (ch is null || ch.State != UnlockHelper.State.Unlocked) continue;

                if (!string.IsNullOrWhiteSpace(ch.TraitId))
                {
                    var tr = _locate.LocateTrait(g, ch.TraitId);
                    if (tr is not null)
                    {
                         ApplyEffect(ItemHelper.ItemType.Trait, tr.Id, "aStages");
                    }
                }
            }
        }

        public void OnExpeditionEnded(GameModel g, ExpeditionModel ex)
        {
            if (g is null || ex is null) return;
        }

        private void RevertTraitEffects(GameModel g)
        {
            foreach (var r in g.Resources.Values)
            {
                if (r is null) continue;
                r.AddMod = Math.Max(0, r.AddMod - 0.5);
            }
        }

        // Specialties
        public (double Actual, double Total) GetSpecialtyTimer(string specialtyId)
        {
            if (string.IsNullOrWhiteSpace(specialtyId)) return (0, 0);

            if (_specTimers.TryGetValue(specialtyId, out var t))
            {
                var now = DateTimeOffset.UtcNow;
                var remaining = (t.endsAt - now).TotalSeconds;
                if (remaining <= 0)
                {
                    _specTimers.Remove(specialtyId);
                    return (0, t.totalSec);
                }
                return (remaining, t.totalSec);
            }
            return (0, 0);
        }
        public (double Actual, double Total) GetSpecialtyTimer(SpecialtyModel spec)
            => spec is null ? (0, 0) : GetSpecialtyTimer(spec.Id);
    }
}
