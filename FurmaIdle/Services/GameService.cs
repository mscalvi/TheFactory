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

        // Tick
        void Tick(double dtSeconds);

        // Contratos
        bool StartContract(string stageId, string contractId, out string? reason);
        bool StopContract(string stageId, string contractId);
        bool BuyOrActivateContract(string stageId, string contractId, out string? reason);
        int GetContractsCap(string stageId);

        // Melhorias
        bool BuyUpgrade(string upgradeId, out string? reason);

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

            UpgradeData.EnsureUpgradesCatalog(Current); 
            _effects.Recompute(Current);

            _selectedStageId = _stages.GetFirstUnlocked(Current);
            Changed?.Invoke();
        }

        #region Stage foco de UI
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
        #endregion

        #region Resources
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

        private bool TrySpend(string resourceId, double amount)
        {
            if (string.IsNullOrWhiteSpace(resourceId) || !(amount > 0)) return false;
            var r = Get(resourceId);
            if (r is null || r.Actual < amount) return false;
            r.Actual -= amount;
            Changed?.Invoke();
            return true;
        }
        #endregion

        #region Clicks
        public void Click(string stageId)
        {
            if (!Current.Clicks.TryGetValue(stageId, out var click)) return;
            var mult = _effects.ClicksGainMult();
            var gain = click.BaseGain * click.Modifier * mult;
            Add(StageData.GetResourceId(stageId), gain);
            click.TotalGain += gain;
            Changed?.Invoke();
        }
        #endregion

        #region Roster
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
        #endregion

        #region Expedition
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

            // Contratos
            ex.Contracts = ContractData.CreateInitialContracts(); 
            ex.ActiveContracts = new List<ContractRun>();
            ex.LockedContractByLevel = new Dictionary<int, string>();

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

            if (st.Expedition is { } ex2)
            {
                foreach (var kv in ex2.Contracts) kv.Value.Quant = 0;
                ex2.ActiveContracts.Clear();
                ex2.LockedContractByLevel.Clear();
            }

            ids.Clear();
            ex.ExpeditionStatus = ExpeditionStatus.Idle;
            ex.Start = null;

            Logged?.Invoke($"Expedição encerrada em {stageName}" + (string.IsNullOrWhiteSpace(reason) ? "." : $": {reason}"), LogKind.Success);
            Changed?.Invoke();
            return true;
        }
        #endregion

        #region Ticks
        public void Tick(double dtSeconds)
        {
            // saneamento do delta
            if (!(dtSeconds > 0) || double.IsNaN(dtSeconds) || double.IsInfinity(dtSeconds)) return;

            // (opcional) limitar "catch-up" para não estourar ciclos após longas pausas
            const double MaxStep = 0.25; // 250 ms por subpasso para estabilizar barras de progresso
            int steps = (int)Math.Ceiling(dtSeconds / MaxStep);
            double step = dtSeconds / steps;

            bool anyChange = false;

            for (int s = 0; s < steps; s++)
            {
                foreach (var st in Current.Stages.Values)
                {
                    var ex = st.Expedition;
                    if (ex?.ExpeditionStatus != ExpeditionEnum.ExpeditionStatus.Active) continue;
                    if (ex.ActiveContracts is null || ex.Contracts is null) continue;

                    foreach (var run in ex.ActiveContracts)
                    {
                        if (!ex.Contracts.TryGetValue(run.ContractId, out var c)) continue;
                        if (c.Quant <= 0) continue;

                        // Tabela base por nível
                        if (!ContractsPricingHelper.TryGetBalance(c, out var bal)) continue;

                        // ---- APLICA MELHORIAS ----
                        // ganho multiplicativo
                        var gainMult = _effects.ContractGainMult(c.Id);   // ex.: x1.10, x1.15…
                                                                          // tempo multiplicativo (0.9 = 10% mais rápido)
                        var timeMult = _effects.ContractTimeMult(c.Id);

                        // parâmetros efetivos do contrato
                        double coinsPerCycle = bal.CoinsPerCycle * gainMult;
                        double secondsPerCycle = Math.Max(0.02, bal.SecondsPerCycle * timeMult); // clamp mínimo

                        // avança progresso
                        run.ProgressSec += step;

                        if (run.ProgressSec >= secondsPerCycle)
                        {
                            double cycles = Math.Floor(run.ProgressSec / secondsPerCycle);
                            if (cycles >= 1.0)
                            {
                                double amount = cycles * coinsPerCycle * c.Quant; // Quant pilha linear
                                if (amount > 0)
                                {
                                    Add(bal.ResourceId, amount);
                                    anyChange = true;
                                }
                                run.ProgressSec -= cycles * secondsPerCycle;
                            }
                        }
                    }
                }

                // ---- GERAÇÃO PASSIVA POR RECURSO (opcional, se você usa) ----
                if (Current.Resources is not null)
                {
                    foreach (var r in Current.Resources.Values)
                    {
                        double addPerSec = _effects.ResourceGenAddPerSecond(r.Id); // soma “all” + específico
                        if (addPerSec > 0)
                        {
                            Add(r.Id, addPerSec * step);
                            anyChange = true;
                        }
                    }
                }
            }

            Current.LastTickUtc = DateTimeOffset.UtcNow;

            // Evita re-render em todo tick se nada mudou
            if (anyChange) Changed?.Invoke();
        }
        #endregion

        #region Contratos
        public bool StartContract(string stageId, string contractId, out string? reason)
        {
            reason = null;
            if (string.IsNullOrWhiteSpace(stageId) || string.IsNullOrWhiteSpace(contractId))
            { reason = "Parâmetros inválidos."; return false; }

            if (!Current.Stages.TryGetValue(stageId, out var st) || st.Expedition is null)
            { reason = "Stage/expedição indisponível."; return false; }

            var ex = st.Expedition;
            if (ex.ExpeditionStatus != ExpeditionEnum.ExpeditionStatus.Active)
            { reason = "Expedição não está ativa."; return false; }

            // já existe?
            if (ex.ActiveContracts.Any(r => r.ContractId == contractId))
            { reason = "Contrato já está em execução."; return false; }

            // valida o contrato e pega o nível
            if (!ContractData.All.TryGetValue(contractId, out var def))
            { reason = "Contrato inválido."; return false; }

            // slots: usa a regra que você já tem na UI (stage.ContractsSlots)
            var slots = st.ContractsSlots > 0 ? st.ContractsSlots : 3;
            if (ex.ActiveContracts.Count >= slots)
            { reason = "Sem slots de contrato disponíveis."; return false; }

            // pega o balanço pelo nível
            if (!ContractBalanceData.ByLevel.TryGetValue(def.Level, out var bal))
            { reason = $"Sem tabela de balanço para nível {def.Level}."; return false; }

            ex.ActiveContracts.Add(new ContractRun
            {
                ContractId = def.Id,
                ProgressSec = 0
            });

            Logged?.Invoke($"Contrato {def.Name} iniciado (nível {def.Level}).", LogKind.Success);
            Changed?.Invoke();
            return true;
        }

        public bool StopContract(string stageId, string contractId)
        {
            if (!Current.Stages.TryGetValue(stageId, out var st) || st.Expedition is null) return false;
            var ex = st.Expedition;
            var removed = ex.ActiveContracts.RemoveAll(r => r.ContractId == contractId) > 0;
            if (removed)
            {
                Logged?.Invoke($"Contrato {contractId} encerrado.", LogKind.Info);
                Changed?.Invoke();
            }
            return removed;
        }

        public bool BuyOrActivateContract(string stageId, string contractId, out string? reason)
        {
            reason = null;

            try
            {
                if (!Current.Stages.TryGetValue(stageId, out var st) || st.Expedition is null)
                { reason = "Stage/expedição indisponível."; return false; }

                var expd = st.Expedition;
                if (expd.ExpeditionStatus != ExpeditionEnum.ExpeditionStatus.Active)
                { reason = "Expedição não está ativa."; return false; }

                // ======= GARANTIAS (evita NRE em saves antigos) =======
                expd.Contracts ??= new Dictionary<string, ContractModel>();
                expd.ActiveContracts ??= new List<ContractRun>();
                expd.LockedContractByLevel ??= new Dictionary<int, string>();

                // ======= Resolve contrato runtime =======
                if (!expd.Contracts.TryGetValue(contractId, out var c))
                {
                    if (!ContractData.All.TryGetValue(contractId, out var def))
                    { reason = "Contrato inválido."; return false; }

                    c = new ContractModel
                    {
                        Id = def.Id,
                        Name = def.Name,
                        Level = def.Level,
                        Image = def.Image,
                        FirstKnowId = def.FirstKnowId,
                        SecondKnowId = def.SecondKnowId,
                        ThirdKnowId = def.ThirdKnowId,
                        FirstDiferential = def.FirstDiferential,
                        SecondDiferential = def.SecondDiferential,
                        Unlocked = def.Unlocked,
                        Avaliable = def.Avaliable,
                        ConDestId = def.ConDestId,
                        Quant = 0
                    };
                    expd.Contracts[contractId] = c;
                }

                // ======= 1 contrato por NÍVEL =======
                if (expd.LockedContractByLevel.TryGetValue(c.Level, out var chosenId) && chosenId != c.Id)
                { reason = "Já existe um contrato ativo para este nível nesta expedição."; return false; }

                // ======= CAP: soma de TODAS as Quant =======
                var cap = GetContractsCap(stageId);  // soma dos MaxContracts da party
                var usedUnits = expd.Contracts.Values.Sum(k => k.Quant);

                // vamos comprar +1: precisa caber
                if (usedUnits + 1 > cap)
                {
                    Logged?.Invoke($"[DBG] cap={cap}, used={usedUnits}, buying={contractId}", LogKind.Info);

                    reason = $"Limite de contratos atingido ({usedUnits}/{cap}).";
                    return false;
                }

                // ======= Preço/Produção por Level =======
                var price = ContractsPricingHelper.NextPrice(c);
                var (resId, cps, spc) = ContractsPricingHelper.ProdParams(c);
                if (string.IsNullOrWhiteSpace(resId) || !(cps > 0) || !(spc > 0))
                { reason = "Tabela de balanço ausente."; return false; }

                if (!TrySpend(resId, price))
                { reason = $"Custa {price:N0} {resId}, saldo insuficiente."; return false; }

                // ======= Compra/ativação =======
                c.Quant += 1;

                // trava o nível na primeira compra deste nível
                if (!expd.LockedContractByLevel.ContainsKey(c.Level))
                    expd.LockedContractByLevel[c.Level] = c.Id;

                if (!expd.ActiveContracts.Any(r => r.ContractId == c.Id))
                    expd.ActiveContracts.Add(new ContractRun { ContractId = c.Id });

                var pps = ContractsPricingHelper.ProdPerSecond(c);
                Logged?.Invoke($"Contrato {c.Name} (nível {c.Level}) agora Quant={c.Quant} · {pps:N2}/s. Cap {usedUnits + 1}/{cap}", LogKind.Success);
                Changed?.Invoke();
                return true;
            }
            catch (Exception e)
            {
                reason = "Falha inesperada ao comprar contrato.";
                Logged?.Invoke($"[ERROR] BuyOrActivateContract: {e}", LogKind.Error);
                return false;
            }
        }

        public int GetContractsCap(string stageId)
        {
            var ex = GetExpedition(stageId);
            if (ex?.PartyId is null || ex.PartyId.Count == 0) return 0;

            int extra = _effects.ExtraContractsPerChar(); // mx00
            int cap = 0;
            foreach (var charId in ex.PartyId)
            {
                if (Current.Characters.TryGetValue(charId, out var c))
                    cap += Math.Max(0, c.MaxContracts + extra);
            }
            return cap;
        }
        #endregion

        #region Melhorias
        public bool BuyUpgrade(string upgradeId, out string? reason)
        {
            reason = null;

            if (string.IsNullOrWhiteSpace(upgradeId))
            { reason = "Upgrade inválida."; return false; }

            if (!Current.Upgrades.TryGetValue(upgradeId, out var u))
            { reason = "Upgrade inexistente."; return false; }

            if (!u.Avaliable)
            { reason = "Upgrade indisponível."; return false; }

            if (u.IsMaxed)
            { reason = $"Limite atingido ({u.Buys}/{u.MaxBuys})."; return false; }

            // ----- preço e moeda -----
            double price = UpgradePricingHelper.NextPrice(u);
            string resId = string.IsNullOrWhiteSpace(u.CostResourceId) ? "r001" : u.CostResourceId;

            if (!TrySpend(resId, price))
            { reason = $"Custa {price:N0} {resId}."; return false; }

            // ----- aplica compra -----
            u.Buys += 1;

            // disponibilidade pós-compra
            if (u.MaxBuys <= 1)
            {
                // upgrades one-shot
                u.Avaliable = false;
            }
            else
            {
                // multi-buy (ex.: mx00/mx01) continuam visíveis até esgotar
                u.Avaliable = !u.IsMaxed;
            }

            // recalc dos efeitos para tick/click/caps etc.
            _effects.Recompute(Current);

            Logged?.Invoke($"Upgrade {u.Name}: {u.Buys}/{u.MaxBuys}. Próx custo: {UpgradePricingHelper.NextPrice(u):N0} {resId}.", LogKind.Success);
            Changed?.Invoke();
            return true;
        }

        static bool IsUpgradeAvailable(GameModel m, UpgradeModel u)
        {
            if (u.TechId == null) return true; // sempre visível (mx00/mx01)
            return m.Technologies.TryGetValue(u.TechId, out var t) && t.Unlocked;
        }
        #endregion
    }
}
