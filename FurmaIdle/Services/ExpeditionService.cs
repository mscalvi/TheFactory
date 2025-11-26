using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Resources;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;
using static FurmaIdle.Helpers.UnlockHelper;

namespace FurmaIdle.Services
{
    public interface IExpeditionService
    {
        int GetPartyCap(StageModel stage);
        bool CanToggleChar(StageModel stage, string charId);
        bool ToggleChar(StageModel stage, string charId);

        List<CharacterModel> GetInExpCharacters(ExpeditionModel expedition);

        Task FirstExpeditionStart();
        Task LaunchExpedition(StageModel stage);
        Task EndExpedition(StageModel stage);

        Task EndExpansion(string expansionId);
    }

    public sealed class ExpeditionService : IExpeditionService
    {
        private readonly ILocateService _locate;
        private readonly ICurrentGameService _game;
        private readonly IEffectService _effect;
        private readonly IKnowledgeService _knowledge;
        private readonly IUiService _ui;
        private readonly ILoreService _lore;

        public ExpeditionService(ILocateService locate, ICurrentGameService game, IEffectService effect, IKnowledgeService knowledge, IUiService ui, ILoreService lore)
        {
            _locate = locate;
            _game = game;
            _effect = effect;
            _knowledge = knowledge;
            _ui = ui;
            _lore = lore;
        }

        public int GetPartyCap(StageModel stage)
        {
            int partySize = 0;

            partySize = stage.StartPartySize;

            foreach (var modifier in stage.Modifiers)
            {
                if (modifier.Type == EffectHelper.EffectType.PartyCapSize)
                {
                    if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                    {
                        partySize += (int)modifier.Value;
                    }
                }
            }

            return partySize;
        }

        public bool CanToggleChar(StageModel stage, string charId)
        {
            var expedition = stage.Expedition;
            var character = _locate.LocateCharacter(_game.CurrentGame, charId);

            if (expedition.ExpeditionState != ExpeditionState.Idle) return false;
            if (character.CharState == CharState.InLine) return true;

            int countLine = 0;
            foreach (var characters in _game.CurrentGame.Characters)
            {
                if (characters.Value.CharState == CharState.InLine) countLine++;
            }

            if(countLine < GetPartyCap(stage))
            {
                return true;
            } else
            {
                return false;
            }
        }

        public bool ToggleChar(StageModel stage, string charId)
        {
            var character = _locate.LocateCharacter(_game.CurrentGame, charId);

            if (character.CharState == CharState.InBase)
            {
                character.CharState = CharState.InLine;
                character.InStageId = stage.Id;
                return true;
            } 
            else if (character.CharState == CharState.InLine)
            {
                character.CharState = CharState.InBase;
                return true;
            }

            return false;
        }

        public List<CharacterModel> GetInExpCharacters(ExpeditionModel expedition)
        {
            var result = new List<CharacterModel>();
            if (expedition?.PartyIds == null) return result;

            foreach (var id in expedition.PartyIds)
            {
                if (string.IsNullOrWhiteSpace(id)) continue;
                var c = _locate.LocateCharacter(_game.CurrentGame, id);
                if (c != null) result.Add(c);
            }
            return result;
        }

        // Start e End
        public async Task FirstExpeditionStart()
        {
            var game = _game.CurrentGame;
            var stage = _locate.LocateStage(game, game.SelectedStageId);
            var expedition = stage?.Expedition;

            await _game.Mutate(game =>
            {
                if (expedition.ExpeditionState == UnlockHelper.ExpeditionState.Active)
                {
                    return;
                }
                else
                {
                    expedition = new ExpeditionModel();
                    stage.Expedition = expedition;
                }

                stage.ExpeditionStats = new StatsModel();

                expedition.PartyIds.Clear();

                foreach (var character in game.Characters)
                {
                    if (character.Value.State == State.Unlocked)
                    {
                        expedition.PartyIds.Add(character.Key);
                        character.Value.CharState = CharState.InStage;
                        character.Value.InStageId = stage.Id;
                    }
                }

                expedition.StageId = stage.Id;
                expedition.ExpeditionState = UnlockHelper.ExpeditionState.Active;
                expedition.StartedAt = DateTimeOffset.UtcNow;

                expedition.FinishedAt = null;

            }, save: true);

            foreach (var characterId in expedition.PartyIds)
            {

                var character = _locate.LocateCharacter(game, characterId);
                var traitId = character.TraitId;
                await _effect.ApplyEffect(ItemHelper.ItemType.Trait, traitId, stage.Id);
            }

            _ui.NavMenuControl("GameStart");
            _lore.LoreTrigger("GameStart");
        }

        public async Task LaunchExpedition(StageModel stage)
        {
            const int minBusyMs = 2000;
            var sw = Stopwatch.StartNew();

            _ui.SetBusy("Hora de partir! Com esse grupo, não tem como falharmos!");

            try
            {

                var expedition = stage?.Expedition;
                var game = _game.CurrentGame;

                await _game.Mutate(game =>
                {
                    if (expedition.ExpeditionState == UnlockHelper.ExpeditionState.Active)
                    {
                        return;
                    }
                    else
                    {
                        expedition = new ExpeditionModel();
                        stage.Expedition = expedition;
                    }

                    stage.ExpeditionStats = new StatsModel();

                    expedition.PartyIds.Clear();

                    foreach (var character in game.Characters)
                    {
                        if (character.Value.CharState == CharState.InLine)
                        {
                            expedition.PartyIds.Add(character.Key);
                            character.Value.CharState = CharState.InStage;
                            character.Value.InStageId = stage.Id;
                        }
                    }

                    expedition.StageId = stage.Id;
                    expedition.ExpeditionState = UnlockHelper.ExpeditionState.Active;
                    expedition.StartedAt = DateTimeOffset.UtcNow;

                    expedition.FinishedAt = null;

                    _ui.NavMenuControl("ExpeditionStart");
                    _lore.LoreTrigger("ExpeditionStart");

                }, save: true);

                foreach (var characterId in expedition.PartyIds)
                {
                    var character = _locate.LocateCharacter(game, characterId);
                    var traitId = character.TraitId;
                    await _effect.ApplyEffect(ItemHelper.ItemType.Trait, traitId, stage.Id);
                }
            }
            finally
            {
                sw.Stop();
                var elapsed = (int)sw.ElapsedMilliseconds;
                var remaining = minBusyMs - elapsed;

                if (remaining > 0)
                {
                    await Task.Delay(remaining);
                }

                _ui.ClearBusy();
            }
        }

        public async Task EndExpedition(StageModel stage)
        {
            const int minBusyMs = 2000;
            var sw = Stopwatch.StartNew();

            _ui.SetBusy("Encerrando a Expansão, organizando os relatórios e revendo o aprendizado...");

            try
            {
                var expansion = _locate.LocateExpansion(_game.CurrentGame, _game.CurrentGame.CurrentExpansionId);

                // transforma coins em Knowledge
                long cTotal = 0;
                foreach (var coins in stage.ExpeditionStats.CoinsGain)
                {
                    if (coins.Key == stage.CoinId)
                    {
                        cTotal += coins.Value;
                    }

                    if (expansion.ExpansionStats.Coins.TryGetValue(coins.Key, out var expanCoin))
                    {
                        expanCoin -= cTotal;
                        expansion.ExpansionStats.Coins[coins.Key] = expanCoin;
                    }                    
                }

                stage.ExpeditionStats.Coins.Clear();

                await _knowledge.EndExpeditionKnowGain(stage, cTotal);

                await _game.Mutate(game =>
                {
                    var expedition = stage?.Expedition;
                    if (stage is null || expedition is null) return;

                    var expansion = _locate.LocateExpansion(game, game.CurrentExpansionId);

                    // devolver personagens para a base
                    if (expedition.PartyIds is not null && expedition.PartyIds.Count > 0)
                    {
                        foreach (var cid in expedition.PartyIds)
                        {
                            if (game.Characters.TryGetValue(cid, out var ch) && ch is not null)
                            {
                                ch.CharState = UnlockHelper.CharState.InBase;
                                ch.InStageId = null;
                            }
                        }
                        expedition.PartyIds.Clear();
                    }

                    if (stage.ActiveContracts is not null && stage.ActiveContracts.Count > 0)
                    {
                        // limpar contratos/timers do stage
                        foreach (var contracts in stage.ActiveContracts)
                        {
                            var contract = _locate.LocateContract(game, contracts.Key);
                            expansion.inUseContracts.Remove(contracts.Key);
                            contract.GameUseState = UnlockHelper.ContractState.Avaliable;
                        }
                    }

                    stage.ActiveContracts?.Clear();
                    stage.lockedContractLevel.Clear();

                    // reseta upgrades
                    foreach (var upgrades in game.Upgrades)
                    {
                        if (upgrades.Value.Persistence == Persistence.untilExpedition)
                        {
                            if (upgrades.Value.State != State.Blocked)
                            {
                                upgrades.Value.State = State.Available;
                                upgrades.Value.ActualBuy = 0;
                            }
                        }
                    }

                    // finalizar expedição
                    expedition.FinishedAt = DateTimeOffset.UtcNow;
                    expedition.ExpeditionState = UnlockHelper.ExpeditionState.Idle;

                    _ui.NavMenuControl("ExpeditionEnd");
                    _lore.LoreTrigger("ExpeditionEnd", "aprendeu");

                }, save: true);

                await _game.Mutate(game =>
                {
                    // reseta modifiers

                    foreach (var characters in game.Characters)
                    {
                        ScrubExpeditionMods(characters.Value.Modifiers);
                    }
                    foreach (var click in game.Clicks)
                    {
                        ScrubExpeditionMods(click.Value.Modifiers);
                    }
                    foreach (var contracts in game.Contracts)
                    {
                        ScrubExpeditionMods(contracts.Value.Modifiers);
                    }
                    foreach (var knowledge in game.Knowledges)
                    {
                        ScrubExpeditionMods(knowledge.Value.Modifiers);
                    }
                    foreach (var local in game.Locals)
                    {
                        ScrubExpeditionMods(local.Value.Modifiers);
                    }
                    foreach (var resource in game.Resources)
                    {
                        ScrubExpeditionMods(resource.Value.Modifiers);
                    }
                    foreach (var specialty in game.Specialties)
                    {
                        ScrubExpeditionMods(specialty.Value.Modifiers);
                    }
                    foreach (var stage in game.Stages)
                    {
                        ScrubExpeditionMods(stage.Value.Modifiers);
                    }
                    foreach (var tech in game.Techs)
                    {
                        ScrubExpeditionMods(tech.Value.Modifiers);
                    }
                    foreach (var trait in game.Traits)
                    {
                        ScrubExpeditionMods(trait.Value.Modifiers);
                    }
                    foreach (var upgrade in game.Upgrades)
                    {
                        ScrubExpeditionMods(upgrade.Value.Modifiers);
                    }
                }, save: true);
            }
            finally
            {
                sw.Stop();
                var elapsed = (int)sw.ElapsedMilliseconds;
                var remaining = minBusyMs - elapsed;

                if (remaining > 0)
                {
                    await Task.Delay(remaining);
                }

                _ui.ClearBusy();
            }

        }

        public async Task EndExpansion(string expansionId)
        {
            const int minBusyMs = 3000;
            var sw = Stopwatch.StartNew();

            _ui.SetBusy("Convocando todos os membros da Guilda, hora de expandirmos nossos negócios...");

            try
            {
                foreach (var stage in _game.CurrentGame.Stages.Values)
                {
                    if (stage.State != State.Unlocked) continue;

                    // transforma coins em Knowledge
                    long cTotal = 0;

                    var stats = stage.ExpeditionStats;
                    if (stats?.CoinsGain != null)
                    {
                        foreach (var coins in stage.ExpeditionStats.CoinsGain)
                        {
                            if (coins.Key == stage.CoinId)
                            {
                                cTotal += coins.Value;
                            }
                        }

                        stats.Coins.Clear();
                    }

                    _game.CurrentGame.NoExpeditionStats = new StatsModel();

                    await _knowledge.EndExpeditionKnowGain(stage, cTotal);

                    await _game.Mutate(game =>
                    {
                        var expedition = stage?.Expedition;
                        if (stage is null || expedition is null) return;

                        var expansion = _locate.LocateExpansion(game, game.CurrentExpansionId);

                        expansion.ExpansionStats.Knowledge.Clear();
                        expansion.ExpansionStats.Resources.Clear();

                        // devolver personagens para a base
                        if (expedition.PartyIds is not null && expedition.PartyIds.Count > 0)
                        {
                            foreach (var cid in expedition.PartyIds)
                            {
                                if (game.Characters.TryGetValue(cid, out var ch) && ch is not null)
                                {
                                    ch.CharState = UnlockHelper.CharState.InBase;
                                    ch.InStageId = null;
                                }
                            }
                            expedition.PartyIds.Clear();
                        }

                        if (stage.ActiveContracts is not null && stage.ActiveContracts.Count > 0)
                        {
                            // limpar contratos/timers do stage
                            foreach (var contracts in stage.ActiveContracts)
                            {
                                var contract = _locate.LocateContract(game, contracts.Key);
                                expansion.inUseContracts.Remove(contracts.Key);
                                contract.GameUseState = UnlockHelper.ContractState.Avaliable;
                            }
                        }

                        stage.ActiveContracts?.Clear();
                        stage.lockedContractLevel.Clear();

                        // finalizar expedição
                        expedition.FinishedAt = DateTimeOffset.UtcNow;
                        expedition.ExpeditionState = UnlockHelper.ExpeditionState.Idle;

                        // reseta upgrades
                        foreach (var upgrades in game.Upgrades)
                        {
                            if (upgrades.Value.Persistence == Persistence.untilExpedition)
                            {
                                if (upgrades.Value.State != State.Blocked)
                                {
                                    upgrades.Value.State = State.Available;
                                    upgrades.Value.ActualBuy = 0;
                                }
                            }
                        }
                        foreach (var upgrades in game.Upgrades)
                        {
                            if (upgrades.Value.Persistence == Persistence.untilExpansion)
                            {
                                if (upgrades.Value.State != State.Blocked)
                                {
                                    upgrades.Value.State = State.Available;
                                    upgrades.Value.ActualBuy = 0;
                                }
                            }
                        }
                    }, save: true);
                }

                await _game.Mutate(game =>
                {
                    // reseta modifiers
                    foreach (var characters in game.Characters)
                    {
                        ScrubExpansionMods(characters.Value.Modifiers);
                    }
                    foreach (var click in game.Clicks)
                    {
                        ScrubExpansionMods(click.Value.Modifiers);
                    }
                    foreach (var contracts in game.Contracts)
                    {
                        ScrubExpansionMods(contracts.Value.Modifiers);
                    }
                    foreach (var knowledge in game.Knowledges)
                    {
                        ScrubExpansionMods(knowledge.Value.Modifiers);
                    }
                    foreach (var local in game.Locals)
                    {
                        ScrubExpansionMods(local.Value.Modifiers);
                    }
                    foreach (var resource in game.Resources)
                    {
                        ScrubExpansionMods(resource.Value.Modifiers);
                    }
                    foreach (var specialty in game.Specialties)
                    {
                        ScrubExpansionMods(specialty.Value.Modifiers);
                    }
                    foreach (var stage in game.Stages)
                    {
                        ScrubExpansionMods(stage.Value.Modifiers);
                    }
                    foreach (var tech in game.Techs)
                    {
                        ScrubExpansionMods(tech.Value.Modifiers);
                    }
                    foreach (var trait in game.Traits)
                    {
                        ScrubExpansionMods(trait.Value.Modifiers);
                    }
                    foreach (var upgrade in game.Upgrades)
                    {
                        ScrubExpansionMods(upgrade.Value.Modifiers);
                    }

                }, save: true);

                await _game.Mutate(game =>
                {
                    var expansion = _locate.LocateExpansion(game, expansionId);

                    expansion.FinishedAt = DateTimeOffset.UtcNow;

                    game.CurrentExpansionId = expansion.NextExpansion;

                    _ui.NavMenuControl("ExpansionEnd");
                    _lore.LoreTrigger("ExpansionEnd");

                }, save: true);
            }            
            finally
            {
                sw.Stop();
                var elapsed = (int)sw.ElapsedMilliseconds;
                var remaining = minBusyMs - elapsed;

                if (remaining > 0)
                {
                    await Task.Delay(remaining);
                }

                _ui.ClearBusy();
            }
        }

        private static void ScrubExpeditionMods(List<ModifierModel> list)
        {
            list.RemoveAll(m =>
                    m.Scope == Persistence.untilExpedition || m.Scope == Persistence.untilTimer
                );
        }
        private static void ScrubExpansionMods(List<ModifierModel> list)
        {
            list.RemoveAll(m =>
                    m.Scope == Persistence.untilExpansion || m.Scope == Persistence.untilExpedition || m.Scope == Persistence.untilTimer
                );
        }
    }
}
