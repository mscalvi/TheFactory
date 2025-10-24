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

        ExpeditionModel GetOrCreateCurrentExpedition();

        // novo: consulta e seleção
        IEnumerable<CharacterModel> GetCharactersInBase();
        IReadOnlyCollection<string> GetPartyIds();
        bool IsExpeditionActive();
        bool CharSelected(string charId);
        int GetPartyCap();
        bool CanToggleChar(string charId);
        bool ToggleChar(string charId, out string? reason);

        Task LaunchExpedition(IReadOnlyCollection<string> roster);
        Task EndExpedition(GameModel g, string stageId);
    }

    public sealed class ExpeditionService : IExpeditionService
    {
        private readonly ILocateService _locate;
        private readonly ICurrentGameService _game;
        private readonly IEffectService _effect;
        private readonly IUnlockService _unlock;

        public ExpeditionService(ILocateService locate, ICurrentGameService game, IEffectService effect, IUnlockService unlock)
        {
            _locate = locate;
            _game = game;
            _effect = effect;
            _unlock = unlock;
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

        public ExpeditionModel GetOrCreateCurrentExpedition()
        {
            var st = _locate.LocateStage(_game.CurrentGame, _game.CurrentGame.SelectedStageId);
            // garante não-nulo
            return st.ActiveExpedition ??= new ExpeditionModel
            {
                StageId = st.Id,
                ExpeditionState = UnlockHelper.ExpeditionState.Idle,
                PartyIds = new List<string>()
            };
        }

        public bool IsExpeditionActive()
        {
            var st = _locate.LocateStage(_game.CurrentGame, _game.CurrentGame.SelectedStageId);
            var ex = st?.ActiveExpedition;
            return ex is not null && ex.ExpeditionState == UnlockHelper.ExpeditionState.Active;
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
        public IReadOnlyCollection<string> GetPartyIds()
        {
            var ex = GetOrCreateCurrentExpedition();
            return ex.PartyIds ??= new List<string>();
        }
        public bool CharSelected(string charId)
        {
            var ids = GetPartyIds();
            return ids.Contains(charId);
        }
        public int GetPartyCap()
        {
            var st = _locate.LocateStage(_game.CurrentGame, _game.CurrentGame.SelectedStageId);
            return (st?.PartySizeActual > 0) ? st!.PartySizeActual : 3;
        }
        public bool CanToggleChar(string charId)
        {
            if (IsExpeditionActive()) return false;
            var ex = GetOrCreateCurrentExpedition();

            // Remover é sempre permitido
            if (ex.PartyIds!.Contains(charId)) return true;

            // Adicionar respeitando cap
            return ex.PartyIds!.Count < GetPartyCap();
        }
        public bool ToggleChar(string charId, out string? reason)
        {
            reason = null;

            if (string.IsNullOrWhiteSpace(charId)) { reason = "Id inválido."; return false; }
            if (IsExpeditionActive()) { reason = "Expedição já está ativa."; return false; }

            var ex = GetOrCreateCurrentExpedition();
            ex.PartyIds ??= new List<string>();

            // Se já está, desseleciona
            if (ex.PartyIds.Remove(charId))
                return true;

            // Se não está, respeita o cap
            if (ex.PartyIds.Count >= GetPartyCap())
            {
                reason = $"Limite de equipe atingido ({GetPartyCap()}).";
                return false;
            }

            ex.PartyIds.Add(charId);
            return true;
        }

        // Start e End
        public async Task LaunchExpedition(IReadOnlyCollection<string> roster)
        {
            await _game.Mutate(g =>
            {
                var st = _locate.LocateStage(_game.CurrentGame, _game.CurrentGame.SelectedStageId);
                var ex = GetOrCreateCurrentExpedition();

                if (ex.ExpeditionState == UnlockHelper.ExpeditionState.Active)
                    return;

                ex.PartyIds ??= new List<string>();

                var cap = GetPartyCap();
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
                    c.InStageId = st.Id;

                    finalIds.Add(id);
                }

                ex.PartyIds.Clear();
                ex.PartyIds.AddRange(finalIds);

                ex.StageId = st.Id;
                ex.ExpeditionState = UnlockHelper.ExpeditionState.Active;
                ex.StartedAt = DateTimeOffset.UtcNow;

                _effect.OnExpeditionStarted(g, ex);
                ex.FinishedAt = null;

            }, save: true);
        }

        public async Task EndExpedition(GameModel g, string stageId)
        {
            List<HashSet<string>> saves = new();

            await _game.Mutate(g => 
            {
                SaveState(g, stageId, saves);
            }, save:  false);

            await _game.Mutate(g =>
            {
                var st = _locate.LocateStage(g, stageId);
                var ex = st?.ActiveExpedition;
                if (st is null || ex is null) return;

                // finalizar expedição
                ex.FinishedAt = DateTimeOffset.UtcNow;
                ex.ExpeditionState = UnlockHelper.ExpeditionState.Idle;

                // devolver personagens para a base
                if (ex.PartyIds is not null && ex.PartyIds.Count > 0)
                {
                    foreach (var cid in ex.PartyIds)
                    {
                        if (g.Characters.TryGetValue(cid, out var ch) && ch is not null)
                        {
                            ch.CharState = UnlockHelper.CharState.InBase;
                            ch.InStageId = null;
                        }
                    }
                    ex.PartyIds.Clear();
                }

                // limpar contratos/timers do stage
                st.ActiveContracts?.Clear();
                st.lockedContracts?.Clear();

                // zerar a coin do stage nesta expedição
                var coinId = st.CoinId;
                if (!string.IsNullOrWhiteSpace(coinId))
                {
                    AddOrSet(g.ExpeditionStats.Coins, coinId, setTo: 0);
                    AddOrSet(g.ExpeditionStats.CoinsGain, coinId, setTo: 0);
                    AddOrSetFrac(g.ExpeditionStats.CoinsFrac, coinId, setTo: 0.0);
                }
            }, save: true);

            foreach (var hash in saves)
            {
                foreach (var id in hash)
                {
                    await _unlock.UnlockItem(id);
                    _effect.ReApplyEffect(id);
                }
            }
        }

        public List<HashSet<string>> SaveState(GameModel g, string stageId, List<HashSet<string>> saves)
        {
            // ---------- Fase 1: snapshot do que persiste ----------
            HashSet<string> keepCharacters = new(StringComparer.Ordinal);
            foreach (var u in g.Characters.Values)
            {
                if (u.State != UnlockHelper.State.Unlocked) continue;
                if (u.Persistence != Persistence.untilExpedition)
                    keepCharacters.Add(u.Id);
            }
            saves.Add(keepCharacters);

            HashSet<string> keepCoins = new(StringComparer.Ordinal);
            foreach (var u in g.Coins.Values)
            {
                if (u.State != UnlockHelper.State.Unlocked) continue;
                if (u.Persistence != Persistence.untilExpedition)
                    keepCoins.Add(u.Id);
            }
            saves.Add(keepCoins);

            HashSet<string> keepContracts = new(StringComparer.Ordinal);
            foreach (var u in g.Contracts.Values)
            {
                if (u.State != UnlockHelper.State.Unlocked) continue;
                if (u.Persistence != Persistence.untilExpedition)
                    keepContracts.Add(u.Id);
            }
            saves.Add(keepContracts);

            HashSet<string> keepExpansions = new(StringComparer.Ordinal);
            foreach (var u in g.Expansions.Values)
            {
                if (u.State != UnlockHelper.State.Unlocked) continue;
                if (u.Persistence != Persistence.untilExpedition)
                    keepExpansions.Add(u.Id);
            }
            saves.Add(keepExpansions);

            HashSet<string> keepKnowledges = new(StringComparer.Ordinal);
            foreach (var u in g.Knowledges.Values)
            {
                if (u.State != UnlockHelper.State.Unlocked) continue;
                if (u.Persistence != Persistence.untilExpedition)
                    keepKnowledges.Add(u.Id);
            }
            saves.Add(keepKnowledges);

            HashSet<string> keepLocals = new(StringComparer.Ordinal);
            foreach (var u in g.Locals.Values)
            {
                if (u.State != UnlockHelper.State.Unlocked) continue;
                if (u.Persistence != Persistence.untilExpedition)
                    keepLocals.Add(u.Id);
            }
            saves.Add(keepLocals);

            HashSet<string> keepResources = new(StringComparer.Ordinal);
            foreach (var u in g.Resources.Values)
            {
                if (u.State != UnlockHelper.State.Unlocked) continue;
                if (u.Persistence != Persistence.untilExpedition)
                    keepResources.Add(u.Id);
            }
            saves.Add(keepResources);

            HashSet<string> keepStages = new(StringComparer.Ordinal);
            foreach (var u in g.Stages.Values)
            {
                if (u.State != UnlockHelper.State.Unlocked) continue;
                if (u.Persistence != Persistence.untilExpedition)
                    keepStages.Add(u.Id);
            }
            saves.Add(keepStages);

            HashSet<string> keepTechs = new(StringComparer.Ordinal);
            foreach (var u in g.Techs.Values)
            {
                if (u.State != UnlockHelper.State.Unlocked) continue;
                if (u.Persistence != Persistence.untilExpedition)
                    keepTechs.Add(u.Id);
            }
            saves.Add(keepTechs);

            HashSet<string> keepUpgrades = new(StringComparer.Ordinal);
            foreach (var u in g.Upgrades.Values)
            {
                if (u.State != UnlockHelper.State.Unlocked) continue;
                if (u.Persistence != Persistence.untilExpedition)
                    keepUpgrades.Add(u.Id);
            }
            saves.Add(keepUpgrades);

            return saves;
        }
        
        private static void AddOrSet(Dictionary<string, long> dict, string key, long setTo)
            => dict[key] = setTo;

        private static void AddOrSetFrac(Dictionary<string, double> dict, string key, double setTo)
            => dict[key] = setTo;
    }
}
