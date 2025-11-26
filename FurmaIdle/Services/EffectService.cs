using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Diagnostics.Contracts;
using System.Threading.Channels;
using System.Transactions;
using System.Xml.Linq;
using static FurmaIdle.Helpers.EffectHelper;

namespace FurmaIdle.Services
{
    public interface IEffectService
    {
        Task ApplyEffect(ItemHelper.ItemType type, string itemId, string stageId);
    }

    public sealed class EffectService : IEffectService
    {
        private readonly ICurrentGameService _game;
        private readonly IUnlockService _unlock;
        private readonly ILocateService _locate;
        private readonly IUiLogService _log;
        private readonly ISpecialtiesService _specialty;
        private readonly IModifierService _modifier;

        public EffectService(ICurrentGameService Game, IUnlockService Unlock, IUiLogService Log, ILocateService Locate, ISpecialtiesService Specialties, IModifierService modifier)
        {
            _game = Game;
            _unlock = Unlock;
            _locate = Locate;
            _log = Log;
            _specialty = Specialties;
            _modifier = modifier;
        }

        public async Task ApplyEffect(ItemHelper.ItemType type, string itemId, string stageId)
        {
            var game = _game.CurrentGame;
            var stage = _locate.LocateStage(game, stageId);

            if (type == ItemHelper.ItemType.Upgrade)
            {
                var upgrade = _locate.LocateUpgrade(game, itemId);
                string targetTypeId = upgrade.TargetId.Length >= 2
                    ? upgrade.TargetId.Substring(0, 1)
                    : upgrade.TargetId;

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

                await _game.Mutate(game =>
                {
                    if (upgrade.EffectOp != EffectOperation.Unlock)
                    {
                        switch (targetTypeId)
                        {
                            case "a": // All of a Kind
                                if (upgrade.TargetId == "aContracts")
                                {
                                    foreach (var acontract in game.Contracts)
                                    {
                                        var aNewMod = new ModifierModel
                                        {
                                            ApplyerId = upgrade.Id,
                                            Type = upgrade.EffectType,
                                            Scope = upgrade.Persistence,
                                            Operation = upgrade.EffectOp,
                                            Value = upgrade.EffectValue,
                                            Supertype = upgrade.EffectSupertype,
                                        };

                                        acontract.Value.Modifiers.Add(aNewMod);
                                    }
                                }
                                if (upgrade.TargetId == "aKnowledges")
                                {
                                    foreach (var aknow in game.Knowledges)
                                    {

                                        var aNewMod = new ModifierModel
                                        {
                                            ApplyerId = upgrade.Id,
                                            Type = upgrade.EffectType,
                                            Scope = upgrade.Persistence,
                                            Operation = upgrade.EffectOp,
                                            Value = upgrade.EffectValue,
                                            Supertype = upgrade.EffectSupertype,
                                        };

                                        aknow.Value.Modifiers.Add(aNewMod);
                                    }
                                }
                                if (upgrade.TargetId == "aCoins")
                                {
                                    foreach (var acoin in game.Coins)
                                    {
                                        var aNewMod = new ModifierModel
                                        {
                                            ApplyerId = upgrade.Id,
                                            Type = upgrade.EffectType,
                                            Scope = upgrade.Persistence,
                                            Operation = upgrade.EffectOp,
                                            Value = upgrade.EffectValue,
                                            Supertype = upgrade.EffectSupertype,
                                        };

                                        acoin.Value.Modifiers.Add(aNewMod);
                                    }
                                }
                                if (upgrade.TargetId == "aResources")
                                {
                                    foreach (var aresource in game.Resources)
                                    {
                                        var aNewMod = new ModifierModel
                                        {
                                            ApplyerId = upgrade.Id,
                                            Type = upgrade.EffectType,
                                            Scope = upgrade.Persistence,
                                            Operation = upgrade.EffectOp,
                                            Value = upgrade.EffectValue,
                                            Supertype = upgrade.EffectSupertype,
                                        };

                                        aresource.Value.Modifiers.Add(aNewMod);
                                    }
                                }
                                if (upgrade.TargetId == "aClicks")
                                {
                                    foreach (var aclick in game.Clicks)
                                    {
                                        var aNewMod = new ModifierModel
                                        {
                                            ApplyerId = upgrade.Id,
                                            Type = upgrade.EffectType,
                                            Scope = upgrade.Persistence,
                                            Operation = upgrade.EffectOp,
                                            Value = upgrade.EffectValue,
                                            Supertype = upgrade.EffectSupertype,
                                        };

                                        aclick.Value.Modifiers.Add(aNewMod);
                                    }
                                }
                                if (upgrade.TargetId == "aCharacters")
                                {
                                    foreach (var acharacters in game.Characters)
                                    {
                                        var aNewMod = new ModifierModel
                                        {
                                            ApplyerId = upgrade.Id,
                                            Type = upgrade.EffectType,
                                            Scope = upgrade.Persistence,
                                            Operation = upgrade.EffectOp,
                                            Value = upgrade.EffectValue,
                                            Supertype = upgrade.EffectSupertype,
                                        };

                                        acharacters.Value.Modifiers.Add(aNewMod);
                                    }
                                }
                                if (upgrade.TargetId == "aUpgrades")
                                {
                                    foreach (var aupgrades in game.Upgrades)
                                    {
                                        var aNewMod = new ModifierModel
                                        {
                                            ApplyerId = upgrade.Id,
                                            Type = upgrade.EffectType,
                                            Scope = upgrade.Persistence,
                                            Operation = upgrade.EffectOp,
                                            Value = upgrade.EffectValue,
                                            Supertype = upgrade.EffectSupertype,
                                        };

                                        aupgrades.Value.Modifiers.Add(aNewMod);
                                    }
                                }
                                break;
                            case "m": // Coins
                                var coins = _locate.LocateCoin(game, upgrade.TargetId);

                                var mMod = new ModifierModel
                                {
                                    ApplyerId = upgrade.Id,
                                    Type = upgrade.EffectType,
                                    Scope = upgrade.Persistence,
                                    Operation = upgrade.EffectOp,
                                    Value = upgrade.EffectValue,
                                    Supertype = upgrade.EffectSupertype,
                                };

                                coins.Modifiers.Add(mMod);
                                break;
                            case "p": // Characters
                                var character = _locate.LocateCharacter(game, upgrade.TargetId);

                                var pMod = new ModifierModel
                                {
                                    ApplyerId = upgrade.Id,
                                    Type = upgrade.EffectType,
                                    Scope = upgrade.Persistence,
                                    Operation = upgrade.EffectOp,
                                    Value = upgrade.EffectValue,
                                    Supertype = upgrade.EffectSupertype,
                                };

                                character.Modifiers.Add(pMod);
                                break;
                            case "k": // Knowledge
                                var knowledge = _locate.LocateKnowledge(game, upgrade.TargetId);

                                var kMod = new ModifierModel
                                {
                                    ApplyerId = upgrade.Id,
                                    Type = upgrade.EffectType,
                                    Scope = upgrade.Persistence,
                                    Operation = upgrade.EffectOp,
                                    Value = upgrade.EffectValue,
                                    Supertype = upgrade.EffectSupertype,
                                };

                                knowledge.Modifiers.Add(kMod);
                                break;
                            case "t": // Techs
                                var tech = _locate.LocateTech(game, upgrade.TargetId);

                                var tmod = new ModifierModel
                                {
                                    ApplyerId = upgrade.Id,
                                    Type = upgrade.EffectType,
                                    Scope = upgrade.Persistence,
                                    Operation = upgrade.EffectOp,
                                    Value = upgrade.EffectValue,
                                    Supertype = upgrade.EffectSupertype,
                                };

                                tech.Modifiers.Add(tmod);
                                break;
                            case "u": // Upgrades
                                var targetupgrade = _locate.LocateUpgrade(game, upgrade.TargetId);

                                var umod = new ModifierModel
                                {
                                    ApplyerId = upgrade.Id,
                                    Type = upgrade.EffectType,
                                    Scope = upgrade.Persistence,
                                    Operation = upgrade.EffectOp,
                                    Value = upgrade.EffectValue,
                                    Supertype = upgrade.EffectSupertype,
                                };

                                targetupgrade.Modifiers.Add(umod);
                                break;
                            case "l": // Locals
                                var local = _locate.LocateLocal(game, upgrade.TargetId);

                                var lmod = new ModifierModel
                                {
                                    ApplyerId = upgrade.Id,
                                    Type = upgrade.EffectType,
                                    Scope = upgrade.Persistence,
                                    Operation = upgrade.EffectOp,
                                    Value = upgrade.EffectValue,
                                    Supertype = upgrade.EffectSupertype,
                                };

                                local.Modifiers.Add(lmod);
                                break;
                            case "s": // Stages
                                var targetstage = _locate.LocateStage(game, upgrade.TargetId);

                                var smod = new ModifierModel
                                {
                                    ApplyerId = upgrade.Id,
                                    Type = upgrade.EffectType,
                                    Scope = upgrade.Persistence,
                                    Operation = upgrade.EffectOp,
                                    Value = upgrade.EffectValue,
                                    Supertype = upgrade.EffectSupertype,
                                };

                                targetstage.Modifiers.Add(smod);
                                break;
                            case "x": // Expansions
                                var expansion = _locate.LocateExpansion(game, upgrade.TargetId);

                                var xmod = new ModifierModel
                                {
                                    ApplyerId = upgrade.Id,
                                    Type = upgrade.EffectType,
                                    Scope = upgrade.Persistence,
                                    Operation = upgrade.EffectOp,
                                    Value = upgrade.EffectValue,
                                    Supertype = upgrade.EffectSupertype,
                                };

                                expansion.Modifiers.Add(xmod);
                                break;
                            case "d": // Expeditions
                                var expedition = _locate.LocateExpedition(game, upgrade.TargetId);

                                var dmod = new ModifierModel
                                {
                                    ApplyerId = upgrade.Id,
                                    Type = upgrade.EffectType,
                                    Scope = upgrade.Persistence,
                                    Operation = upgrade.EffectOp,
                                    Value = upgrade.EffectValue,
                                    Supertype = upgrade.EffectSupertype,
                                };

                                expedition.Modifiers.Add(dmod);
                                break;
                            case "o": // Traits
                                var trait = _locate.LocateTrait(game, upgrade.TargetId);

                                var omod = new ModifierModel
                                {
                                    ApplyerId = upgrade.Id,
                                    Type = upgrade.EffectType,
                                    Scope = upgrade.Persistence,
                                    Operation = upgrade.EffectOp,
                                    Value = upgrade.EffectValue,
                                    Supertype = upgrade.EffectSupertype,
                                };

                                trait.Modifiers.Add(omod);
                                break;
                            case "e": // Specialty
                                var speciality = _locate.LocateSpecialty(game, upgrade.TargetId);

                                var emod = new ModifierModel
                                {
                                    ApplyerId = upgrade.Id,
                                    Type = upgrade.EffectType,
                                    Scope = upgrade.Persistence,
                                    Operation = upgrade.EffectOp,
                                    Value = upgrade.EffectValue,
                                    Supertype = upgrade.EffectSupertype,
                                };

                                speciality.Modifiers.Add(emod);
                                break;
                            case "c": // Contracts
                                var contract = _locate.LocateContract(game, upgrade.TargetId);

                                var cmod = new ModifierModel
                                {
                                    ApplyerId = upgrade.Id,
                                    Type = upgrade.EffectType,
                                    Scope = upgrade.Persistence,
                                    Operation = upgrade.EffectOp,
                                    Value = upgrade.EffectValue,
                                    Supertype = upgrade.EffectSupertype,
                                };

                                contract.Modifiers.Add(cmod);
                                break;
                            case "i": // Clicks
                                var click = _locate.LocateClick(game, upgrade.TargetId);

                                var imod = new ModifierModel
                                {
                                    ApplyerId = upgrade.Id,
                                    Type = upgrade.EffectType,
                                    Scope = upgrade.Persistence,
                                    Operation = upgrade.EffectOp,
                                    Value = upgrade.EffectValue,
                                    Supertype = upgrade.EffectSupertype,
                                };

                                click.Modifiers.Add(imod);
                                break;
                            case "r": // Resources
                                var resource = _locate.LocateResource(game, upgrade.TargetId);

                                var rmod = new ModifierModel
                                {
                                    ApplyerId = upgrade.Id,
                                    Type = upgrade.EffectType,
                                    Scope = upgrade.Persistence,
                                    Operation = upgrade.EffectOp,
                                    Value = upgrade.EffectValue,
                                    Supertype = upgrade.EffectSupertype,
                                };

                                resource.Modifiers.Add(rmod);
                                break;
                        }
                    }
                }, save: true);
            }
            if (type == ItemHelper.ItemType.Specialty)
            {
                var spec = _locate.LocateSpecialty(_game.CurrentGame, itemId);
                var Game = _game.CurrentGame;
                if (spec is null) return;

                string targetTypeId = spec.TargetId.Length >= 2
                    ? spec.TargetId.Substring(0, 1)
                    : spec.TargetId;

                await _game.Mutate(g =>
                {
                    var dur = Math.Max(0.001, spec.Duration);
                    var now = DateTimeOffset.UtcNow;

                    if (itemId != "e01")
                    {
                        switch (targetTypeId)
                        {
                            case "a": // All of a Kind
                                if (spec.TargetId == "aContracts")
                                {
                                    foreach (var acontract in game.Contracts)
                                    {
                                        var amod = new ModifierModel
                                        {
                                            ApplyerId = spec.Id,
                                            Type = spec.EffectType,
                                            Scope = spec.Persistence,
                                            Operation = spec.EffectOp,
                                            Value = spec.EffectValue,
                                            Supertype = spec.EffectSupertype,
                                        };

                                        acontract.Value.Modifiers.Add(amod);
                                    }
                                }
                                if (spec.TargetId == "aKnowledges")
                                {
                                    foreach (var aknow in game.Knowledges)
                                    {
                                        var amod = new ModifierModel
                                        {
                                            ApplyerId = spec.Id,
                                            Type = spec.EffectType,
                                            Scope = spec.Persistence,
                                            Operation = spec.EffectOp,
                                            Value = spec.EffectValue,
                                            Supertype = spec.EffectSupertype,
                                        };

                                        aknow.Value.Modifiers.Add(amod);
                                    }
                                }
                                if (spec.TargetId == "aCoins")
                                {
                                    foreach (var acoin in game.Coins)
                                    {
                                        var amod = new ModifierModel
                                        {
                                            ApplyerId = spec.Id,
                                            Type = spec.EffectType,
                                            Scope = spec.Persistence,
                                            Operation = spec.EffectOp,
                                            Value = spec.EffectValue,
                                            Supertype = spec.EffectSupertype,
                                        };

                                        acoin.Value.Modifiers.Add(amod);
                                    }
                                }
                                if (spec.TargetId == "aResources")
                                {
                                    foreach (var aresource in game.Resources)
                                    {
                                        var amod = new ModifierModel
                                        {
                                            ApplyerId = spec.Id,
                                            Type = spec.EffectType,
                                            Scope = spec.Persistence,
                                            Operation = spec.EffectOp,
                                            Value = spec.EffectValue,
                                            Supertype = spec.EffectSupertype,
                                        };

                                        aresource.Value.Modifiers.Add(amod);
                                    }
                                }
                                if (spec.TargetId == "aClicks")
                                {
                                    foreach (var aclick in game.Clicks)
                                    {
                                        var amod = new ModifierModel
                                        {
                                            ApplyerId = spec.Id,
                                            Type = spec.EffectType,
                                            Scope = spec.Persistence,
                                            Operation = spec.EffectOp,
                                            Value = spec.EffectValue,
                                            Supertype = spec.EffectSupertype,
                                        };

                                        aclick.Value.Modifiers.Add(amod);
                                    }
                                }
                                if (spec.TargetId == "aCharacters")
                                {
                                    foreach (var acharacters in game.Characters)
                                    {
                                        var amod = new ModifierModel
                                        {
                                            ApplyerId = spec.Id,
                                            Type = spec.EffectType,
                                            Scope = spec.Persistence,
                                            Operation = spec.EffectOp,
                                            Value = spec.EffectValue,
                                            Supertype = spec.EffectSupertype,
                                        };

                                        acharacters.Value.Modifiers.Add(amod);
                                    }
                                }
                                if (spec.TargetId == "aUpgrades")
                                {
                                    foreach (var aupgrades in game.Upgrades)
                                    {
                                        var amod = new ModifierModel
                                        {
                                            ApplyerId = spec.Id,
                                            Type = spec.EffectType,
                                            Scope = spec.Persistence,
                                            Operation = spec.EffectOp,
                                            Value = spec.EffectValue,
                                            Supertype = spec.EffectSupertype,
                                        };

                                        aupgrades.Value.Modifiers.Add(amod);
                                    }
                                }
                                if (spec.TargetId == "aSpecialties")
                                {
                                    foreach (var aspecialties in game.Specialties)
                                    {
                                        var amod = new ModifierModel
                                        {
                                            ApplyerId = spec.Id,
                                            Type = spec.EffectType,
                                            Scope = spec.Persistence,
                                            Operation = spec.EffectOp,
                                            Value = spec.EffectValue,
                                            Supertype = spec.EffectSupertype,
                                        };

                                        aspecialties.Value.Modifiers.Add(amod);
                                    }
                                }
                                break;
                            case "m": // Coins
                                var coins = _locate.LocateCoin(game, spec.TargetId);

                                var mmod = new ModifierModel
                                {
                                    ApplyerId = spec.Id,
                                    Type = spec.EffectType,
                                    Scope = spec.Persistence,
                                    Operation = spec.EffectOp,
                                    Value = spec.EffectValue,
                                    Supertype = spec.EffectSupertype,
                                };

                                coins.Modifiers.Add(mmod);
                                break;
                            case "p": // Characters
                                var character = _locate.LocateCharacter(game, spec.TargetId);

                                var pmod = new ModifierModel
                                {
                                    ApplyerId = spec.Id,
                                    Type = spec.EffectType,
                                    Scope = spec.Persistence,
                                    Operation = spec.EffectOp,
                                    Value = spec.EffectValue,
                                    Supertype = spec.EffectSupertype,
                                };

                                character.Modifiers.Add(pmod);
                                break;
                            case "k": // Knowledge
                                var knowledge = _locate.LocateKnowledge(game, spec.TargetId);

                                var kmod = new ModifierModel
                                {
                                    ApplyerId = spec.Id,
                                    Type = spec.EffectType,
                                    Scope = spec.Persistence,
                                    Operation = spec.EffectOp,
                                    Value = spec.EffectValue,
                                    Supertype = spec.EffectSupertype,
                                };

                                knowledge.Modifiers.Add(kmod);
                                break;
                            case "t": // Techs
                                var tech = _locate.LocateTech(game, spec.TargetId);

                                var tmod = new ModifierModel
                                {
                                    ApplyerId = spec.Id,
                                    Type = spec.EffectType,
                                    Scope = spec.Persistence,
                                    Operation = spec.EffectOp,
                                    Value = spec.EffectValue,
                                    Supertype = spec.EffectSupertype,
                                };

                                tech.Modifiers.Add(tmod);
                                break;
                            case "u": // Upgrades
                                var targetupgrade = _locate.LocateUpgrade(game, spec.TargetId);

                                var umod = new ModifierModel
                                {
                                    ApplyerId = spec.Id,
                                    Type = spec.EffectType,
                                    Scope = spec.Persistence,
                                    Operation = spec.EffectOp,
                                    Value = spec.EffectValue,
                                    Supertype = spec.EffectSupertype,
                                };

                                targetupgrade.Modifiers.Add(umod);
                                break;
                            case "l": // Locals
                                var local = _locate.LocateLocal(game, spec.TargetId);

                                var lmod = new ModifierModel
                                {
                                    ApplyerId = spec.Id,
                                    Type = spec.EffectType,
                                    Scope = spec.Persistence,
                                    Operation = spec.EffectOp,
                                    Value = spec.EffectValue,
                                    Supertype = spec.EffectSupertype,
                                };

                                local.Modifiers.Add(lmod);
                                break;
                            case "s": // Stages
                                var targetstage = _locate.LocateStage(game, spec.TargetId);

                                var smod = new ModifierModel
                                {
                                    ApplyerId = spec.Id,
                                    Type = spec.EffectType,
                                    Scope = spec.Persistence,
                                    Operation = spec.EffectOp,
                                    Value = spec.EffectValue,
                                    Supertype = spec.EffectSupertype,
                                };

                                targetstage.Modifiers.Add(smod);
                                break;
                            case "x": // Expansions
                                var expansion = _locate.LocateExpansion(game, spec.TargetId);

                                var xmod = new ModifierModel
                                {
                                    ApplyerId = spec.Id,
                                    Type = spec.EffectType,
                                    Scope = spec.Persistence,
                                    Operation = spec.EffectOp,
                                    Value = spec.EffectValue,
                                    Supertype = spec.EffectSupertype,
                                };

                                expansion.Modifiers.Add(xmod);
                                break;
                            case "d": // Expeditions
                                var expedition = _locate.LocateExpedition(game, spec.TargetId);

                                var dmod = new ModifierModel
                                {
                                    ApplyerId = spec.Id,
                                    Type = spec.EffectType,
                                    Scope = spec.Persistence,
                                    Operation = spec.EffectOp,
                                    Value = spec.EffectValue,
                                    Supertype = spec.EffectSupertype,
                                };

                                expedition.Modifiers.Add(dmod);
                                break;
                            case "o": // Traits
                                var trait = _locate.LocateTrait(game, spec.TargetId);

                                var omod = new ModifierModel
                                {
                                    ApplyerId = spec.Id,
                                    Type = spec.EffectType,
                                    Scope = spec.Persistence,
                                    Operation = spec.EffectOp,
                                    Value = spec.EffectValue,
                                    Supertype = spec.EffectSupertype,
                                };

                                trait.Modifiers.Add(omod);
                                break;
                            case "e": // Specialty
                                var speciality = _locate.LocateSpecialty(game, spec.TargetId);

                                var emod = new ModifierModel
                                {
                                    ApplyerId = spec.Id,
                                    Type = spec.EffectType,
                                    Scope = spec.Persistence,
                                    Operation = spec.EffectOp,
                                    Value = spec.EffectValue,
                                    Supertype = spec.EffectSupertype,
                                };

                                speciality.Modifiers.Add(emod);
                                break;
                            case "c": // Contracts
                                var contract = _locate.LocateContract(game, spec.TargetId);

                                var cmod = new ModifierModel
                                {
                                    ApplyerId = spec.Id,
                                    Type = spec.EffectType,
                                    Scope = spec.Persistence,
                                    Operation = spec.EffectOp,
                                    Value = spec.EffectValue,
                                    Supertype = spec.EffectSupertype,
                                };

                                contract.Modifiers.Add(cmod);
                                break;
                            case "i": // Clicks
                                var click = _locate.LocateClick(game, spec.TargetId);

                                var imod = new ModifierModel
                                {
                                    ApplyerId = spec.Id,
                                    Type = spec.EffectType,
                                    Scope = spec.Persistence,
                                    Operation = spec.EffectOp,
                                    Value = spec.EffectValue,
                                    Supertype = spec.EffectSupertype,
                                };

                                click.Modifiers.Add(imod);
                                break;
                            case "r": // Resources
                                var resource = _locate.LocateResource(game, spec.TargetId);

                                var rmod = new ModifierModel
                                {
                                    ApplyerId = spec.Id,
                                    Type = spec.EffectType,
                                    Scope = spec.Persistence,
                                    Operation = spec.EffectOp,
                                    Value = spec.EffectValue,
                                    Supertype = spec.EffectSupertype,
                                };

                                resource.Modifiers.Add(rmod);
                                break;
                        }
                    }

                    var timerModifiers = _modifier.GetModifiers(ItemHelper.ItemType.Specialty, spec.Id, stage.Id, EffectSupertype.Time);
                    double duration = (spec.Duration + timerModifiers.AddMod) * timerModifiers.MultMod;
                    _specialty.ActivateSpecialtyTimer(spec.Id, duration);
                }, save: true);

            }
            if (type == ItemHelper.ItemType.Trait)
            {
                await _game.Mutate(g =>
                {
                    var trait = _locate.LocateTrait(_game.CurrentGame, itemId);

                    string targetTypeId = trait.TargetId.Length >= 2
                        ? trait.TargetId.Substring(0, 1)
                        : trait.TargetId;
                    switch (targetTypeId)
                    {
                        case "a": // All of a Kind
                            if (trait.TargetId == "aContracts")
                            {
                                foreach (var acontract in game.Contracts)
                                {
                                    var amod = new ModifierModel
                                    {
                                        ApplyerId = trait.Id,
                                        Type = trait.EffectType,
                                        Scope = trait.Persistence,
                                        Operation = trait.EffectOp,
                                        Value = trait.EffectValue,
                                        Supertype = trait.EffectSupertype,
                                    };

                                    acontract.Value.Modifiers.Add(amod);
                                }
                            }
                            if (trait.TargetId == "aKnowledges")
                            {
                                foreach (var aknow in game.Knowledges)
                                {
                                    var amod = new ModifierModel
                                    {
                                        ApplyerId = trait.Id,
                                        Type = trait.EffectType,
                                        Scope = trait.Persistence,
                                        Operation = trait.EffectOp,
                                        Value = trait.EffectValue,
                                        Supertype = trait.EffectSupertype,
                                    };

                                    aknow.Value.Modifiers.Add(amod);
                                }
                            }
                            if (trait.TargetId == "aCoins")
                            {
                                foreach (var acoin in game.Coins)
                                {
                                    var amod = new ModifierModel
                                    {
                                        ApplyerId = trait.Id,
                                        Type = trait.EffectType,
                                        Scope = trait.Persistence,
                                        Operation = trait.EffectOp,
                                        Value = trait.EffectValue,
                                        Supertype = trait.EffectSupertype,
                                    };

                                    acoin.Value.Modifiers.Add(amod);
                                }
                            }
                            if (trait.TargetId == "aResources")
                            {
                                foreach (var aresource in game.Resources)
                                {
                                    var amod = new ModifierModel
                                    {
                                        ApplyerId = trait.Id,
                                        Type = trait.EffectType,
                                        Scope = trait.Persistence,
                                        Operation = trait.EffectOp,
                                        Value = trait.EffectValue,
                                        Supertype = trait.EffectSupertype,
                                    };

                                    aresource.Value.Modifiers.Add(amod);
                                }
                            }
                            if (trait.TargetId == "aClicks")
                            {
                                foreach (var aclick in game.Clicks)
                                {
                                    var amod = new ModifierModel
                                    {
                                        ApplyerId = trait.Id,
                                        Type = trait.EffectType,
                                        Scope = trait.Persistence,
                                        Operation = trait.EffectOp,
                                        Value = trait.EffectValue,
                                        Supertype = trait.EffectSupertype,
                                    };

                                    aclick.Value.Modifiers.Add(amod);
                                }
                            }
                            if (trait.TargetId == "aCharacters")
                            {
                                foreach (var acharacters in game.Characters)
                                {
                                    var amod = new ModifierModel
                                    {
                                        ApplyerId = trait.Id,
                                        Type = trait.EffectType,
                                        Scope = trait.Persistence,
                                        Operation = trait.EffectOp,
                                        Value = trait.EffectValue,
                                        Supertype = trait.EffectSupertype,
                                    };

                                    acharacters.Value.Modifiers.Add(amod);
                                }
                            }
                            if (trait.TargetId == "aUpgrades")
                            {
                                foreach (var aupgrades in game.Upgrades)
                                {
                                    var amod = new ModifierModel
                                    {
                                        ApplyerId = trait.Id,
                                        Type = trait.EffectType,
                                        Scope = trait.Persistence,
                                        Operation = trait.EffectOp,
                                        Value = trait.EffectValue,
                                        Supertype = trait.EffectSupertype,
                                    };

                                    aupgrades.Value.Modifiers.Add(amod);
                                }
                            }
                            if (trait.TargetId == "aSpecialties")
                            {
                                foreach (var aspecialties in game.Specialties)
                                {
                                    var amod = new ModifierModel
                                    {
                                        ApplyerId = trait.Id,
                                        Type = trait.EffectType,
                                        Scope = trait.Persistence,
                                        Operation = trait.EffectOp,
                                        Value = trait.EffectValue,
                                        Supertype = trait.EffectSupertype,
                                    };

                                    aspecialties.Value.Modifiers.Add(amod);
                                }
                            }
                            break;
                        case "m": // Coins
                            var coins = _locate.LocateCoin(game, trait.TargetId);

                            var mmod = new ModifierModel
                            {
                                ApplyerId = trait.Id,
                                Type = trait.EffectType,
                                Scope = trait.Persistence,
                                Operation = trait.EffectOp,
                                Value = trait.EffectValue,
                                Supertype = trait.EffectSupertype,
                            };

                            coins.Modifiers.Add(mmod);
                            break;
                        case "p": // Characters
                            var character = _locate.LocateCharacter(game, trait.TargetId);

                            var pmod = new ModifierModel
                            {
                                ApplyerId = trait.Id,
                                Type = trait.EffectType,
                                Scope = trait.Persistence,
                                Operation = trait.EffectOp,
                                Value = trait.EffectValue,
                                Supertype = trait.EffectSupertype,
                            };

                            character.Modifiers.Add(pmod);
                            break;
                        case "k": // Knowledge
                            var knowledge = _locate.LocateKnowledge(game, trait.TargetId);

                            var kmod = new ModifierModel
                            {
                                ApplyerId = trait.Id,
                                Type = trait.EffectType,
                                Scope = trait.Persistence,
                                Operation = trait.EffectOp,
                                Value = trait.EffectValue,
                                Supertype = trait.EffectSupertype,
                            };

                            knowledge.Modifiers.Add(kmod);
                            break;
                        case "t": // Techs
                            var tech = _locate.LocateTech(game, trait.TargetId);

                            var tmod = new ModifierModel
                            {
                                ApplyerId = trait.Id,
                                Type = trait.EffectType,
                                Scope = trait.Persistence,
                                Operation = trait.EffectOp,
                                Value = trait.EffectValue,
                                Supertype = trait.EffectSupertype,
                            };

                            tech.Modifiers.Add(tmod);
                            break;
                        case "u": // Upgrades
                            var targetupgrade = _locate.LocateUpgrade(game, trait.TargetId);

                            var umod = new ModifierModel
                            {
                                ApplyerId = trait.Id,
                                Type = trait.EffectType,
                                Scope = trait.Persistence,
                                Operation = trait.EffectOp,
                                Value = trait.EffectValue,
                                Supertype = trait.EffectSupertype,
                            };

                            targetupgrade.Modifiers.Add(umod);
                            break;
                        case "l": // Locals
                            var local = _locate.LocateLocal(game, trait.TargetId);

                            var lmod = new ModifierModel
                            {
                                ApplyerId = trait.Id,
                                Type = trait.EffectType,
                                Scope = trait.Persistence,
                                Operation = trait.EffectOp,
                                Value = trait.EffectValue,
                                Supertype = trait.EffectSupertype,
                            };

                            local.Modifiers.Add(lmod);
                            break;
                        case "s": // Stages
                            var targetstage = _locate.LocateStage(game, trait.TargetId);

                            var smod = new ModifierModel
                            {
                                ApplyerId = trait.Id,
                                Type = trait.EffectType,
                                Scope = trait.Persistence,
                                Operation = trait.EffectOp,
                                Value = trait.EffectValue,
                                Supertype = trait.EffectSupertype,
                            };

                            stage.Modifiers.Add(smod);
                            break;
                        case "x": // Expansions
                            var expansion = _locate.LocateExpansion(game, trait.TargetId);

                            var xmod = new ModifierModel
                            {
                                ApplyerId = trait.Id,
                                Type = trait.EffectType,
                                Scope = trait.Persistence,
                                Operation = trait.EffectOp,
                                Value = trait.EffectValue,
                                Supertype = trait.EffectSupertype,
                            };

                            expansion.Modifiers.Add(xmod);
                            break;
                        case "d": // Expeditions
                            var expedition = _locate.LocateExpedition(game, trait.TargetId);

                            var dmod = new ModifierModel
                            {
                                ApplyerId = trait.Id,
                                Type = trait.EffectType,
                                Scope = trait.Persistence,
                                Operation = trait.EffectOp,
                                Value = trait.EffectValue,
                                Supertype = trait.EffectSupertype,
                            };

                            expedition.Modifiers.Add(dmod);
                            break;
                        case "o": // Traits
                            var targettrait = _locate.LocateTrait(game, trait.TargetId);

                            var omod = new ModifierModel
                            {
                                ApplyerId = trait.Id,
                                Type = trait.EffectType,
                                Scope = trait.Persistence,
                                Operation = trait.EffectOp,
                                Value = trait.EffectValue,
                                Supertype = trait.EffectSupertype,
                            };

                            targettrait.Modifiers.Add(omod);
                            break;
                        case "e": // Speciality
                            var specialty = _locate.LocateSpecialty(game, trait.TargetId);

                            var emod = new ModifierModel
                            {
                                ApplyerId = trait.Id,
                                Type = trait.EffectType,
                                Scope = trait.Persistence,
                                Operation = trait.EffectOp,
                                Value = trait.EffectValue,
                                Supertype = trait.EffectSupertype,
                            };

                            specialty.Modifiers.Add(emod);
                            break;
                        case "c": // Contracts
                            var contract = _locate.LocateContract(game, trait.TargetId);

                            var cmod = new ModifierModel
                            {
                                ApplyerId = trait.Id,
                                Type = trait.EffectType,
                                Scope = trait.Persistence,
                                Operation = trait.EffectOp,
                                Value = trait.EffectValue,
                                Supertype = trait.EffectSupertype,
                            };

                            contract.Modifiers.Add(cmod);
                            break;
                        case "i": // Clicks
                            var click = _locate.LocateStageClick(game, trait.TargetId);

                            var imod = new ModifierModel
                            {
                                ApplyerId = trait.Id,
                                Type = trait.EffectType,
                                Scope = trait.Persistence,
                                Operation = trait.EffectOp,
                                Value = trait.EffectValue,
                                Supertype = trait.EffectSupertype,
                            };

                            click.Modifiers.Add(imod);
                            break;
                        case "r": // Resources
                            var resource = _locate.LocateResource(game, trait.TargetId);

                            var rmod = new ModifierModel
                            {
                                ApplyerId = trait.Id,
                                Type = trait.EffectType,
                                Scope = trait.Persistence,
                                Operation = trait.EffectOp,
                                Value = trait.EffectValue,
                                Supertype = trait.EffectSupertype,
                            };

                            resource.Modifiers.Add(rmod);
                            break;
                    }

                }, save: true);
            }
        }
    }
}
