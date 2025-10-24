using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Services;
using System.Xml.Linq;

namespace FurmaIdle.Services
{
    public interface IEffectService
    {
        Task ApplyEffect(ItemHelper.ItemType type, string itemId, string stageId);
        
        void OnExpeditionStarted(GameModel g, ExpeditionModel ex);
        void OnExpeditionEnded(GameModel g, ExpeditionModel ex);
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
                                        coin.Value.AddMod += (int)upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        coin.Value.MultMod *= (int)upgrade.EffectValue;
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
                                        know.Value.AddMod += (int)upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        know.Value.MultMod *= (int)upgrade.EffectValue;
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
                                        resource.Value.AddMod += (int)upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        resource.Value.MultMod *= (int)upgrade.EffectValue;
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
                                        click.Value.AddMod += (int)upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        click.Value.MultMod *= (int)upgrade.EffectValue;
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
                                        contract.Value.AddMod += (int)upgrade.EffectValue;
                                    }
                                    if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        contract.Value.MultMod *= (int)upgrade.EffectValue;
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
                // Uso de Habilidade
                Console.WriteLine("[Purchase] Aplicando Efeitos da Especialidade");
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
    }
}
