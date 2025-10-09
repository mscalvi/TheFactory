using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using static FurmaIdle.Helpers.ExpeditionEnum;
using static FurmaIdle.Models.CharacterModel;

namespace FurmaIdle.Services
{
    public interface IGameService
    {
        GameModel Current { get; }

        // Stage (foco de UI)
        string SelectedStageId { get; }
        bool SetSelectedStage(string stageId);
        StageModel? GetSelectedStage();

        // Resources
        ResourceModel? Get(string id);
        void Add(string id, double amount = 1);
        void Click(string stageId);

        // Roster (seleção pré-start)
        bool ToggleRoster(string charId, out string? reason);
        int GetRosterCount();
        IReadOnlyCollection<string> GetRoster();

        // Expedition (por stage)
        ExpeditionModel? GetExpedition(string stageId);
        bool HasAnyExpeditionActive { get; }
        int GetEffectivePartyCap(string stageId);
        double GetKnowledgeRate(string stageId);
        bool StartExpedition(string stageId, IReadOnlyCollection<string> roster, out string? reason);
        bool EndExpedition(string stageId, string? reason = null);
        PartyInfo GetPartyInfo(string stageId);

        // Gerais
        void Attach(GameModel model);
        event Action? Changed;
        event Action<string, LogKind>? Logged;
    }

    public sealed class GameService : IGameService
    {
        private readonly IUpgradeService _effects;
        private readonly IStageService _stages;
        public GameModel Current { get; private set; } = new();
        public event Action? Changed;
        public event Action<string, LogKind>? Logged;

        public GameService(IUpgradeService effects, IStageService stages)
        {
            _effects = effects ?? throw new ArgumentNullException(nameof(effects));
            _stages = stages ?? throw new ArgumentNullException(nameof(stages));
        }

        public void Attach(GameModel model)
        {
            Current = model ?? throw new ArgumentNullException(nameof(model));

            Current.Guild ??= new GuildModel();
            Current.Guild.Roster ??= new HashSet<string>();

            _selectedStageId = _stages.GetFirstUnlocked(Current);
            Changed?.Invoke();
        }

        // ===== Stage foco de UI =====
        private string _selectedStageId = "s00";
        public string SelectedStageId => _selectedStageId;

        public bool SetSelectedStage(string stageId)
        {
            if (string.IsNullOrWhiteSpace(stageId)) return false;
            if (!_stages.CanSelect(Current, stageId, out var reason))
            {
                Logged?.Invoke(reason ?? "Stage indisponível.", LogKind.Error);
                return false;
            }
            if (_selectedStageId == stageId) return false;

            _selectedStageId = stageId;
            Changed?.Invoke();
            return true;
        }

        public StageModel? GetSelectedStage() => _stages.Get(Current, _selectedStageId);

        // ===== Resources =====
        public ResourceModel? Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || Current.Resources is null) return null;
            return Current.Resources.TryGetValue(id, out var r) ? r : null;
        }

        private ResourceModel EnsureResource(string id)
        {
            if (Current.Resources is null) Current.Resources = new();
            if (!Current.Resources.TryGetValue(id, out var r))
            {
                r = new ResourceModel { Id = id, Actual = 0, Total = 0 };
                Current.Resources[id] = r;
            }
            return r;
        }

        public void Add(string id, double amount = 1)
        {
            if (string.IsNullOrWhiteSpace(id) || amount == 0) return;
            var r = EnsureResource(id);
            r.Actual += amount;
            if (amount > 0) r.Total += amount;
            Changed?.Invoke();
        }

        public void Click(string stageId)
        {
            if (string.IsNullOrWhiteSpace(stageId) || Current.Clicks is null) return;
            if (!Current.Clicks.TryGetValue(stageId, out var click)) return;

            var gain = click.BaseGain * click.Modifier;
            if (!(gain > 0) || double.IsNaN(gain) || double.IsInfinity(gain)) return;

            var resId = StageData.GetResourceId(stageId);
            Add(resId, gain);
            click.TotalGain += gain;
            Changed?.Invoke();
        }

        // ===== Roster (seleção pré-start) =====
        public bool ToggleRoster(string charId, out string? reason)
        {
            reason = null;
            if (string.IsNullOrWhiteSpace(charId)) { reason = "Id inválido."; return false; }
            if (!Current.Characters.TryGetValue(charId, out var c)) { reason = "Personagem inexistente."; return false; }

            // (opcional) travar mudança com expedições ativas
            if (HasAnyExpeditionActive) { reason = "Não é possível alterar a equipe com expedições ativas."; return false; }

            if (c.CharState != CharStateEnum.CharState.InBase)
            { reason = $"{c.Name} não está na Base."; return false; }

            var roster = Current.Guild.Roster;

            if (roster.Contains(charId))
            {
                roster.Remove(charId);
                Changed?.Invoke();
                return true;
            }
            else
            {
                if (roster.Count >= (Current.Guild?.PartyCapMax ?? 0))
                { reason = "Capacidade máxima da guilda atingida."; return false; }

                roster.Add(charId);
                Changed?.Invoke();
                return true;
            }
        }

        public int GetRosterCount() => Current.Guild?.Roster.Count ?? 0;
        public IReadOnlyCollection<string> GetRoster()
        {
            return (IReadOnlyCollection<string>?)Current?.Guild?.Roster ?? Array.Empty<string>();
        }

        // ===== Expedition =====
        public ExpeditionModel? GetExpedition(string stageId)
        {
            if (string.IsNullOrWhiteSpace(stageId)) return null;
            return Current.Stages.TryGetValue(stageId, out var st) ? st.Expedition : null;
        }

        public bool HasAnyExpeditionActive
            => Current.Stages.Values.Any(s => s.Expedition?.ExpeditionStatus == ExpeditionStatus.Active);

        public int GetEffectivePartyCap(string stageId)
        {
            var stageCap = _stages.GetEffectivePartyCap(Current, stageId); // cap "do stage"
            var guildCap = Current.Guild?.PartyCapMax ?? 0;
            return Math.Min(stageCap, guildCap);
        }
        public PartyInfo GetPartyInfo(string stageId)
        {
            var usedRoster = Current?.Guild?.Roster?.Count ?? 0;
            var capRoster = Current?.Guild?.PartyCapMax ?? 0;

            var ex = GetExpedition(stageId);
            var active = ex?.ExpeditionStatus == ExpeditionEnum.ExpeditionStatus.Active;
            var usedStage = active ? (ex?.PartyId?.Count ?? 0) : 0;

            var capStage = GetEffectivePartyCap(stageId);

            return new PartyInfo(
                UsedRoster: usedRoster,
                CapRoster: capRoster,
                UsedStage: usedStage,
                CapStage: capStage,
                ExpeditionActive: active
            );
        }

        public double GetKnowledgeRate(string stageId)
        {
            if (!Current.Stages.TryGetValue(stageId, out var st)) return 0;
            var partyCount = st.Expedition?.PartyId.Count ?? 0;
            var capEff = GetEffectivePartyCap(stageId);

            const double basePerSlot = 0.20; // /s por slot de cap
            const double perMember = 0.05; // /s por membro alocado
            double rate = capEff * basePerSlot + partyCount * perMember;

            // Multiplicadores (se existirem)
            double mult = 1.0; // _effects.ExpeditionKnowledgeMult(stageId);
            return rate * mult;
        }

        public bool StartExpedition(string stageId, IReadOnlyCollection<string> roster, out string? reason)
        {
            reason = null;
            if (string.IsNullOrWhiteSpace(stageId)) { reason = "Stage inválido."; return false; }
            if (!Current.Stages.TryGetValue(stageId, out var st)) { reason = "Stage inexistente."; return false; }
            if (!st.Unlocked) { reason = "Stage bloqueado."; return false; }

            // cria/garante o modelo de expedição
            var ex = st.Expedition ??= new ExpeditionModel { StageId = stageId };
            ex.PartyId ??= new List<string>();
            if (ex.ExpeditionStatus == ExpeditionStatus.Active) { reason = "Expedição já está ativa."; return false; }

            var cap = GetEffectivePartyCap(stageId);
            if (roster is null || roster.Count == 0) { reason = "Selecione pelo menos 1 membro."; return false; }
            if (roster.Count > cap) { reason = $"Seleção excede o limite ({cap})."; return false; }

            // Validação usando modelo robusto (sem NRE)
            var party = new List<CharacterModel>(roster.Count);
            foreach (var id in roster)
            {
                if (string.IsNullOrWhiteSpace(id)) { reason = "Id vazio."; return false; }
                if (!Current.Characters.TryGetValue(id, out var c)) { reason = $"Personagem inválido: {id}"; return false; }
                if (c.CharState != CharStateEnum.CharState.InBase) { reason = $"{c.Name} não está na Base."; return false; }
                if (IsCharacterEngagedInAnyExpeditionSafe(id)) { reason = $"{c.Name} já está em outra expedição."; return false; }
                party.Add(c);
            }

            // Commit
            ex.PartyId.Clear();
            foreach (var c in party)
            {
                c.CharState = CharStateEnum.CharState.OnStage;
                c.CharDestId = stageId;
                ex.PartyId.Add(c.Id);
            }
            ex.ExpeditionStatus = ExpeditionStatus.Active;
            ex.Start = DateTimeOffset.UtcNow;

            // limpar seleção global sem NRE
            Current.Guild?.Roster?.Clear();

            string stageName = LookupData.Stage(Current, _stages, stageId).Name;

            Logged?.Invoke($"Expedição iniciada em {stageName}, com {ex.PartyId.Count} membros. Sobraram {cap- ex.PartyId.Count} vagas. Boa aventura!", LogKind.Success);
            Changed?.Invoke();
            return true;
        }

        private bool IsCharacterEngagedInAnyExpeditionSafe(string charId)
        {
            foreach (var st in Current.Stages.Values)
            {
                var ex = st.Expedition;
                if (ex is null) continue;
                if (ex.ExpeditionStatus != ExpeditionStatus.Active) continue;

                var list = ex.PartyId;
                if (list is null || list.Count == 0) continue;

                if (list.Contains(charId)) return true;
            }
            return false;
        }

        public bool EndExpedition(string stageId, string? reason = null)
        {
            string stageName = LookupData.Stage(Current, _stages, stageId).Name;

            if (string.IsNullOrWhiteSpace(stageId)) return false;
            if (!Current.Stages.TryGetValue(stageId, out var st)) return false;

            var ex = st.Expedition;
            if (ex is null || ex.ExpeditionStatus == ExpeditionStatus.Idle) return false;

            var ids = ex.PartyId ??= new List<string>();
            foreach (var id in ids.ToList())
            {
                if (!Current.Characters.TryGetValue(id, out var c)) continue;
                c.CharState = CharStateEnum.CharState.InBase;
                c.CharDestId = null;
            }

            ids.Clear();
            ex.ExpeditionStatus = ExpeditionStatus.Idle;
            ex.Start = null;

            Logged?.Invoke($"Expedição encerrada em {stageName}" + (string.IsNullOrWhiteSpace(reason) ? "." : $": {reason}"), LogKind.Success);
            Changed?.Invoke();
            return true;
        }

    }
}
