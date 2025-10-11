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

        // Capacidade
        int ExtraContractsPerChar();                  // +N por personagem (ex.: mx00)
    }


    public sealed class UpgradeService : IUpgradeService
    {
        private readonly Dictionary<string, double> _gainMultByContract = new(StringComparer.Ordinal);
        private readonly Dictionary<string, double> _timeMultByContract = new(StringComparer.Ordinal);
        private readonly Dictionary<string, double> _resGenAddPerSec = new(StringComparer.Ordinal);
        private readonly Dictionary<string, double> _resGenMultById = new(StringComparer.Ordinal);

        private double _gainMultAll = 1.0;
        private double _timeMultAll = 1.0;
        private double _clicksGainMult = 1.0;
        private int _extraContractsPerChar = 0;
        private double _resGenMultAll = 1.0;

        public void Recompute(GameModel m)
        {
            _gainMultByContract.Clear();
            _timeMultByContract.Clear();
            _resGenAddPerSec.Clear();
            _gainMultAll = 1.0;
            _timeMultAll = 1.0;
            _clicksGainMult = 1.0;
            _extraContractsPerChar = 0;
            _resGenMultById.Clear();
            _resGenMultAll = 1.0;

            if (m?.Runtime != null) m.Runtime.CharacterHireCostMult = 1.0;
            if (m?.Upgrades is null) return;

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
                                _resGenAddPerSec[key] = cur + eff.Value * qty; // aditivo
                            }
                            break;

                        case EffectTarget.ContractCap:
                            if (scope == "all")
                                _extraContractsPerChar += (int)(eff.Value * qty);
                            break;
                    }
                }
            }

            ApplyTraits(m);
            ApplyActiveSpecialties(m);
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
                if (ex is null || ex.ExpeditionStatus != ExpeditionEnum.ExpeditionStatus.Active) continue;

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

    }
}
