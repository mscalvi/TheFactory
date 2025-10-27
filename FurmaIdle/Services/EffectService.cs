using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Services;
using System.Diagnostics.Contracts;
using System.Threading.Channels;
using System.Xml.Linq;
using static FurmaIdle.Helpers.EffectHelper;
using static FurmaIdle.Helpers.ItemHelper;
using static FurmaIdle.Helpers.LogHelper;

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

        public EffectService(ICurrentGameService Game, IUnlockService Unlock, IUiLogService Log, ILocateService Locate)
        {
            _game = Game;
            _unlock = Unlock;
            _locate = Locate;
            _log = Log;
        }

        public async Task ApplyEffect(ItemHelper.ItemType type, string itemId, string stageId)
        {
            var game = _game.CurrentGame;
            var stage = _locate.LocateStage(game, stageId);

            ModifierModel newModifier = new ModifierModel();

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
                                        newModifier.ApplyerId = itemId;
                                        newModifier.Type = upgrade.EffectType;
                                        newModifier.Scope = upgrade.Persistence;
                                        newModifier.Operation = upgrade.EffectOp;
                                        newModifier.Value = upgrade.EffectValue;

                                        acontract.Value.Modifiers.Add(newModifier);
                                    }
                                }
                                if (upgrade.TargetId == "aKnowledges")
                                {
                                    foreach (var aknow in game.Knowledges)
                                    {
                                        newModifier.ApplyerId = itemId;
                                        newModifier.Type = upgrade.EffectType;
                                        newModifier.Scope = upgrade.Persistence;
                                        newModifier.Operation = upgrade.EffectOp;
                                        newModifier.Value = upgrade.EffectValue;

                                        aknow.Value.Modifiers.Add(newModifier);
                                    }
                                }
                                if (upgrade.TargetId == "aCoins")
                                {
                                    foreach (var acoin in game.Coins)
                                    {
                                        newModifier.ApplyerId = itemId;
                                        newModifier.Type = upgrade.EffectType;
                                        newModifier.Scope = upgrade.Persistence;
                                        newModifier.Operation = upgrade.EffectOp;
                                        newModifier.Value = upgrade.EffectValue;

                                        acoin.Value.Modifiers.Add(newModifier);
                                    }
                                }
                                if (upgrade.TargetId == "aResources")
                                {
                                    foreach (var aresource in game.Resources)
                                    {
                                        newModifier.ApplyerId = itemId;
                                        newModifier.Type = upgrade.EffectType;
                                        newModifier.Scope = upgrade.Persistence;
                                        newModifier.Operation = upgrade.EffectOp;
                                        newModifier.Value = upgrade.EffectValue;

                                        aresource.Value.Modifiers.Add(newModifier);
                                    }
                                }
                                if (upgrade.TargetId == "aClicks")
                                {
                                    foreach (var aclick in game.Clicks)
                                    {
                                        newModifier.ApplyerId = itemId;
                                        newModifier.Type = upgrade.EffectType;
                                        newModifier.Scope = upgrade.Persistence;
                                        newModifier.Operation = upgrade.EffectOp;
                                        newModifier.Value = upgrade.EffectValue;

                                        aclick.Value.Modifiers.Add(newModifier);
                                    }
                                }
                                if (upgrade.TargetId == "aCharacters")
                                {
                                    foreach (var acharacters in game.Characters)
                                    {
                                        newModifier.ApplyerId = itemId;
                                        newModifier.Type = upgrade.EffectType;
                                        newModifier.Scope = upgrade.Persistence;
                                        newModifier.Operation = upgrade.EffectOp;
                                        newModifier.Value = upgrade.EffectValue;

                                        acharacters.Value.Modifiers.Add(newModifier);
                                    }
                                }
                                if (upgrade.TargetId == "aUpgrades")
                                {
                                    foreach (var aupgrades in game.Upgrades)
                                    {
                                        newModifier.ApplyerId = itemId;
                                        newModifier.Type = upgrade.EffectType;
                                        newModifier.Scope = upgrade.Persistence;
                                        newModifier.Operation = upgrade.EffectOp;
                                        newModifier.Value = upgrade.EffectValue;

                                        aupgrades.Value.Modifiers.Add(newModifier);
                                    }
                                }
                                break;
                            case "m": // Coins
                                var coins = _locate.LocateCoin(game, upgrade.TargetId);

                                newModifier.ApplyerId = itemId;
                                newModifier.Type = upgrade.EffectType;
                                newModifier.Scope = upgrade.Persistence;
                                newModifier.Operation = upgrade.EffectOp;
                                newModifier.Value = upgrade.EffectValue;

                                coins.Modifiers.Add(newModifier);
                                break;
                            case "p": // Characters
                                var character = _locate.LocateCharacter(game, upgrade.TargetId);

                                newModifier.ApplyerId = itemId;
                                newModifier.Type = upgrade.EffectType;
                                newModifier.Scope = upgrade.Persistence;
                                newModifier.Operation = upgrade.EffectOp;
                                newModifier.Value = upgrade.EffectValue;

                                character.Modifiers.Add(newModifier);
                                break;
                            case "k": // Knowledge
                                var knowledge = _locate.LocateKnowledge(game, upgrade.TargetId);

                                newModifier.ApplyerId = itemId;
                                newModifier.Type = upgrade.EffectType;
                                newModifier.Scope = upgrade.Persistence;
                                newModifier.Operation = upgrade.EffectOp;
                                newModifier.Value = upgrade.EffectValue;

                                knowledge.Modifiers.Add(newModifier);
                                break;
                            case "t": // Techs
                                var tech = _locate.LocateTech(game, upgrade.TargetId);

                                newModifier.ApplyerId = itemId;
                                newModifier.Type = upgrade.EffectType;
                                newModifier.Scope = upgrade.Persistence;
                                newModifier.Operation = upgrade.EffectOp;
                                newModifier.Value = upgrade.EffectValue;

                                tech.Modifiers.Add(newModifier);
                                break;
                            case "u": // Upgrades
                                var targetupgrade = _locate.LocateUpgrade(game, upgrade.TargetId);

                                newModifier.ApplyerId = itemId;
                                newModifier.Type = upgrade.EffectType;
                                newModifier.Scope = upgrade.Persistence;
                                newModifier.Operation = upgrade.EffectOp;
                                newModifier.Value = upgrade.EffectValue;

                                targetupgrade.Modifiers.Add(newModifier);
                                break;
                            case "l": // Locals
                                var local = _locate.LocateLocal(game, upgrade.TargetId);

                                newModifier.ApplyerId = itemId;
                                newModifier.Type = upgrade.EffectType;
                                newModifier.Scope = upgrade.Persistence;
                                newModifier.Operation = upgrade.EffectOp;
                                newModifier.Value = upgrade.EffectValue;

                                local.Modifiers.Add(newModifier);
                                break;
                            case "s": // Stages
                                var targetstage = _locate.LocateStage(game, upgrade.TargetId);

                                newModifier.ApplyerId = itemId;
                                newModifier.Type = upgrade.EffectType;
                                newModifier.Scope = upgrade.Persistence;
                                newModifier.Operation = upgrade.EffectOp;
                                newModifier.Value = upgrade.EffectValue;

                                targetstage.Modifiers.Add(newModifier);
                                break;
                            case "x": // Expansions
                                var expansion = _locate.LocateExpansion(game, upgrade.TargetId);

                                newModifier.ApplyerId = itemId;
                                newModifier.Type = upgrade.EffectType;
                                newModifier.Scope = upgrade.Persistence;
                                newModifier.Operation = upgrade.EffectOp;
                                newModifier.Value = upgrade.EffectValue;

                                expansion.Modifiers.Add(newModifier);
                                break;
                            case "d": // Expeditions
                                var expedition = _locate.LocateExpedition(game, upgrade.TargetId);

                                newModifier.ApplyerId = itemId;
                                newModifier.Type = upgrade.EffectType;
                                newModifier.Scope = upgrade.Persistence;
                                newModifier.Operation = upgrade.EffectOp;
                                newModifier.Value = upgrade.EffectValue;

                                expedition.Modifiers.Add(newModifier);
                                break;
                            case "o": // Traits
                                var trait = _locate.LocateTrait(game, upgrade.TargetId);

                                newModifier.ApplyerId = itemId;
                                newModifier.Type = upgrade.EffectType;
                                newModifier.Scope = upgrade.Persistence;
                                newModifier.Operation = upgrade.EffectOp;
                                newModifier.Value = upgrade.EffectValue;

                                trait.Modifiers.Add(newModifier);
                                break;
                            case "e": // Specialty
                                var speciality = _locate.LocateSpecialty(game, upgrade.TargetId);

                                newModifier.ApplyerId = itemId;
                                newModifier.Type = upgrade.EffectType;
                                newModifier.Scope = upgrade.Persistence;
                                newModifier.Operation = upgrade.EffectOp;
                                newModifier.Value = upgrade.EffectValue;

                                speciality.Modifiers.Add(newModifier);
                                break;
                            case "c": // Contracts
                                var contract = _locate.LocateContract(game, upgrade.TargetId);

                                newModifier.ApplyerId = itemId;
                                newModifier.Type = upgrade.EffectType;
                                newModifier.Scope = upgrade.Persistence;
                                newModifier.Operation = upgrade.EffectOp;
                                newModifier.Value = upgrade.EffectValue;

                                contract.Modifiers.Add(newModifier);
                                break;
                            case "i": // Clicks
                                var click = _locate.LocateStageClick(game, upgrade.TargetId);

                                newModifier.ApplyerId = itemId;
                                newModifier.Type = upgrade.EffectType;
                                newModifier.Scope = upgrade.Persistence;
                                newModifier.Operation = upgrade.EffectOp;
                                newModifier.Value = upgrade.EffectValue;

                                click.Modifiers.Add(newModifier);
                                break;
                            case "r": // Resources
                                var resource = _locate.LocateResource(game, upgrade.TargetId);

                                newModifier.ApplyerId = itemId;
                                newModifier.Type = upgrade.EffectType;
                                newModifier.Scope = upgrade.Persistence;
                                newModifier.Operation = upgrade.EffectOp;
                                newModifier.Value = upgrade.EffectValue;

                                resource.Modifiers.Add(newModifier);
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

                    switch (targetTypeId)
                    {
                        case "a": // All of a Kind
                            if (spec.TargetId == "aContracts")
                            {
                                foreach (var acontract in game.Contracts)
                                {
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = spec.EffectType;
                                    newModifier.Scope = spec.Persistence;
                                    newModifier.Operation = spec.EffectOp;
                                    newModifier.Value = spec.EffectValue;

                                    acontract.Value.Modifiers.Add(newModifier);
                                }
                            }
                            if (spec.TargetId == "aKnowledges")
                            {
                                foreach (var aknow in game.Knowledges)
                                {
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = spec.EffectType;
                                    newModifier.Scope = spec.Persistence;
                                    newModifier.Operation = spec.EffectOp;
                                    newModifier.Value = spec.EffectValue;

                                    aknow.Value.Modifiers.Add(newModifier);
                                }
                            }
                            if (spec.TargetId == "aCoins")
                            {
                                foreach (var acoin in game.Coins)
                                {
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = spec.EffectType;
                                    newModifier.Scope = spec.Persistence;
                                    newModifier.Operation = spec.EffectOp;
                                    newModifier.Value = spec.EffectValue;

                                    acoin.Value.Modifiers.Add(newModifier);
                                }
                            }
                            if (spec.TargetId == "aResources")
                            {
                                foreach (var aresource in game.Resources)
                                {
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = spec.EffectType;
                                    newModifier.Scope = spec.Persistence;
                                    newModifier.Operation = spec.EffectOp;
                                    newModifier.Value = spec.EffectValue;

                                    aresource.Value.Modifiers.Add(newModifier);
                                }
                            }
                            if (spec.TargetId == "aClicks")
                            {
                                foreach (var aclick in game.Clicks)
                                {
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = spec.EffectType;
                                    newModifier.Scope = spec.Persistence;
                                    newModifier.Operation = spec.EffectOp;
                                    newModifier.Value = spec.EffectValue;

                                    aclick.Value.Modifiers.Add(newModifier);
                                }
                            }
                            if (spec.TargetId == "aCharacters")
                            {
                                foreach (var acharacters in game.Characters)
                                {
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = spec.EffectType;
                                    newModifier.Scope = spec.Persistence;
                                    newModifier.Operation = spec.EffectOp;
                                    newModifier.Value = spec.EffectValue;

                                    acharacters.Value.Modifiers.Add(newModifier);
                                }
                            }
                            if (spec.TargetId == "aUpgrades")
                            {
                                foreach (var aupgrades in game.Upgrades)
                                {
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = spec.EffectType;
                                    newModifier.Scope = spec.Persistence;
                                    newModifier.Operation = spec.EffectOp;
                                    newModifier.Value = spec.EffectValue;

                                    aupgrades.Value.Modifiers.Add(newModifier);
                                }
                            }
                            break;
                        case "z": // All of a Kind in a Stage
                            if (spec.TargetId == "zContracts")
                            {
                                foreach (var contractId in stage.ActiveContracts)
                                {
                                    var targetcontract = _locate.LocateContract(game, contractId.Key);

                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = spec.EffectType;
                                    newModifier.Scope = spec.Persistence;
                                    newModifier.Operation = spec.EffectOp;
                                    newModifier.Value = spec.EffectValue;

                                    targetcontract.Modifiers.Add(newModifier);
                                }
                            }
                            if (spec.TargetId == "zCharacters")
                            {
                                foreach (var characterId in stage.Expedition.PartyIds)
                                {
                                    var character = _locate.LocateCharacter(game, characterId);

                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = spec.EffectType;
                                    newModifier.Scope = spec.Persistence;
                                    newModifier.Operation = spec.EffectOp;
                                    newModifier.Value = spec.EffectValue;

                                    character.Modifiers.Add(newModifier);
                                }
                            }
                            if (spec.TargetId == "zSpecialties")
                            {
                                foreach (var characterId in stage.Expedition.PartyIds)
                                {
                                    var character = _locate.LocateCharacter(game, characterId);
                                    var specialty = _locate.LocateSpecialty(game, character.SpecialtyId);

                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = spec.EffectType;
                                    newModifier.Scope = spec.Persistence;
                                    newModifier.Operation = spec.EffectOp;
                                    newModifier.Value = spec.EffectValue;

                                    specialty.Modifiers.Add(newModifier);
                                }
                            }
                            break;
                        case "m": // Coins
                            var coins = _locate.LocateCoin(game, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            coins.Modifiers.Add(newModifier);
                            break;
                        case "p": // Characters
                            var characte = _locate.LocateCharacter(game, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            characte.Modifiers.Add(newModifier);
                            break;
                        case "k": // Knowledge
                            var knowledge = _locate.LocateKnowledge(game, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            knowledge.Modifiers.Add(newModifier);
                            break;
                        case "t": // Techs
                            var tech = _locate.LocateTech(game, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            tech.Modifiers.Add(newModifier);
                            break;
                        case "u": // Upgrades
                            var targetupgrade = _locate.LocateUpgrade(game, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            targetupgrade.Modifiers.Add(newModifier);
                            break;
                        case "l": // Locals
                            var local = _locate.LocateLocal(game, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            local.Modifiers.Add(newModifier);
                            break;
                        case "s": // Stages
                            var targetstage = _locate.LocateStage(game, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            targetstage.Modifiers.Add(newModifier);
                            break;
                        case "x": // Expansions
                            var expansion = _locate.LocateExpansion(game, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            expansion.Modifiers.Add(newModifier);
                            break;
                        case "d": // Expeditions
                            var expedition = _locate.LocateExpedition(game, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            expedition.Modifiers.Add(newModifier);
                            break;
                        case "o": // Traits
                            var trait = _locate.LocateTrait(game, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            trait.Modifiers.Add(newModifier);
                            break;
                        case "e": // Specialty
                            var speciality = _locate.LocateSpecialty(game, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            speciality.Modifiers.Add(newModifier);
                            break;
                        case "c": // Contracts
                            var contract = _locate.LocateContract(game, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            contract.Modifiers.Add(newModifier);
                            break;
                        case "i": // Clicks
                            var click = _locate.LocateStageClick(game, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            click.Modifiers.Add(newModifier);
                            break;
                        case "r": // Resources
                            var resource = _locate.LocateResource(game, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            resource.Modifiers.Add(newModifier);
                            break;
                    }

                }, save: true);
            }
            if (type == ItemHelper.ItemType.Trait)
            {
                await _game.Mutate(g =>
                {
                    var trait = _locate.LocateTrait(_game.CurrentGame, itemId);

                    ModifierModel newModifier = new ModifierModel();
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
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = trait.EffectType;
                                    newModifier.Scope = trait.Persistence;
                                    newModifier.Operation = trait.EffectOp;
                                    newModifier.Value = trait.EffectValue;

                                    acontract.Value.Modifiers.Add(newModifier);
                                }
                            }
                            if (trait.TargetId == "aKnowledges")
                            {
                                foreach (var aknow in game.Knowledges)
                                {
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = trait.EffectType;
                                    newModifier.Scope = trait.Persistence;
                                    newModifier.Operation = trait.EffectOp;
                                    newModifier.Value = trait.EffectValue;

                                    aknow.Value.Modifiers.Add(newModifier);
                                }
                            }
                            if (trait.TargetId == "aCoins")
                            {
                                foreach (var acoin in game.Coins)
                                {
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = trait.EffectType;
                                    newModifier.Scope = trait.Persistence;
                                    newModifier.Operation = trait.EffectOp;
                                    newModifier.Value = trait.EffectValue;

                                    acoin.Value.Modifiers.Add(newModifier);
                                }
                            }
                            if (trait.TargetId == "aResources")
                            {
                                foreach (var aresource in game.Resources)
                                {
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = trait.EffectType;
                                    newModifier.Scope = trait.Persistence;
                                    newModifier.Operation = trait.EffectOp;
                                    newModifier.Value = trait.EffectValue;

                                    aresource.Value.Modifiers.Add(newModifier);
                                }
                            }
                            if (trait.TargetId == "aClicks")
                            {
                                foreach (var aclick in game.Clicks)
                                {
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = trait.EffectType;
                                    newModifier.Scope = trait.Persistence;
                                    newModifier.Operation = trait.EffectOp;
                                    newModifier.Value = trait.EffectValue;

                                    aclick.Value.Modifiers.Add(newModifier);
                                }
                            }
                            if (trait.TargetId == "aCharacters")
                            {
                                foreach (var acharacters in game.Characters)
                                {
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = trait.EffectType;
                                    newModifier.Scope = trait.Persistence;
                                    newModifier.Operation = trait.EffectOp;
                                    newModifier.Value = trait.EffectValue;

                                    acharacters.Value.Modifiers.Add(newModifier);
                                }
                            }
                            if (trait.TargetId == "aUpgrades")
                            {
                                foreach (var aupgrades in game.Upgrades)
                                {
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = trait.EffectType;
                                    newModifier.Scope = trait.Persistence;
                                    newModifier.Operation = trait.EffectOp;
                                    newModifier.Value = trait.EffectValue;

                                    aupgrades.Value.Modifiers.Add(newModifier);
                                }
                            }
                            break;
                        case "z": // All of a Kind in a Stage
                            if (trait.TargetId == "zContracts")
                            {
                                foreach (var contractId in stage.ActiveContracts)
                                {
                                    var targetcontract = _locate.LocateContract(game, contractId.Key);

                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = trait.EffectType;
                                    newModifier.Scope = trait.Persistence;
                                    newModifier.Operation = trait.EffectOp;
                                    newModifier.Value = trait.EffectValue;

                                    targetcontract.Modifiers.Add(newModifier);
                                }
                            }
                            if (trait.TargetId == "zCharacters")
                            {
                                foreach (var characterId in stage.Expedition.PartyIds)
                                {
                                    var character = _locate.LocateCharacter(game, characterId);

                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = trait.EffectType;
                                    newModifier.Scope = trait.Persistence;
                                    newModifier.Operation = trait.EffectOp;
                                    newModifier.Value = trait.EffectValue;

                                    character.Modifiers.Add(newModifier);
                                }
                            }
                            if (trait.TargetId == "zSpecialties")
                            {
                                foreach (var characterId in stage.Expedition.PartyIds)
                                {
                                    var character = _locate.LocateCharacter(game, characterId);
                                    var targetSpecialty = _locate.LocateSpecialty(game, character.SpecialtyId);

                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = trait.EffectType;
                                    newModifier.Scope = trait.Persistence;
                                    newModifier.Operation = trait.EffectOp;
                                    newModifier.Value = trait.EffectValue;

                                    targetSpecialty.Modifiers.Add(newModifier);
                                }
                            }
                            break;
                        case "m": // Coins
                            var coins = _locate.LocateCoin(game, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            coins.Modifiers.Add(newModifier);
                            break;
                        case "p": // Characters
                            var characte = _locate.LocateCharacter(game, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            characte.Modifiers.Add(newModifier);
                            break;
                        case "k": // Knowledge
                            var knowledge = _locate.LocateKnowledge(game, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            knowledge.Modifiers.Add(newModifier);
                            break;
                        case "t": // Techs
                            var tech = _locate.LocateTech(game, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            tech.Modifiers.Add(newModifier);
                            break;
                        case "u": // Upgrades
                            var targetupgrade = _locate.LocateUpgrade(game, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            targetupgrade.Modifiers.Add(newModifier);
                            break;
                        case "l": // Locals
                            var local = _locate.LocateLocal(game, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            local.Modifiers.Add(newModifier);
                            break;
                        case "s": // Stages
                            var targetstage = _locate.LocateStage(game, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            targetstage.Modifiers.Add(newModifier);
                            break;
                        case "x": // Expansions
                            var expansion = _locate.LocateExpansion(game, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            expansion.Modifiers.Add(newModifier);
                            break;
                        case "d": // Expeditions
                            var expedition = _locate.LocateExpedition(game, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            expedition.Modifiers.Add(newModifier);
                            break;
                        case "o": // Traits
                            var targettrait = _locate.LocateTrait(game, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            targettrait.Modifiers.Add(newModifier);
                            break;
                        case "e": // Speciality
                            var specialty = _locate.LocateSpecialty(game, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            specialty.Modifiers.Add(newModifier);
                            break;
                        case "c": // Contracts
                            var contract = _locate.LocateContract(game, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            contract.Modifiers.Add(newModifier);
                            break;
                        case "i": // Clicks
                            var click = _locate.LocateStageClick(game, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            click.Modifiers.Add(newModifier);
                            break;
                        case "r": // Resources
                            var resource = _locate.LocateResource(game, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            resource.Modifiers.Add(newModifier);
                            break;
                    }

                }, save: true);
            }
        }
    }
}
