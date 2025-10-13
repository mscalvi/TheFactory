using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Channels;
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
        void Add(string id, double amount = 1, bool notify = true);
        void Click(string stageId);
        public double GetIncomePerSecondForStage(string stageId);
        double GetResourceCapPerChar(string resId);
        bool TrySpend(string resId, double amount);
        bool CanAfford(string resId, double amount);

        // Roster (seleção pré-start)
        bool ToggleRoster(string charId, out string? reason);
        int GetRosterCount();
        IReadOnlyCollection<string> GetRoster();

        // Expedition (por stage)
        ExpeditionModel? GetExpedition(string stageId);
        bool HasAnyExpeditionActive { get; }
        int GetEffectivePartyCap(string stageId);
        bool KnowledgeGain(string knowledgeId, int points, out string? reason);
        Dictionary<string, int> GetKnowledgePreview(string stageId);
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
        double GetContractProgress01(string stageId, string contractId);

        // Destinos
        bool BuyDestination(string destId, out string? reason);

        // Tecnologias
        bool BuyTech(string techId, out string? reason);

        // Melhorias
        bool BuyUpgrade(string upgradeId, out string? reason);

        // Characters
        bool BuyCharacter(string charId, out string? reason);

        // Especialidades
        bool ActivateSpecialty(string charId, out string? reason);
        (bool active, double remaining, double total) GetSpecialtyTimer(string charId);

        // Expansões
        bool BuyExpansion(string expId, out string? reason);

        // Gerais
        void Attach(GameModel model);
        event Action? Changed;
        event Action<string, LogKind>? Logged;
    }

    public sealed class GameService : IGameService
    {
        private readonly IUpgradeService _effects;
        private readonly IStageService _stages;
        private readonly IUnlockService _unlock;
        public GameModel Current { get; private set; } = new();
        public event Action? Changed;
        public event Action<string, LogKind>? Logged;

        public GameService(IUpgradeService effects, IStageService stages, IUnlockService unlock)
        {
            _effects = effects ?? throw new ArgumentNullException(nameof(effects));
            _stages = stages ?? throw new ArgumentNullException(nameof(stages));
            _unlock = unlock;
        }

        public void Attach(GameModel model)
        {
            Current = model ?? throw new ArgumentNullException(nameof(model));

            Current.Guild ??= new GuildModel();
            Current.Guild.Roster ??= new HashSet<string>();

            _unlock.RecomputeUpgradesAvailability(Current);
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

        public void Add(string resId, double amount = 1, bool notify = true)
        {
            if (string.IsNullOrWhiteSpace(resId) || amount == 0) return;
            var r = EnsureResource(resId);

            if (amount > 0)
            {
                // acumula fração e só aplica inteiros
                r.PendingFrac += amount;
                int inc = (int)Math.Floor(r.PendingFrac);
                if (inc > 0)
                {
                    r.PendingFrac -= inc;

                    // aplica cap por personagem (se expedição ativa)
                    var st = GetSelectedStage();
                    var ex = st?.Expedition;
                    if (ex?.ExpeditionStatus == ExpeditionEnum.ExpeditionStatus.Active)
                    {
                        int members = Math.Max(0, ex.PartyId?.Count ?? 0);
                        if (members > 0)
                        {
                            var capPerChar = _effects.ResourceCapPerChar(resId, r.CharacterCap);
                            if (capPerChar > 0)
                            {
                                double capTotal = capPerChar * members;
                                // aplica incremento respeitando o cap
                                var room = (int)Math.Max(0, Math.Floor(capTotal - r.Actual));
                                if (room <= 0) inc = 0;
                                else if (inc > room) inc = room;
                            }
                        }
                    }

                    if (inc > 0)
                    {
                        r.Actual += inc;
                        r.Total += inc;

                        // marca ganhos da run (só com expedição ativa)
                        var ex2 = GetSelectedStage()?.Expedition;
                        if (ex2?.ExpeditionStatus == ExpeditionEnum.ExpeditionStatus.Active)
                        {
                            ex2.RunGainsByRes ??= new Dictionary<string, double>(StringComparer.Ordinal);
                            ex2.RunGainsByRes.TryGetValue(resId, out var cur);
                            ex2.RunGainsByRes[resId] = cur + inc;
                        }
                    }
                }
            }
            else // amount < 0 (gastos eventuais que usem Add negativo — se você usa TrySpend, pode ignorar)
            {
                int dec = (int)Math.Ceiling(amount); // amount é negativo; arredonda para baixo em módulo inteiro
                if (dec != 0)
                {
                    // zera buffer porque vamos mexer no saldo diretamente
                    r.PendingFrac = 0;
                    r.Actual = Math.Max(0, r.Actual + dec); // dec é negativo
                                                            // Total não diminui em gastos
                }
            }

            if (notify) Changed?.Invoke();
        }

        public bool TrySpend(string resourceId, double amount)
        {
            if (string.IsNullOrWhiteSpace(resourceId) || !(amount > 0)) return false;
            var r = Get(resourceId);
            if (r is null || r.Actual < amount) return false;
            r.Actual -= amount;
            Changed?.Invoke();
            return true;
        }

        public bool CanAfford(string resId, double amount)
        {
            if (string.IsNullOrWhiteSpace(resId) || amount <= 0) return true;
            return Current.Resources.TryGetValue(resId, out var r) && r is not null && r.Actual >= amount;
        }

        public double GetIncomePerSecondForStage(string stageId)
        {
            if (!Current.Stages.TryGetValue(stageId, out var st) || st.Expedition is null) return 0;
            var ex = st.Expedition;
            double total = 0;

            foreach (var run in ex.ActiveContracts)
            {
                if (!ex.Contracts.TryGetValue(run.ContractId, out var c)) continue;

                var (_, cps, spc) = ContractsPricingHelper.ProdParams(c);
                if (!(c.Quant > 0 && cps > 0 && spc > 0)) continue;

                var g = _effects.ContractGainMult(c.Id);
                var t = _effects.ContractTimeMult(c.Id);
                total += (cps * g) / (spc * t) * c.Quant;
            }
            return total;
        }

        public double GetResourceCapPerChar(string resId)
        {
            if (!Current.Resources.TryGetValue(resId, out var r) || r is null)
                return 0;
            return _effects.ResourceCapPerChar(resId, r.CharacterCap);
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
            var stageCap = _stages.GetEffectivePartyCap(Current, stageId);
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

        public bool KnowledgeGain(string knowledgeId, int points, out string? reason)
        {
            reason = null;

            if (points <= 0) { reason = "Nada a ganhar."; return false; }
            if (Current.Knowledges is null || !Current.Knowledges.TryGetValue(knowledgeId, out var k))
            { reason = $"Conhecimento {knowledgeId} indisponível."; return false; }

            // só ganha se o conhecimento estiver “existente” no momento
            if (!k.Avaliable)
            { reason = $"{k.Name} está bloqueado."; return false; }

            k.Points += points;

            // opcional: marcar nos ganhos da expedição ativa (para UI/log)
            var st = GetSelectedStage();
            var ex = st?.Expedition;
            if (ex?.ExpeditionStatus == ExpeditionEnum.ExpeditionStatus.Active)
            {
                ex.RunKnowGains.TryGetValue(knowledgeId, out var cur);
                ex.RunKnowGains[knowledgeId] = cur + points;
            }

            Logged?.Invoke($"+{points} conhecimento {k.Name} ({knowledgeId}).", LogKind.Success);
            _effects.Recompute(Current);
            Changed?.Invoke();
            return true;
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

            // se não vier roster, tenta auto-preencher com todos da Base que couberem
            var requested = (roster ?? Array.Empty<string>()).ToList();
            if (requested.Count == 0)
            {
                var baseCandidates = GetBaseCandidates();
                if (baseCandidates.Count == 0)
                {
                    reason = "Ninguém na Base.";
                    return false;
                }

                if (baseCandidates.Count <= cap)
                {
                    requested = baseCandidates;
                }
                else
                {
                    reason = $"Selecione até {cap} membros dentre {baseCandidates.Count} disponíveis na Base.";
                    return false;
                }
            }

            if (requested.Count > cap) { reason = $"Seleção excede o limite ({cap})."; return false; }
            if (requested.Count < 1) { reason = "Selecione pelo menos 1 membro."; return false; }

            // Contratos (zera para nova run)
            ex.Contracts = ContractData.CreateInitialContracts();
            ex.ActiveContracts = new List<ContractRun>();
            ex.LockedContractByLevel = new Dictionary<int, string>();

            // Validação usando modelo robusto (sem NRE)
            var party = new List<CharacterModel>(requested.Count);
            foreach (var id in requested)
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
                c.CharStageId = stageId;
                ex.PartyId.Add(c.Id);
            }
            ex.ExpeditionStatus = ExpeditionStatus.Active;
            ex.Start = DateTimeOffset.UtcNow;

            // limpar seleção global sem NRE
            Current.Guild?.Roster?.Clear();

            string stageName = LookupData.Stage(Current, _stages, stageId).Name;
            Logged?.Invoke($"Expedição iniciada em {stageName}, com {ex.PartyId.Count} membros. Sobraram {Math.Max(0, cap - ex.PartyId.Count)} vagas. Boa aventura!", LogKind.Success);
            _unlock.ApplyStageEntry(Current, stageId);
            Changed?.Invoke();
            _effects.Recompute(Current);
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

            // Liberar personagens da expedição (sempre)
            var ids = (ex.PartyId ??= new List<string>()).ToList();
            foreach (var id in ids)
            {
                if (!Current.Characters.TryGetValue(id, out var c)) continue;
                c.CharState = CharStateEnum.CharState.InBase;
                c.CharStageId = null;
            }

            // Contratos e progresso
            ex.ActiveContracts?.Clear();
            ex.LockedContractByLevel?.Clear();
            if (ex.Contracts is not null)
            {
                foreach (var c in ex.Contracts.Values.Where(k => k.Quant > 0).ToList())
                {
                    if (c.Persistence == ResetPersistenceEnum.ResetPersistence.ExpeditionOnly)
                        c.Quant = 0;
                }
            }

            // Recursos – zera apenas o saldo atual
            if (Current.Resources is not null)
            {
                foreach (var r in Current.Resources.Values)
                {
                    if (r.Persistence == ResetPersistenceEnum.ResetPersistence.ExpeditionOnly)
                    {
                        r.Actual = 0;
                    }
                }
            }

            foreach (var u in Current.Upgrades.Values)
            {
                if (u.Persistence == ResetPersistenceEnum.ResetPersistence.ExpansionOnly)
                {
                    u.Buys = 0;
                }
            }

            if (ex is not null)
            {
                ex.RunGainsByRes ??= new();
                var coinResId = st.ResourceId;
                ex.RunGainsByRes.TryGetValue(coinResId, out var coins);

                if (coins > 0)
                {
                    if (ex is null) goto _after_knowledge;

                    // 1) Coleta PESOS (GainFactor) por Knowledge
                    var weights = new Dictionary<string, int>(StringComparer.Ordinal);

                    // 1.a) Personagens da party: Main +2, Second +1
                    foreach (var charId in ex.PartyId)
                    {
                        if (!Current.Characters.TryGetValue(charId, out var ch)) continue;
                        if (!string.IsNullOrWhiteSpace(ch.MainKnowId))
                            weights[ch.MainKnowId] = (weights.TryGetValue(ch.MainKnowId, out var w) ? w : 0) + 2;
                        if (!string.IsNullOrWhiteSpace(ch.SecondKnowId))
                            weights[ch.SecondKnowId] = (weights.TryGetValue(ch.SecondKnowId, out var w) ? w : 0) + 1;
                    }

                    // 1.b) Contratos ativos: First +1, Second +2, Third +3 — multiplicado por Quant
                    if (ex.Contracts is not null)
                    {
                        foreach (var c in ex.Contracts.Values)
                        {
                            if (c.Quant <= 0) continue;

                            void addW(string? kid, int w)
                            {
                                if (string.IsNullOrWhiteSpace(kid)) return;
                                int inc = w * Math.Max(1, c.Quant);
                                weights[kid] = (weights.TryGetValue(kid, out var cur) ? cur : 0) + inc;
                            }

                            addW(c.FirstKnowId, 1);
                            addW(c.SecondKnowId, 2);
                            addW(c.ThirdKnowId, 3);
                        }
                    }

                    // filtra só knowledges existentes e “presentes” no momento
                    var aliveWeights = new Dictionary<string, int>(StringComparer.Ordinal);
                    foreach (var kv in weights)
                    {
                        if (!Current.Knowledges.TryGetValue(kv.Key, out var k)) continue;
                        if (!k.Avaliable) continue;
                        if (kv.Value > 0) aliveWeights[kv.Key] = kv.Value;
                    }

                    if (aliveWeights.Count == 0)
                        goto _after_knowledge;

                    // 2) Base por conhecimento
                    //    base_k = (coins / CoinsBase)^Curve * BaseMult
                    var baseByK = new Dictionary<string, double>(StringComparer.Ordinal);
                    foreach (var kid in aliveWeights.Keys)
                    {
                        var k = Current.Knowledges[kid];
                        if (k.GainBase <= 0) { baseByK[kid] = 0; continue; }
                        var raw = Math.Pow(coins / k.GainBase, k.GainCurve) * k.GainMultiplier;
                        baseByK[kid] = Math.Max(0, raw);
                    }

                    // 3) Total de pontos (média ponderada pelos pesos)
                    double sumW = aliveWeights.Values.Sum();
                    double blendedBase = baseByK.Sum(x => x.Value * aliveWeights[x.Key]) / Math.Max(1.0, sumW);
                    int totalPoints = (int)Math.Floor(blendedBase);
                    if (totalPoints <= 0) goto _after_knowledge;

                    // 4) Divide proporcionalmente pelos pesos, com correção de sobra
                    var provisional = new Dictionary<string, int>(StringComparer.Ordinal);
                    var remainders = new List<(string kid, double frac)>(aliveWeights.Count);
                    int assigned = 0;

                    foreach (var (kid, w) in aliveWeights)
                    {
                        double share = totalPoints * (w / sumW);
                        int pts = (int)Math.Floor(share);
                        provisional[kid] = pts;
                        assigned += pts;
                        remainders.Add((kid, share - pts));
                    }

                    int leftover = totalPoints - assigned;
                    if (leftover > 0)
                    {
                        foreach (var (kid, _) in remainders.OrderByDescending(r => r.frac))
                        {
                            provisional[kid] += 1;
                            leftover--;
                            if (leftover == 0) break;
                        }
                    }

                    // 5) Concede
                    foreach (var (kid, pts) in provisional)
                        if (pts > 0) KnowledgeGain(kid, pts, out _);

                        _after_knowledge:;
                }


                ex.RunGainsByRes?.Clear();
            }

                ids.Clear();
            ex.ExpeditionStatus = ExpeditionStatus.Idle;
            ex.Start = null;

            Current.Guild?.Roster?.Clear();

            _effects.Recompute(Current);
            RecomputeUpgradesUnlockedAndAvailability();

            Changed?.Invoke();
            return true;
        }

        private static bool ComputeUpgradeUnlocked(GameModel m, UpgradeModel u)
        {
            if (u is null) return false;

            // Gate por tecnologia (exemplo)
            if (!string.IsNullOrWhiteSpace(u.TechId))
                if (!m.Technologies.TryGetValue(u.TechId, out var t) || !t.Unlocked)
                    return false;

            // (Se tiver gates por destino/stage, adicione aqui)

            return true;
        }

        public Dictionary<string, int> GetKnowledgePreview(string stageId)
        {
            var result = new Dictionary<string, int>(StringComparer.Ordinal);

            if (!Current.Stages.TryGetValue(stageId, out var st) || st?.Expedition is null)
                return result;

            var ex = st.Expedition;
            if (ex.ExpeditionStatus != ExpeditionEnum.ExpeditionStatus.Active)
                return result;

            // coins acumuladas nesta run para o recurso do stage
            ex.RunGainsByRes ??= new();
            var coinResId = st.ResourceId;
            ex.RunGainsByRes.TryGetValue(coinResId, out var coins);
            if (coins <= 0) return result;

            // 1) pesos (GainFactor) por knowledge
            var weights = new Dictionary<string, int>(StringComparer.Ordinal);

            // party: Main +2, Second +1
            foreach (var charId in ex.PartyId)
            {
                if (!Current.Characters.TryGetValue(charId, out var ch)) continue;
                if (!string.IsNullOrWhiteSpace(ch.MainKnowId))
                    weights[ch.MainKnowId] = (weights.TryGetValue(ch.MainKnowId, out var w) ? w : 0) + 2;
                if (!string.IsNullOrWhiteSpace(ch.SecondKnowId))
                    weights[ch.SecondKnowId] = (weights.TryGetValue(ch.SecondKnowId, out var w) ? w : 0) + 1;
            }

            // contratos: First +1, Second +2, Third +3, multiplicado por Quant
            if (ex.Contracts is not null)
            {
                foreach (var c in ex.Contracts.Values)
                {
                    if (c.Quant <= 0) continue;

                    void addW(string? kid, int w)
                    {
                        if (string.IsNullOrWhiteSpace(kid)) return;
                        int inc = w * Math.Max(1, c.Quant);
                        weights[kid] = (weights.TryGetValue(kid, out var cur) ? cur : 0) + inc;
                    }

                    addW(c.FirstKnowId, 1);
                    addW(c.SecondKnowId, 2);
                    addW(c.ThirdKnowId, 3);
                }
            }

            // filtra knowledges válidos e disponíveis
            var aliveWeights = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var kv in weights)
            {
                if (!Current.Knowledges.TryGetValue(kv.Key, out var k)) continue;
                if (!k.Avaliable) continue;
                if (kv.Value > 0) aliveWeights[kv.Key] = kv.Value;
            }
            if (aliveWeights.Count == 0) return result;

            // 2) base_k pela fórmula (coins / base)^curve * mult
            var baseByK = new Dictionary<string, double>(StringComparer.Ordinal);
            foreach (var kid in aliveWeights.Keys)
            {
                var k = Current.Knowledges[kid];
                if (k.GainBase <= 0) { baseByK[kid] = 0; continue; }
                var raw = Math.Pow(coins / k.GainBase, k.GainCurve) * k.GainMultiplier;
                baseByK[kid] = Math.Max(0, raw);
            }

            // 3) totalPoints = floor(média ponderada pelos pesos)
            double sumW = aliveWeights.Values.Sum();
            double blendedBase = baseByK.Sum(x => x.Value * aliveWeights[x.Key]) / Math.Max(1.0, sumW);
            int totalPoints = (int)Math.Floor(blendedBase);
            if (totalPoints <= 0) return result;

            // 4) divide proporcionalmente + sobra pelos maiores restos
            var provisional = new Dictionary<string, int>(StringComparer.Ordinal);
            var remainders = new List<(string kid, double frac)>(aliveWeights.Count);
            int assigned = 0;

            foreach (var (kid, w) in aliveWeights)
            {
                double share = totalPoints * (w / sumW);
                int pts = (int)Math.Floor(share);
                provisional[kid] = pts;
                assigned += pts;
                remainders.Add((kid, share - pts));
            }

            int leftover = totalPoints - assigned;
            if (leftover > 0)
            {
                foreach (var (kid, _) in remainders.OrderByDescending(r => r.frac))
                {
                    provisional[kid] += 1;
                    leftover--;
                    if (leftover == 0) break;
                }
            }

            // 5) devolve
            foreach (var (kid, pts) in provisional)
                if (pts > 0) result[kid] = pts;

            return result;
        }

        private void RecomputeUpgradesUnlockedAndAvailability()
        {
            foreach (var u in Current.Upgrades.Values)
            {
                u.Unlocked = ComputeUpgradeUnlocked(Current, u);
                u.Avaliable = u.Unlocked && !u.IsMaxed;
            }
        }

        private List<string> GetBaseCandidates()
        {
            var ids = new List<string>();
            foreach (var c in Current.Characters.Values)
            {
                if (c.CharState == CharStateEnum.CharState.InBase
                    && !IsCharacterEngagedInAnyExpeditionSafe(c.Id))
                {
                    ids.Add(c.Id);
                }
            }
            return ids;
        }
        #endregion

        #region Ticks
        public void Tick(double dtSeconds)
        {
            // saneamento do delta
            if (!(dtSeconds > 0) || double.IsNaN(dtSeconds) || double.IsInfinity(dtSeconds)) return;

            const double MaxStep = 0.25;
            int steps = (int)Math.Ceiling(dtSeconds / MaxStep);
            double step = dtSeconds / steps;

            bool anyChange = false;
            bool effectsDirty = false;

            for (int s = 0; s < steps; s++)
            {
                foreach (var st in Current.Stages.Values)
                {
                    var ex = st.Expedition;
                    if (ex?.ExpeditionStatus != ExpeditionEnum.ExpeditionStatus.Active) continue;
                    if (ex.ActiveContracts is null || ex.Contracts is null) continue;

                    var before = ex.ActiveSpecialties.Count;
                    ex.ActiveSpecialties.RemoveAll(a => a.EndsAtUtc <= DateTimeOffset.UtcNow);
                    if (ex.ActiveSpecialties.Count != before)
                        effectsDirty = true;

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
                        double secondsPerCycle = Math.Max(0.02, bal.SecondsPerCycle * timeMult);

                        // avança progresso
                        run.ProgressSec += step;

                        if (run.ProgressSec >= secondsPerCycle)
                        {
                            double cycles = Math.Floor(run.ProgressSec / secondsPerCycle);
                            if (cycles >= 1.0)
                            {
                                double amount = cycles * coinsPerCycle * c.Quant;
                                if (amount > 0)
                                {
                                    Add(bal.ResourceId, amount, notify: false);
                                    anyChange = true;
                                }
                                run.ProgressSec -= cycles * secondsPerCycle;
                            }
                        }
                    }
                }

                // ---- GERAÇÃO PASSIVA POR RECURSO
                if (Current.Resources is not null && HasAnyExpeditionActive) // <- guard
                {
                    foreach (var r in Current.Resources.Values)
                    {
                        if (!r.Unlocked) continue;

                        double add = _effects.ResourceGenAddPerSecond(r.Id); // aditivo
                        double mul = _effects.ResourceGenMult(r.Id);         // multiplicativo
                        double eff = add * mul;

                        if (eff > 0)
                        {
                            Add(r.Id, eff * step, notify: false);
                            anyChange = true;
                        }
                    }
                }
            }

            if (effectsDirty)
            {
                _effects.Recompute(Current);
                anyChange = true;
            }

            Current.LastTickUtc = DateTimeOffset.UtcNow;
            RecomputePerSecond();
            if (anyChange) Changed?.Invoke();
        }
        private void RecomputePerSecond()
        {
            // zera
            if (Current.Resources != null)
                foreach (var r in Current.Resources.Values) r.PerSecond = 0;

            foreach (var st in Current.Stages.Values)
            {
                var ex = st.Expedition;
                // ✅ use o enum qualificado que você realmente tem
                if (ex?.ExpeditionStatus != ExpeditionEnum.ExpeditionStatus.Active) continue;

                if (ex.ActiveContracts is null || ex.Contracts is null) continue;

                foreach (var run in ex.ActiveContracts)
                {
                    if (!ex.Contracts.TryGetValue(run.ContractId, out var c)) continue;
                    if (!ContractsPricingHelper.TryGetBalance(c, out var bal)) continue;

                    var gain = _effects.ContractGainMult(c.Id);
                    var time = _effects.ContractTimeMult(c.Id);

                    var cps = bal.CoinsPerCycle * gain;
                    var spc = Math.Max(0.02, bal.SecondsPerCycle * time);
                    if (c.Quant <= 0 || cps <= 0 || spc <= 0) continue;

                    var rate = (cps / spc) * c.Quant; // /s
                    var r = EnsureResource(bal.ResourceId);
                    r.PerSecond += rate;
                }
            }

            // geração passiva
            if (Current.Resources != null)
            {
                bool anyActive = HasAnyExpeditionActive;
                foreach (var r in Current.Resources.Values)
                {
                    if (!r.Unlocked) continue;

                    if (anyActive)
                    {
                        double add = _effects.ResourceGenAddPerSecond(r.Id);
                        double mult = _effects.ResourceGenMult(r.Id);
                        r.PerSecond += Math.Max(0, add * mult);
                    }
                }
            }
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
                {   return false;
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
                Changed?.Invoke();
                return true;
            }
            catch (Exception e)
            {
                reason = "Falha inesperada ao comprar contrato.";
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

        public double GetContractProgress01(string stageId, string contractId)
        {
            if (!Current.Stages.TryGetValue(stageId, out var st)) return 0;
            var ex = st.Expedition;
            if (ex?.ExpeditionStatus != ExpeditionEnum.ExpeditionStatus.Active) return 0;

            // precisa do run (tem o ProgressSec)
            var run = ex.ActiveContracts?.FirstOrDefault(r => r.ContractId == contractId);
            if (run is null) return 0;

            // precisa do contrato e do balance para calcular o secondsPerCycle efetivo
            if (!ex.Contracts.TryGetValue(contractId, out var c)) return 0;
            if (!ContractsPricingHelper.TryGetBalance(c, out var bal)) return 0;

            var gainMult = _effects.ContractGainMult(c.Id);
            var timeMult = _effects.ContractTimeMult(c.Id);
            var secondsPerCycle = Math.Max(0.02, bal.SecondsPerCycle * timeMult);

            var ratio = secondsPerCycle <= 0 ? 0 : run.ProgressSec / secondsPerCycle;
            if (ratio < 0) ratio = 0;
            if (ratio > 1) ratio = 1;
            return ratio;
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
            _unlock.RecomputeUpgradesAvailability(Current);
            if (upgradeId == "mx99")
            {
                const string goldId = "r001";    // sua moeda base
                const double reward = 5000;

                if (Current.Resources.TryGetValue(goldId, out var gold))
                {
                    gold.Actual += reward;
                    gold.Total += reward;
                }
                else
                {
                    // se por algum motivo não existir ainda, cria o estado mínimo
                    var def = ResourceData.GetDef(goldId);
                    var r = new ResourceModel
                    {
                        Id = def.Id,
                        Name = def.Name,
                        Image = def.Image,
                        Sort = def.Sort,
                        ResourceType = def.ResourceType,
                        Unlocked = true,
                        Avaliable = true,
                        Actual = reward,
                        Total = reward
                    };
                    Current.Resources[goldId] = r;
                }

                Logged?.Invoke($"+{reward:N0} {goldId} via {upgradeId}.", LogKind.Success);
            }
            if (upgradeId == "mx02")
            {
                const string rId = "r100";

                // pega definição
                var def = ResourceData.GetDef(rId);

                // cria se não existir no estado
                if (!Current.Resources.TryGetValue(rId, out var r))
                {
                    r = new ResourceModel { Id = def.Id };
                    Current.Resources[rId] = r;
                }

                // sincroniza estáticos
                r.Name = def.Name;
                r.Image = def.Image;
                r.Sort = def.Sort;
                r.ResourceType = def.ResourceType;
                r.Persistence = def.Persistence;

                // libera para uso
                r.Avaliable = true;
                r.Unlocked = true;

                Logged?.Invoke($"Recurso {r.Name} habilitado.", LogKind.Success);
            }

            _effects.Recompute(Current);
            Changed?.Invoke();
            return true;
        }

        #endregion

        #region Destinos
        public bool BuyDestination(string destId, out string? reason)
        {
            reason = null;

            if (string.IsNullOrWhiteSpace(destId))
            { reason = "Destino inválido."; return false; }

            if (!Current.Destinations.TryGetValue(destId, out var d))
            { reason = "Destino inexistente."; return false; }

            if (!d.Avaliable)
            { reason = "Destino indisponível."; return false; }

            if (d.Unlocked)
            { reason = "Destino já adquirido."; return false; }

            var wasUnlocked = d.Unlocked;

            // Preço e moeda
            var price = d.Cost;
            var resId = d.CostResourceId;
            if (!(price > 0)) { price = 0; } // permitir custo 0 (ex.: d00)

            if (price > 0 && !TrySpend(resId, price))
            { reason = "Saldo insuficiente."; return false; }

            // Marca como comprado (unlocked)
            d.Unlocked = true;
            d.Avaliable = false;

            // Caso queira algum efeito imediato pós-compra (opcional no futuro):
            // _unlock.Apply(Current, "dest", destId);

            if (!wasUnlocked)
                _unlock.ApplyDestinationPurchase(Current, destId);

            _effects.Recompute(Current);
            Changed?.Invoke();

            Logged?.Invoke($"Destino adquirido: {d.Name}.", LogKind.Success);
            return true;
        }
        #endregion

        #region Tecnologias
        public bool BuyTech(string techId, out string? reason)
        {
            reason = null;

            if (!Current.Technologies.TryGetValue(techId, out var tech))
            { reason = "Tecnologia inexistente."; return false; }

            if (tech.Unlocked) { reason = "Já pesquisada."; return false; }
            if (!tech.Avaliable) { reason = "Indisponível."; return false; }

            var def = TechData.GetDef(techId);
            var kId = def.CostKnowledgeId;
            var cost = def.Cost;

            if (!Current.Knowledges.TryGetValue(kId, out var k))
            { reason = $"Conhecimento {kId} indisponível."; return false; }

            if (k.Points < cost)
            { reason = $"Precisa de {cost} {k.Name} (você tem {k.Points})."; return false; }

            k.Points -= cost;

            tech.Unlocked = true;
            tech.Avaliable = false;

            _unlock.ApplyTechPurchase(Current, techId);

            _effects.Recompute(Current);
            Logged?.Invoke($"Tecnologia {techId} pesquisada.", LogKind.Success);
            Changed?.Invoke();
            return true;
        }

        #endregion

        #region Personagens
        public bool BuyCharacter(string charId, out string? reason)
        {
            reason = null;

            if (!Current.Characters.TryGetValue(charId, out var live))
            { reason = "Personagem inexistente."; return false; }

            if (!live.Avaliable) { reason = "Indisponível para contratação."; return false; }
            if (live.Unlocked) { reason = "Já contratado."; return false; }

            // custo do catálogo (fonte da verdade)
            var def = CharacterData.GetDef(charId);
            var resId = string.IsNullOrWhiteSpace(def.CostResourceId) ? "r001" : def.CostResourceId;
            var baseCost = Math.Max(0, def.Cost);

            // multiplicador de traço (tr03). Se você não usa runtime, deixe = 1.0
            var mult = Current.Runtime?.CharacterHireCostMult ?? 1.0;
            var effective = Math.Ceiling(baseCost * mult);

            if (effective > 0 && !TrySpend(resId, effective))
            {
                reason = $"Custa {effective:N0} {resId}.";
                return false;
            }

            live.Unlocked = true;
            live.Avaliable = false;
            live.CharState = CharStateEnum.CharState.InBase;
            live.CharStageId = null;

            _effects.Recompute(Current);  
            Logged?.Invoke($"{charId} contratado por {effective:N0} {resId}.", LogKind.Success);
            Changed?.Invoke();
            return true;
        }


        #endregion

        #region Especialidades
        public bool ActivateSpecialty(string charId, out string? reason)
        {
            reason = null;

            var st = GetSelectedStage();
            var ex = st?.Expedition;
            if (ex is null || ex.ExpeditionStatus != ExpeditionEnum.ExpeditionStatus.Active)
            { reason = "Precisa estar em expedição."; return false; }

            if (!Current.Characters.TryGetValue(charId, out var ch))
            { reason = "Personagem inexistente."; return false; }

            if (ch.CharState != CharStateEnum.CharState.OnStage ||
                ch.CharStageId != st!.Id || !ex.PartyId.Contains(charId))
            { reason = "Personagem não está na expedição."; return false; }

            if (string.IsNullOrWhiteSpace(ch.SpecialtyId))
            { reason = "Personagem não possui especialidade."; return false; }

            var spec = SpecialtyData.GetDef(ch.SpecialtyId);

            // remove efeitos vencidos e checa bloqueios
            var now = DateTimeOffset.UtcNow;
            ex.ActiveSpecialties ??= new List<ActiveSpecialtyModel>();
            ex.ActiveSpecialties.RemoveAll(a => a.EndsAtUtc <= now);

            // outras: impedir reativar mesma especialidade do mesmo personagem enquanto durar
            if (spec.Id != "e00" &&
                ex.ActiveSpecialties.Any(a => a.CharId == charId && a.SpecialtyId == spec.Id && a.EndsAtUtc > now))
            { reason = "Especialidade já ativa."; return false; }

            // detecta o tipo do recurso que paga a especialidade
            // pagar custo (NÃO somar em Total nem usar Add; é gasto!)
            var costRes = spec.CostResourceId ?? "r100";
            var cost = Math.Max(0, spec.Cost);
            var coinId = st?.ResourceId;
            var baseCost = Math.Max(0, spec.Cost);

            var resType = ResourceEnum.ResourceType.Resource;
            if (Current.Resources.TryGetValue(costRes, out var rpay) && rpay is not null)
                resType = rpay.ResourceType;

            var mult = _effects.SpecialtyCostMult(resType);
            var effCost = Math.Floor(baseCost * mult);

            if (!TrySpend(costRes, cost))
            {
                reason = $"Precisa de {cost:N0} {costRes}.";
                return false;
            }

            // e00: produção instantânea = 20s do PerSecond ATUAL
            if (spec.Id == "e00")
            {
                RecomputePerSecond();

                const double burstSec = 20;
                if (Current.Resources != null)
                {
                    foreach (var r in Current.Resources.Values)
                    {
                        if (!r.Unlocked) continue;
                        if (r.ResourceType != ResourceEnum.ResourceType.Coin) continue;
                        if (!string.Equals(r.Id, coinId, StringComparison.Ordinal)) continue;

                        var amount = r.PerSecond * burstSec;
                        if (amount > 0) Add(r.Id, amount, notify: false);
                    }
                }

                Logged?.Invoke($"Produção instantânea realizada (+{burstSec:0}s).", LogKind.Success);
                _effects.Recompute(Current);
                Changed?.Invoke();
                return true;
            }

            // Demais especialidades (e01/e02/e03…): apenas agenda buff temporário
            ex.ActiveSpecialties.Add(new ActiveSpecialtyModel
            {
                SpecialtyId = spec.Id,
                CharId = charId,
                EndsAtUtc = now.AddSeconds(spec.DurationSec)
            });

            Logged?.Invoke($"Especialidade ativada por {ch.Id}.", LogKind.Success);
            _effects.Recompute(Current);
            Changed?.Invoke();
            return true;
        }

        public (bool active, double remaining, double total) GetSpecialtyTimer(string charId)
        {
            var st = GetSelectedStage();
            var ex = st?.Expedition;
            if (ex?.ActiveSpecialties is null || ex.ActiveSpecialties.Count == 0)
                return (false, 0, 0);

            var now = DateTimeOffset.UtcNow;
            foreach (var a in ex.ActiveSpecialties)
            {
                if (a.CharId != charId) continue;
                var spec = SpecialtyData.GetDef(a.SpecialtyId);
                if (a.EndsAtUtc <= now) continue;

                var total = Math.Max(0.001, spec.DurationSec);
                var remaining = (a.EndsAtUtc - now).TotalSeconds;
                remaining = Math.Max(0, Math.Min(remaining, total));
                return (true, remaining, total);
            }
            return (false, 0, 0);
        }

        #endregion

        #region Expansões
        public bool BuyExpansion(string expId, out string? reason)
        {
            reason = null;

            // sanity
            if (string.IsNullOrWhiteSpace(expId))
            { reason = "Expansão inválida."; return false; }

            if (Current.Expansions is null || !Current.Expansions.TryGetValue(expId, out var exp) || exp is null)
            { reason = "Expansão inexistente."; return false; }

            if (!exp.Avaliable)
            { reason = "Expansão ainda não está disponível."; return false; }

            if (exp.Unlocked)
            { reason = "Expansão já comprada."; return false; }

            // pagamento
            var costRes = string.IsNullOrWhiteSpace(exp.CoinCostId) ? "r001" : exp.CoinCostId;
            var cost = Math.Max(0, exp.Cost);

            if (!TrySpend(costRes, cost))
            {
                reason = $"Precisa de {cost:N0} {costRes}.";
                return false;
            }

            // marca comprada antes do reset (para persistir status)
            exp.Unlocked = true;

            // HARD RESET
            try
            {
                HardReset(expId);
            }
            catch (Exception ex)
            {
                exp.Unlocked = false;
                Add(costRes, cost, notify: false);
                reason = "Falha ao aplicar a Expansão.";
                Console.WriteLine($"[EXPANSION] HardReset error: {ex}");
                return false;
            }

            // efeitos e UI
            _effects.Recompute(Current);
            Logged?.Invoke($"Expansão {expId} concluída.", LogKind.Success);
            Changed?.Invoke();
            return true;
        }

        public void HardReset(string sourceExpansionId)
        {
            // 1) Finaliza expedições ativas (sem distribuir conhecimento)
            foreach (var st in Current.Stages.Values)
            {
                var ex = st.Expedition;
                if (ex?.ExpeditionStatus == ExpeditionEnum.ExpeditionStatus.Active)
                {
                    EndExpedition(st.Id);
                }
            }

            // 2) Zera estado por persistência
            // IMPORTANTE: ajuste os nomes abaixo conforme seus modelos/enum

            // 2.1 Resources
            if (Current.Resources is not null)
            {
                foreach (var r in Current.Resources.Values)
                {
                    r.Actual = 0;

                    switch (r.Persistence)
                    {
                        case ResetPersistenceEnum.ResetPersistence.Permanent:
                            // mantém Total/unlocks
                            break;

                        case ResetPersistenceEnum.ResetPersistence.ExpeditionOnly:
                            // no hard reset também limpa o que já é limpo no soft
                            // (Total normalmente preservado, mas mantenha sua regra)
                            break;

                        case ResetPersistenceEnum.ResetPersistence.ExpansionOnly:
                            // limpa tudo que só deveria durar até a expansão
                            r.Total = 0;
                            r.Unlocked = false;
                            r.Avaliable = false;
                            break;
                    }
                }
            }

            // 2.2 Upgrades
            if (Current.Upgrades is not null)
            {
                foreach (var u in Current.Upgrades.Values)
                {
                    switch (u.Persistence)
                    {
                        case ResetPersistenceEnum.ResetPersistence.Permanent:
                            // mantém compras
                            break;

                        case ResetPersistenceEnum.ResetPersistence.ExpeditionOnly:
                        case ResetPersistenceEnum.ResetPersistence.ExpansionOnly:
                            u.Buys = 0;
                            u.Unlocked = false;
                            break;
                    }
                }
            }

            // 2.3 Technologies
            if (Current.Technologies is not null)
            {
                foreach (var t in Current.Technologies.Values)
                {
                    switch (t.Persistence)
                    {
                        case ResetPersistenceEnum.ResetPersistence.Permanent:
                            break;

                        case ResetPersistenceEnum.ResetPersistence.ExpeditionOnly:
                        case ResetPersistenceEnum.ResetPersistence.ExpansionOnly:
                            t.Unlocked = false;
                            break;
                    }
                }
            }

            // 2.4 Characters
            if (Current.Characters is not null)
            {
                foreach (var c in Current.Characters.Values)
                {
                    c.CharState = CharStateEnum.CharState.InBase;
                    c.CharStageId = null;

                    switch (c.Persistence)
                    {
                        case ResetPersistenceEnum.ResetPersistence.Permanent:
                            break;

                        case ResetPersistenceEnum.ResetPersistence.ExpeditionOnly:
                        case ResetPersistenceEnum.ResetPersistence.ExpansionOnly:
                            c.Unlocked = false;
                            c.Avaliable = false;
                            break;
                    }
                }
            }

            // 2.5 Destinos / Stages (limpeza de estado runtime)
            if (Current.Stages is not null)
            {
                foreach (var st in Current.Stages.Values)
                {
                    var ex = st.Expedition;
                    if (ex is null) continue;

                    // limpa SEMPRE os dados voláteis de run
                    ex.ActiveContracts?.Clear();
                    ex.LockedContractByLevel?.Clear();
                    ex.Contracts?.Clear();
                    ex.PartyId?.Clear();
                    ex.RunGainsByRes?.Clear();
                    ex.RunKnowGains?.Clear();
                    ex.ActiveSpecialties?.Clear();

                    ex.ExpeditionStatus = ExpeditionEnum.ExpeditionStatus.Idle;
                    ex.Start = null;
                }
            }

            if (Current.Destinations is not null)
            {
                foreach (var d in Current.Destinations.Values)
                {
                    switch (d.Persistence)
                    {
                        case ResetPersistenceEnum.ResetPersistence.Permanent:
                            break;

                        case ResetPersistenceEnum.ResetPersistence.ExpeditionOnly:
                        case ResetPersistenceEnum.ResetPersistence.ExpansionOnly:
                            d.Unlocked = false;
                            d.Avaliable = false;
                            break;
                    }
                }
            }

            // 3) Reaplicar desbloqueios “base” para o stage atual
            //    (recurso do stage, destinos daquele stage, tecnologias e contratos daquele destino etc.)
            var stageId = SelectedStageId;
            _unlock.ApplyStageEntry(Current, stageId);

            // 4) Recomputar efeitos e atualizar UI
            _effects.Recompute(Current);
            Changed?.Invoke();
            Logged?.Invoke($"Hard reset concluído pela expansão {sourceExpansionId}.", LogKind.Success);
        }

        #endregion
    }

}
