using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Runtime.InteropServices;

namespace FurmaIdle.Services
{
    public interface IUpgradeService
    {
        // Recalcula cache (chamar em Attach, BuyUpgrade, unlock de tech etc.)
        void Recompute(GameModel model);

        // Ganho/tempo de contrato
        double ContractGainMult(string contractId);   // multiplicativo (empilha * )
        double ContractTimeMult(string contractId);   // multiplicativo (empilha * )

        // Bônus globais
        double ClicksGainMult();                      // multiplicativo (empilha * )
        double ResourceGenAddPerSecond(string resId); // aditivo (/s), pode somar por “all” e por res específico
        double ResourceGenMult(string resId);
        double SpecialtyCostMult(ResourceEnum.ResourceType resType);

        // Capacidade
        int ExtraContractsPerChar();                  // +N por personagem (ex.: mx00)
        double ResourceCapPerChar(string resId, double baseCap);

    }


    public sealed class UpgradeService : IUpgradeService
    {
        private readonly Dictionary<string, double> _gainMultByContract = new(StringComparer.Ordinal);
        private readonly Dictionary<string, double> _timeMultByContract = new(StringComparer.Ordinal);
        private readonly Dictionary<string, double> _resGenAddPerSec = new(StringComparer.Ordinal);
        private readonly Dictionary<string, double> _resGenMultById = new(StringComparer.Ordinal);
        private readonly Dictionary<string, double> _capMultByRes = new(StringComparer.Ordinal);
        private readonly Dictionary<string, double> _capAddByRes = new(StringComparer.Ordinal);

        private double _capMultAll = 1.0;
        private double _capAddAll = 0.0;
        private double _gainMultAll = 1.0;
        private double _timeMultAll = 1.0;
        private double _clicksGainMult = 1.0;
        private int _extraContractsPerChar = 0;
        private double _resGenMultAll = 1.0;

        public void Recompute(GameModel m)
        {
            // ---- RESETS DE CACHES ----
            _capMultByRes.Clear();
            _capAddByRes.Clear();
            _resGenMultById.Clear();
            _gainMultByContract.Clear();
            _timeMultByContract.Clear();
            _resGenAddPerSec.Clear();

            _gainMultAll = 1.0;
            _timeMultAll = 1.0;
            _clicksGainMult = 1.0;
            _extraContractsPerChar = 0;
            _resGenMultAll = 1.0;

            _capMultAll = 1.0;
            _capAddAll = 0.0;

            // custo de especialidades (novo: e03)
            _specCostMultResources = 1.0;

            if (m?.Runtime != null) m.Runtime.CharacterHireCostMult = 1.0;

            // ---- UPGRADES ----
            if (m?.Upgrades is not null)
            {
                foreach (var u in m.Upgrades.Values)
                {
                    if (u is null || u.Buys <= 0) continue;

                    foreach (var eff in u.Effects ?? Enumerable.Empty<UpgradeEffectModel>())
                    {
                        int qty = u.Buys;
                        string scope = eff.ScopeId ?? "all";

                        switch (eff.Target)
                        {
                            case EffectTarget.ContractGain:
                                if (scope == "all")
                                    _gainMultAll = ApplyMult(_gainMultAll, eff.Value, eff.Op, qty);
                                else
                                {
                                    var cur = _gainMultByContract.TryGetValue(scope, out var v) ? v : 1.0;
                                    _gainMultByContract[scope] = ApplyMult(cur, eff.Value, eff.Op, qty);
                                }
                                break;

                            case EffectTarget.ContractTime:
                                if (scope == "all")
                                    _timeMultAll = ApplyMult(_timeMultAll, eff.Value, eff.Op, qty);
                                else
                                {
                                    var cur = _timeMultByContract.TryGetValue(scope, out var v) ? v : 1.0;
                                    _timeMultByContract[scope] = ApplyMult(cur, eff.Value, eff.Op, qty);
                                }
                                break;

                            case EffectTarget.ClicksGain:
                                _clicksGainMult = ApplyMult(_clicksGainMult, eff.Value, eff.Op, qty);
                                break;

                            case EffectTarget.ResourceGen:
                                {
                                    var key = scope == "all" ? "__all__" : scope;
                                    var cur = _resGenAddPerSec.TryGetValue(key, out var v) ? v : 0.0;
                                    _resGenAddPerSec[key] = cur + eff.Value * qty; // aditivo (/s)
                                    break;
                                }

                            case EffectTarget.ContractCap:
                                if (scope == "all")
                                    _extraContractsPerChar += (int)(eff.Value * qty);
                                break;

                            case EffectTarget.ResourceCapPerChar:
                                {
                                    if (eff.Op == EffectOp.Multiplicative)
                                    {
                                        if (scope == "all") for (int i = 0; i < qty; i++) _capMultAll *= eff.Value;
                                        else
                                        {
                                            var cur = _capMultByRes.TryGetValue(scope, out var v) ? v : 1.0;
                                            for (int i = 0; i < qty; i++) cur *= eff.Value;
                                            _capMultByRes[scope] = cur;
                                        }
                                    }
                                    else
                                    {
                                        if (scope == "all") _capAddAll += eff.Value * qty;
                                        else
                                        {
                                            var cur = _capAddByRes.TryGetValue(scope, out var v) ? v : 0.0;
                                            _capAddByRes[scope] = cur + eff.Value * qty;
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                }
            }

            // ---- CONHECIMENTO → bônus global de produção (ganho de contratos) ----
            if (m?.Knowledges is not null)
            {
                foreach (var k in m.Knowledges.Values)
                {
                    if (k is null || k.Points <= 0) continue;

                    double n = k.Points;
                    double g = k.KnowCoinGain;         // ganho incremental por ponto
                    double p = k.KnowCoinGainPenaltie; // fator de penalidade (0< p <1)
                                                       // soma geométrica dos incrementos: 1 + g + g*p + g*p^2 + ...
                    double mult = 1.0 + g * (1.0 - Math.Pow(p, n)) / (1.0 - p);

                    _gainMultAll *= mult; // aplica como bônus global
                }
            }

            // ---- TRAÇOS (não aplique especialidades aqui para evitar duplicidade) ----
            ApplyTraits(m);

            // ---- ESPECIALIDADES ATIVAS (e01/e02/e03) ----
            // ÚNICO lugar onde aplicamos efeitos temporários de especialidades
            if (m?.Stages is not null)
            {
                var now = DateTimeOffset.UtcNow;

                foreach (var st in m.Stages.Values)
                {
                    var ex = st?.Expedition;
                    if (ex is null || ex.ExpeditionStatus != ExpeditionEnum.ExpeditionStatus.Active) continue;

                    foreach (var a in ex.ActiveSpecialties ?? Enumerable.Empty<ActiveSpecialtyModel>())
                    {
                        if (a is null || a.EndsAtUtc <= now) continue;

                        var sp = SpecialtyData.GetDef(a.SpecialtyId);
                        switch (sp.Id)
                        {
                            case "e01":
                                // Melhora geração passiva de recursos: x1.2
                                _resGenMultAll *= 1.20;
                                break;

                            case "e02":
                                // Melhora produção de contratos (moedas): x2.0 (NÃO mexer tempo)
                                _gainMultAll *= 2.0;
                                break;

                            case "e03":
                                // Diminui consumo de recursos para especialidades (apenas Resource, não Coin): x0.8
                                _specCostMultResources *= 0.8;
                                break;
                        }
                    }
                }
            }

            // pronto — efeitos/caches atualizados
        }


        // multiplicador helper sem ref
        private static double ApplyMult(double current, double val, EffectOp op, int times)
        {
            double r = current;
            if (op == EffectOp.Multiplicative)
            {
                for (int i = 0; i < times; i++) r *= val;
            }
            else
            {
                // aditivo: some ao fator (se estiver usando como “+x”)
                r += val * times;
            }
            return r;
        }

        public double ContractGainMult(string contractId)
        {
            var byId = _gainMultByContract.TryGetValue(contractId, out var v) ? v : 1.0;
            return _gainMultAll * byId;
        }

        public double ContractTimeMult(string contractId)
        {
            var byId = _timeMultByContract.TryGetValue(contractId, out var v) ? v : 1.0;
            return _timeMultAll * byId;
        }

        public double ClicksGainMult() => _clicksGainMult;

        public double ResourceGenAddPerSecond(string resId)
        {
            var all = _resGenAddPerSec.TryGetValue("__all__", out var a) ? a : 0.0;
            var spc = _resGenAddPerSec.TryGetValue(resId, out var b) ? b : 0.0;
            return all + spc;
        }

        public int ExtraContractsPerChar() => _extraContractsPerChar;

        public double ResourceCapPerChar(string resId, double baseCap)
        {
            if (baseCap <= 0) return 0; // 0 = sem limite
            var mult = _capMultAll * (_capMultByRes.TryGetValue(resId, out var m) ? m : 1.0);
            var add = _capAddAll + (_capAddByRes.TryGetValue(resId, out var a) ? a : 0.0);
            return Math.Max(0, (baseCap + add) * mult);
        }
        private static bool IsUpgradeUnlocked(GameModel m, string upgradeId)
        {
            return m?.Upgrades != null
                && m.Upgrades.TryGetValue(upgradeId, out var u)
                && (u.Unlocked || u.Buys > 0);
        }

        private void ApplyTraits(GameModel m)
        {
            foreach (var st in m.Stages.Values)
            {
                var ex = st.Expedition;
                var now = DateTimeOffset.UtcNow;

                if (ex?.ExpeditionStatus != ExpeditionEnum.ExpeditionStatus.Active) continue;
                if (ex.ActiveSpecialties is null) continue;

                foreach (var charId in ex.PartyId)
                {
                    if (!m.Characters.TryGetValue(charId, out var c)) continue;
                    if (string.IsNullOrWhiteSpace(c.TraitId)) continue;

                    var tr = TraitData.GetDef(c.TraitId);

                    // t04: só gerar se r100 estiver UNLOCKED OU se mx02 estiver comprada
                    if (tr.AddPerSecond != 0 && !string.IsNullOrWhiteSpace(tr.ResourceId))
                    {
                        var canGenerate =
                            (m.Resources.TryGetValue(tr.ResourceId, out var res) && res.Unlocked)
                            || IsUpgradeUnlocked(m, "mx02");

                        if (canGenerate)
                        {
                            var key = tr.ResourceId;
                            var cur = _resGenAddPerSec.TryGetValue(key, out var v) ? v : 0.0;
                            _resGenAddPerSec[key] = cur + tr.AddPerSecond;
                        }
                    }

                    // t03 – custo de contratação
                    if (tr.CharacterCostMult != 1.0 && m.Runtime != null)
                        m.Runtime.CharacterHireCostMult *= tr.CharacterCostMult;
                }
            }
        }

        public double ResourceGenMult(string resId)
        {
            var spc = _resGenMultById.TryGetValue(resId, out var v) ? v : 1.0;
            return _resGenMultAll * spc;
        }

        private static bool IsAlive(ActiveSpecialtyModel a, DateTimeOffset now) => a.EndsAtUtc > now;

        private void ApplyActiveSpecialties(GameModel m)
        {
            var now = DateTimeOffset.UtcNow;

            foreach (var st in m.Stages.Values)
            {
                var ex = st.Expedition;
                if (ex is null || ex.ExpeditionStatus != ExpeditionEnum.ExpeditionStatus.Active) continue;
                if (ex.ActiveSpecialties is null || ex.ActiveSpecialties.Count == 0) continue;

                // limpe vencidos (qualquer limpeza aqui ou no Tick também serve)
                ex.ActiveSpecialties.RemoveAll(a => !IsAlive(a, now));
                if (ex.ActiveSpecialties.Count == 0) continue;

                foreach (var a in ex.ActiveSpecialties)
                {
                    var spec = SpecialtyData.GetDef(a.SpecialtyId);

                    switch (spec.Target)
                    {
                        case SpecialtyTarget.Coins:
                            // e02: dobra coins (ganho de contratos) como multiplicador global
                            if (spec.Op == SpecialtyOp.Multiplicative)
                                _gainMultAll *= spec.Value;
                            break;

                        case SpecialtyTarget.Resources:
                            // e01, e03: multiplicar geração de um recurso específico
                            var rid = string.IsNullOrWhiteSpace(spec.ResourceIdScope) ? "__all__" : spec.ResourceIdScope;
                            if (spec.Op == SpecialtyOp.Multiplicative)
                            {
                                if (rid == "__all__") _resGenMultAll *= spec.Value;
                                else _resGenMultById[rid] = (_resGenMultById.TryGetValue(rid, out var cur) ? cur : 1.0) * spec.Value;
                            }
                            break;
                    }
                }
            }
        }

        // Services/UpgradeService.cs (campos privados)
        private double _specCostMultResources = 1.0; // custo de especialidade p/ recursos do tipo Resource

        public double SpecialtyCostMult(ResourceEnum.ResourceType resType)
        {
            return resType == ResourceEnum.ResourceType.Resource ? _specCostMultResources : 1.0;
        }


    }
}
