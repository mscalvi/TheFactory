using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Collections.Generic;
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

        Task LaunchExpedition(StageModel stage);
        Task EndExpedition(StageModel stage);
    }

    public sealed class ExpeditionService : IExpeditionService
    {
        private readonly ILocateService _locate;
        private readonly ICurrentGameService _game;
        private readonly IEffectService _effect;
        private readonly IKnowledgeService _knowledge;

        public ExpeditionService(ILocateService locate, ICurrentGameService game, IEffectService effect, IKnowledgeService knowledge)
        {
            _locate = locate;
            _game = game;
            _effect = effect;
            _knowledge = knowledge;
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
        public async Task LaunchExpedition(StageModel stage)
        {
            var expedition = stage?.Expedition;
            var game = _game.CurrentGame;

            await _game.Mutate(game =>
            {
                if (expedition.ExpeditionState == UnlockHelper.ExpeditionState.Active)
                {
                    return;
                } else
                {
                    expedition = new ExpeditionModel();
                    stage.Expedition = expedition;
                }

                stage.ExpeditionStats = new StatsModel();

                expedition.PartyIds.Clear();

                foreach(var character in game.Characters)
                {
                    if(character.Value.CharState == CharState.InLine)
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
                _effect.ApplyEffect(ItemHelper.ItemType.Trait, traitId, stage.Id);
            }
        }
        public async Task EndExpedition(StageModel stage)
        {
            // transforma coins em Knowledge
            long cTotal = 0;
            foreach (var coins in stage.ExpeditionStats.CoinsGain)
            {
                if (coins.Key == stage.CoinId)
                {
                    cTotal += coins.Value;
                }
            }

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

                // limpar contratos/timers do stage
                foreach (var contracts in stage.ActiveContracts)
                {
                    var contract = _locate.LocateContract(game, contracts.Key);
                    expansion.inUseContracts.Remove(contracts.Key);
                    contract.UseState = UnlockHelper.ContractState.Avaliable;
                }
                stage.ActiveContracts?.Clear();
                stage.lockedContractLevel.Clear();

                // reseta upgrades
                foreach (var upgrades in game.Upgrades)
                {
                    if (upgrades.Value.Persistence == Persistence.untilExpedition)
                    {
                        upgrades.Value.State = State.Available;
                        upgrades.Value.ActualBuy = 0;
                    }
                }

                // reseta modifiers
                foreach (var contracts in game.Contracts)
                {
                    ScrubExpeditionMods(contracts.Value.Modifiers);
                }
                foreach (var resources in game.Resources)
                {
                    ScrubExpeditionMods(resources.Value.Modifiers);
                }
                foreach (var characters in game.Characters)
                {
                    ScrubExpeditionMods(characters.Value.Modifiers);
                }
                foreach (var knowledges in game.Knowledges)
                {
                    ScrubExpeditionMods(knowledges.Value.Modifiers);
                }

                // finalizar expedição
                expedition.FinishedAt = DateTimeOffset.UtcNow;
                expedition.ExpeditionState = UnlockHelper.ExpeditionState.Idle;

            }, save: true);
        }
        private static void ScrubExpeditionMods(List<ModifierModel> list)
        {
            list.RemoveAll(m =>
                    m.Scope == UnlockHelper.Persistence.untilExpedition
                );            
        }
    }
}
