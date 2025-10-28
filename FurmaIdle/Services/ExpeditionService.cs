using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using static FurmaIdle.Helpers.UnlockHelper;

namespace FurmaIdle.Services
{
    public interface IExpeditionService
    {
        List<CharacterModel> GetActiveCharacters(ExpeditionModel expedition);
        ExpeditionModel GetOrCreateCurrentExpedition(StageModel stage);

        IEnumerable<CharacterModel> GetCharactersInBase();
        IReadOnlyCollection<string> GetPartyIds(StageModel stage);
        bool IsExpeditionActive(ExpeditionModel expedition);
        bool CharSelected(StageModel stage, string charId);
        int GetPartyCap(StageModel stage);
        bool CanToggleChar(StageModel stage, string charId);
        bool ToggleChar(StageModel stage, string charId, out string? reason);

        Task LaunchExpedition(StageModel stage, IReadOnlyCollection<string> roster);
        Task EndExpedition(StageModel stage);
    }

    public sealed class ExpeditionService : IExpeditionService
    {
        private readonly ILocateService _locate;
        private readonly ICurrentGameService _game;
        private readonly IEffectService _effect;
        private readonly IUnlockService _unlock;
        private readonly IUiService _ui;

        public ExpeditionService(ILocateService locate, ICurrentGameService game, IEffectService effect, IUnlockService unlock, IUiService ui)
        {
            _locate = locate;
            _game = game;
            _effect = effect;
            _unlock = unlock;
            _ui = ui;
        }

        public List<CharacterModel> GetActiveCharacters(ExpeditionModel expedition)
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

        public ExpeditionModel GetOrCreateCurrentExpedition(StageModel stage)
        {
            return stage.Expedition ??= new ExpeditionModel
            {
                StageId = stage.Id,
                ExpeditionState = UnlockHelper.ExpeditionState.Idle,
                PartyIds = new List<string>()
            };
        }

        public bool IsExpeditionActive(ExpeditionModel expedition)
        {
            return expedition is not null && expedition.ExpeditionState == UnlockHelper.ExpeditionState.Active;
        }

        // ===== Consulta de personagens =====
        public IEnumerable<CharacterModel> GetCharactersInBase()
        {
            var g = _game.CurrentGame;
            if (g?.Characters is null) yield break;

            foreach (var c in g.Characters.Values)
            {
                if (c is null) continue;
                if (c.State != UnlockHelper.State.Unlocked) continue;
                if (c.CharState == UnlockHelper.CharState.InBase) yield return c;
            }
        }

        // ===== Seleção (Party) =====
        public IReadOnlyCollection<string> GetPartyIds(StageModel stage)
        {
            var ex = GetOrCreateCurrentExpedition(stage);
            return ex.PartyIds ??= new List<string>();
        }

        public bool CharSelected(StageModel stage, string charId)
        {
            var ids = GetPartyIds(stage);
            return ids.Contains(charId);
        }

        public int GetPartyCap(StageModel stage)
        {
            int partySize = 0;

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
            if (IsExpeditionActive(stage.Expedition)) return false;
            var ex = stage?.Expedition;

            if (ex.PartyIds!.Contains(charId)) return true;

            return ex.PartyIds!.Count < GetPartyCap(stage);
        }

        public bool ToggleChar(StageModel stage, string charId, out string? reason)
        {
            reason = null;

            if (string.IsNullOrWhiteSpace(charId)) { reason = "Id inválido."; return false; }
            if (IsExpeditionActive(stage.Expedition)) { reason = "Expedição já está ativa."; return false; }

            var ex = GetOrCreateCurrentExpedition(stage);
            ex.PartyIds ??= new List<string>();

            if (ex.PartyIds.Remove(charId))
                return true;

            if (ex.PartyIds.Count >= GetPartyCap(stage))
            {
                reason = $"Limite de equipe atingido ({GetPartyCap(stage)}).";
                return false;
            }

            ex.PartyIds.Add(charId);
            return true;
        }

        // Start e End
        public async Task LaunchExpedition(StageModel stage, IReadOnlyCollection<string> roster)
        {
            await _game.Mutate(game =>
            {
                var ex = stage?.Expedition;

                if (ex.ExpeditionState == UnlockHelper.ExpeditionState.Active)
                    return;

                ex.PartyIds ??= new List<string>();

                var cap = GetPartyCap(stage);
                var ids = (roster ?? Array.Empty<string>())
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Distinct(StringComparer.Ordinal)
                    .Take(cap)
                    .ToList();

                var finalIds = new List<string>(ids.Count);
                foreach (var id in ids)
                {
                    var c = _locate.LocateCharacter(_game.CurrentGame, id);
                    if (c is null) continue;
                    if (c.State != UnlockHelper.State.Unlocked) continue;
                    if (c.CharState != UnlockHelper.CharState.InBase) continue;

                    c.CharState = UnlockHelper.CharState.InStage;
                    c.InStageId = stage.Id;

                    finalIds.Add(id);
                }

                ex.PartyIds.Clear();
                ex.PartyIds.AddRange(finalIds);

                ex.StageId = stage.Id;
                ex.ExpeditionState = UnlockHelper.ExpeditionState.Active;
                ex.StartedAt = DateTimeOffset.UtcNow;

                ex.FinishedAt = null;

                // Lançar Traits

                _ui.UnlockMenu("i4");

            }, save: true);
        }

        public async Task EndExpedition(StageModel stage)
        {
            await _game.Mutate(game =>
            {
                _ui.LockMenu("i4");

                var ex = stage?.Expedition;
                if (stage is null || ex is null) return;

                // finalizar expedição
                ex.FinishedAt = DateTimeOffset.UtcNow;
                ex.ExpeditionState = UnlockHelper.ExpeditionState.Idle;

                // devolver personagens para a base
                if (ex.PartyIds is not null && ex.PartyIds.Count > 0)
                {
                    foreach (var cid in ex.PartyIds)
                    {
                        if (game.Characters.TryGetValue(cid, out var ch) && ch is not null)
                        {
                            ch.CharState = UnlockHelper.CharState.InBase;
                            ch.InStageId = null;
                        }
                    }
                    ex.PartyIds.Clear();
                }

                // limpar contratos/timers do stage
                foreach (var contracts in stage.ActiveContracts)
                {
                    var contract = _locate.LocateContract(game, contracts.Key);
                    contract.UseState = UnlockHelper.ContractState.Avaliable;
                }
                stage.ActiveContracts?.Clear();
                stage.lockedContracts?.Clear();

                // transforma coins em Knowledge
                // Income?

                // reseta upgrades
                foreach (var upgrades in game.Upgrades)
                {
                    if (upgrades.Value.Persistence == Persistence.untilExpedition)
                    {
                        upgrades.Value.State = State.Available;
                        upgrades.Value.ActualBuy = 0;
                    }
                }

            }, save: true);
        }
    }
}
