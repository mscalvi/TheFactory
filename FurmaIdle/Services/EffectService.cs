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
            var g = _game.CurrentGame;

            if (type == ItemHelper.ItemType.Upgrade)
            {
                var upgrade = _locate.LocateUpgrade(_game.CurrentGame, itemId);
                var game = _game.CurrentGame;

                ModifierModel newModifier = new ModifierModel();
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

                await _game.Mutate(g =>
                {
                    var stage = _locate.LocateStage(g, stageId);

                    switch (targetTypeId)
                    {
                        case "a": // All of a Kind
                            if (upgrade.TargetId == "aContracts")
                            {
                                foreach (var acontract in g.Contracts)
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
                                foreach (var aknow in g.Knowledges)
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
                                foreach (var acoin in g.Coins)
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
                                foreach (var aresource in g.Resources)
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
                                foreach (var aclick in g.Clicks)
                                {
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = upgrade.EffectType;
                                    newModifier.Scope = upgrade.Persistence;
                                    newModifier.Operation = upgrade.EffectOp;
                                    newModifier.Value = upgrade.EffectValue;

                                    aclick.Value.Modifiers.Add(newModifier);
                                }
                            }
                            break;
                        case "m": // Coins
                            var coins = _locate.LocateCoin(g, upgrade.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = upgrade.EffectType;
                            newModifier.Scope = upgrade.Persistence;
                            newModifier.Operation = upgrade.EffectOp;
                            newModifier.Value = upgrade.EffectValue;

                            coins.Modifiers.Add(newModifier);
                            break;
                        case "p": // Characters
                            var character = _locate.LocateCharacter(g, upgrade.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = upgrade.EffectType;
                            newModifier.Scope = upgrade.Persistence;
                            newModifier.Operation = upgrade.EffectOp;
                            newModifier.Value = upgrade.EffectValue;

                            character.Modifiers.Add(newModifier);
                            break;
                        case "k": // Knowledge
                            var knowledge = _locate.LocateKnowledge(g, upgrade.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = upgrade.EffectType;
                            newModifier.Scope = upgrade.Persistence;
                            newModifier.Operation = upgrade.EffectOp;
                            newModifier.Value = upgrade.EffectValue;

                            knowledge.Modifiers.Add(newModifier);
                            break;
                        case "t": // Techs
                            var tech = _locate.LocateTech(g, upgrade.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = upgrade.EffectType;
                            newModifier.Scope = upgrade.Persistence;
                            newModifier.Operation = upgrade.EffectOp;
                            newModifier.Value = upgrade.EffectValue;

                            tech.Modifiers.Add(newModifier);
                            break;
                        case "u": // Upgrades
                            var targetupgrade = _locate.LocateUpgrade(g, upgrade.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = upgrade.EffectType;
                            newModifier.Scope = upgrade.Persistence;
                            newModifier.Operation = upgrade.EffectOp;
                            newModifier.Value = upgrade.EffectValue;

                            targetupgrade.Modifiers.Add(newModifier);
                            break;
                        case "l": // Locals
                            var local = _locate.LocateLocal(g, upgrade.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = upgrade.EffectType;
                            newModifier.Scope = upgrade.Persistence;
                            newModifier.Operation = upgrade.EffectOp;
                            newModifier.Value = upgrade.EffectValue;

                            local.Modifiers.Add(newModifier);
                            break;
                        case "s": // Stages
                            var targetstage = _locate.LocateStage(g, upgrade.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = upgrade.EffectType;
                            newModifier.Scope = upgrade.Persistence;
                            newModifier.Operation = upgrade.EffectOp;
                            newModifier.Value = upgrade.EffectValue;

                            targetstage.Modifiers.Add(newModifier);
                            break;
                        case "x": // Expansions
                            var expansion = _locate.LocateExpansion(g, upgrade.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = upgrade.EffectType;
                            newModifier.Scope = upgrade.Persistence;
                            newModifier.Operation = upgrade.EffectOp;
                            newModifier.Value = upgrade.EffectValue;

                            expansion.Modifiers.Add(newModifier);
                            break;
                        case "d": // Expeditions
                            var expedition = _locate.LocateExpedition(g, upgrade.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = upgrade.EffectType;
                            newModifier.Scope = upgrade.Persistence;
                            newModifier.Operation = upgrade.EffectOp;
                            newModifier.Value = upgrade.EffectValue;

                            expedition.Modifiers.Add(newModifier);
                            break;
                        case "o": // Traits
                            var trait = _locate.LocateTrait(g, upgrade.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = upgrade.EffectType;
                            newModifier.Scope = upgrade.Persistence;
                            newModifier.Operation = upgrade.EffectOp;
                            newModifier.Value = upgrade.EffectValue;

                            trait.Modifiers.Add(newModifier);
                            break;
                        case "e": // Specialty
                            var speciality = _locate.LocateSpecialty(g, upgrade.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = upgrade.EffectType;
                            newModifier.Scope = upgrade.Persistence;
                            newModifier.Operation = upgrade.EffectOp;
                            newModifier.Value = upgrade.EffectValue;

                            speciality.Modifiers.Add(newModifier);
                            break;
                        case "c": // Contracts
                            var contract = _locate.LocateContract(g, upgrade.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = upgrade.EffectType;
                            newModifier.Scope = upgrade.Persistence;
                            newModifier.Operation = upgrade.EffectOp;
                            newModifier.Value = upgrade.EffectValue;

                            contract.Modifiers.Add(newModifier);
                            break;
                        case "i": // Clicks
                            var click = _locate.LocateStageClick(g, upgrade.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = upgrade.EffectType;
                            newModifier.Scope = upgrade.Persistence;
                            newModifier.Operation = upgrade.EffectOp;
                            newModifier.Value = upgrade.EffectValue;

                            click.Modifiers.Add(newModifier);
                            break;
                        case "r": // Resources
                            var resource = _locate.LocateResource(g, upgrade.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = upgrade.EffectType;
                            newModifier.Scope = upgrade.Persistence;
                            newModifier.Operation = upgrade.EffectOp;
                            newModifier.Value = upgrade.EffectValue;

                            resource.Modifiers.Add(newModifier);
                            break;
                    }
                }, save: true);
            }
            if (type == ItemHelper.ItemType.Specialty)
            {
                var spec = _locate.LocateSpecialty(_game.CurrentGame, itemId);
                var stage = _locate.LocateStage(_game.CurrentGame, stageId);
                var Game = _game.CurrentGame;
                if (spec is null) return;

                ModifierModel newModifier = new ModifierModel();
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
                                foreach (var acontract in g.Contracts)
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
                                foreach (var aknow in g.Knowledges)
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
                                foreach (var acoin in g.Coins)
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
                                foreach (var aresource in g.Resources)
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
                                foreach (var aclick in g.Clicks)
                                {
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = spec.EffectType;
                                    newModifier.Scope = spec.Persistence;
                                    newModifier.Operation = spec.EffectOp;
                                    newModifier.Value = spec.EffectValue;

                                    aclick.Value.Modifiers.Add(newModifier);
                                }
                            }
                            break;
                        case "m": // Coins
                            var coins = _locate.LocateCoin(g, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            coins.Modifiers.Add(newModifier);
                            break;
                        case "p": // Characters
                            var characte = _locate.LocateCharacter(g, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            characte.Modifiers.Add(newModifier);
                            break;
                        case "k": // Knowledge
                            var knowledge = _locate.LocateKnowledge(g, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            knowledge.Modifiers.Add(newModifier);
                            break;
                        case "t": // Techs
                            var tech = _locate.LocateTech(g, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            tech.Modifiers.Add(newModifier);
                            break;
                        case "u": // Upgrades
                            var targetupgrade = _locate.LocateUpgrade(g, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            targetupgrade.Modifiers.Add(newModifier);
                            break;
                        case "l": // Locals
                            var local = _locate.LocateLocal(g, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            local.Modifiers.Add(newModifier);
                            break;
                        case "s": // Stages
                            var targetstage = _locate.LocateStage(g, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            targetstage.Modifiers.Add(newModifier);
                            break;
                        case "x": // Expansions
                            var expansion = _locate.LocateExpansion(g, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            expansion.Modifiers.Add(newModifier);
                            break;
                        case "d": // Expeditions
                            var expedition = _locate.LocateExpedition(g, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            expedition.Modifiers.Add(newModifier);
                            break;
                        case "o": // Traits
                            var trait = _locate.LocateTrait(g, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            trait.Modifiers.Add(newModifier);
                            break;
                        case "e": // Specialty
                            var speciality = _locate.LocateSpecialty(g, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            speciality.Modifiers.Add(newModifier);
                            break;
                        case "c": // Contracts
                            var contract = _locate.LocateContract(g, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            contract.Modifiers.Add(newModifier);
                            break;
                        case "i": // Clicks
                            var click = _locate.LocateStageClick(g, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            click.Modifiers.Add(newModifier);
                            break;
                        case "r": // Resources
                            var resource = _locate.LocateResource(g, spec.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = spec.EffectType;
                            newModifier.Scope = spec.Persistence;
                            newModifier.Operation = spec.EffectOp;
                            newModifier.Value = spec.EffectValue;

                            resource.Modifiers.Add(newModifier);
                            break;
                    }

                    _specTimers[itemId] = (now.AddSeconds(dur), dur);

                    Console.WriteLine($"[Purchase] Specialty {itemId} ativa por {dur:0.##}s");

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
                                foreach (var acontract in g.Contracts)
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
                                foreach (var aknow in g.Knowledges)
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
                                foreach (var acoin in g.Coins)
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
                                foreach (var aresource in g.Resources)
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
                                foreach (var aclick in g.Clicks)
                                {
                                    newModifier.ApplyerId = itemId;
                                    newModifier.Type = trait.EffectType;
                                    newModifier.Scope = trait.Persistence;
                                    newModifier.Operation = trait.EffectOp;
                                    newModifier.Value = trait.EffectValue;

                                    aclick.Value.Modifiers.Add(newModifier);
                                }
                            }
                            break;
                        case "m": // Coins
                            var coins = _locate.LocateCoin(g, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            coins.Modifiers.Add(newModifier);
                            break;
                        case "p": // Characters
                            var characte = _locate.LocateCharacter(g, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            characte.Modifiers.Add(newModifier);
                            break;
                        case "k": // Knowledge
                            var knowledge = _locate.LocateKnowledge(g, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            knowledge.Modifiers.Add(newModifier);
                            break;
                        case "t": // Techs
                            var tech = _locate.LocateTech(g, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            tech.Modifiers.Add(newModifier);
                            break;
                        case "u": // Upgrades
                            var targetupgrade = _locate.LocateUpgrade(g, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            targetupgrade.Modifiers.Add(newModifier);
                            break;
                        case "l": // Locals
                            var local = _locate.LocateLocal(g, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            local.Modifiers.Add(newModifier);
                            break;
                        case "s": // Stages
                            var targetstage = _locate.LocateStage(g, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            targetstage.Modifiers.Add(newModifier);
                            break;
                        case "x": // Expansions
                            var expansion = _locate.LocateExpansion(g, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            expansion.Modifiers.Add(newModifier);
                            break;
                        case "d": // Expeditions
                            var expedition = _locate.LocateExpedition(g, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            expedition.Modifiers.Add(newModifier);
                            break;
                        case "o": // Traits
                            var targettrait = _locate.LocateTrait(g, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            targettrait.Modifiers.Add(newModifier);
                            break;
                        case "e": // Speciality
                            var specialty = _locate.LocateSpecialty(g, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            specialty.Modifiers.Add(newModifier);
                            break;
                        case "c": // Contracts
                            var contract = _locate.LocateContract(g, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            contract.Modifiers.Add(newModifier);
                            break;
                        case "i": // Clicks
                            var click = _locate.LocateStageClick(g, trait.TargetId);

                            newModifier.ApplyerId = itemId;
                            newModifier.Type = trait.EffectType;
                            newModifier.Scope = trait.Persistence;
                            newModifier.Operation = trait.EffectOp;
                            newModifier.Value = trait.EffectValue;

                            click.Modifiers.Add(newModifier);
                            break;
                        case "r": // Resources
                            var resource = _locate.LocateResource(g, trait.TargetId);

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
