using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Xml.Linq;

namespace FurmaIdle.Services
{
    public interface IExpeditionService
    {
        List<CharacterModel> GetActiveCharacters(ExpeditionModel expedition);

        StageModel? GetCurrentStage();
        ExpeditionModel GetOrCreateCurrentExpedition();

        // novo: consulta e seleção
        IEnumerable<CharacterModel> GetCharactersInBase();   // só “na base”
        IReadOnlyCollection<string> GetPartyIds();           // ids selecionados
        bool IsExpeditionActive();                           // está ativa?
        bool CharSelected(string charId);
        int GetPartyCap();                                  // PartySizeMax ou fallback
        bool CanToggleChar(string charId);
        bool ToggleChar(string charId, out string? reason);  // seleciona/desseleciona respeitando cap

        Task LaunchExpedition(IReadOnlyCollection<string> roster);
    }

    public sealed class ExpeditionService : IExpeditionService
    {
        private readonly ILocateService _locate;
        private readonly ICurrentGameService _game;

        public ExpeditionService(ILocateService locate, ICurrentGameService game)
        {
            _locate = locate;
            _game = game;
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

        // ===== Helpers de Stage/Expedição =====
        public StageModel? GetCurrentStage()
        {
            var stId = _game.CurrentGame?.SelectedStageId;
            if (string.IsNullOrWhiteSpace(stId)) return null;

            if (_game.CurrentGame?.Stages != null &&
                _game.CurrentGame.Stages.TryGetValue(stId, out var st))
                return st;

            return null;
        }

        public ExpeditionModel GetOrCreateCurrentExpedition()
        {
            var st = GetCurrentStage() ?? throw new InvalidOperationException("Nenhum Stage selecionado.");
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
            var st = GetCurrentStage();
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
            var st = GetCurrentStage();
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

        public async Task LaunchExpedition(IReadOnlyCollection<string> roster)
        {
            await _game.Mutate(g =>
            {
                var st = GetCurrentStage() ?? throw new InvalidOperationException("Nenhum Stage selecionado.");
                var ex = GetOrCreateCurrentExpedition();

                if (ex.ExpeditionState == UnlockHelper.ExpeditionState.Active)
                    return; // já ativa — nada a fazer

                ex.PartyIds ??= new List<string>();

                // Normaliza roster: remove vazios, distinct e respeita o cap
                var cap = GetPartyCap();
                var ids = (roster ?? Array.Empty<string>())
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Distinct(StringComparer.Ordinal)
                    .Take(cap)
                    .ToList();

                // Mantém apenas personagens válidos e "na base"
                var finalIds = new List<string>(ids.Count);
                foreach (var id in ids)
                {
                    var c = _locate.LocateCharacter(_game.CurrentGame, id);
                    if (c is null) continue;
                    if (c.State != UnlockHelper.State.Unlocked) continue;
                    if (c.CharState != UnlockHelper.CharState.InBase) continue;

                    // Marca no personagem
                    c.CharState = UnlockHelper.CharState.OnStage;
                    c.InStageId = st.Id;

                    finalIds.Add(id);
                }

                // Atualiza expedição
                ex.PartyIds.Clear();
                ex.PartyIds.AddRange(finalIds);
                ex.StageId = st.Id;
                ex.ExpeditionState = UnlockHelper.ExpeditionState.Active;

                ex.StartedAt = DateTimeOffset.UtcNow;
                ex.FinishedAt = null;

            }, save: true); // <-- persiste ao final
        }
    }
}
