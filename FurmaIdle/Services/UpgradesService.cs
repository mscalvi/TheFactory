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

        // Capacidade
        int ExtraContractsPerChar();                  // +N por personagem (ex.: mx00)
    }


    public sealed class UpgradeService : IUpgradeService
    {
        private readonly Dictionary<string, double> _gainMultByContract = new(StringComparer.Ordinal);
        private readonly Dictionary<string, double> _timeMultByContract = new(StringComparer.Ordinal);
        private readonly Dictionary<string, double> _resGenAddPerSec = new(StringComparer.Ordinal);

        private double _gainMultAll = 1.0;
        private double _timeMultAll = 1.0;
        private double _clicksGainMult = 1.0;
        private int _extraContractsPerChar = 0;

        public void Recompute(GameModel m)
        {
            _gainMultByContract.Clear();
            _timeMultByContract.Clear();
            _resGenAddPerSec.Clear();
            _gainMultAll = 1.0;
            _timeMultAll = 1.0;
            _clicksGainMult = 1.0;
            _extraContractsPerChar = 0;

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

        private void ApplyTraits(GameModel m)
        {
            // Personagens na expedição selecionada (ou todas expedições ativas, se houver múltiplas)
            foreach (var st in m.Stages.Values)
            {
                var ex = st.Expedition;
                if (ex is null || ex.ExpeditionStatus != ExpeditionEnum.ExpeditionStatus.Active) continue;

                foreach (var charId in ex.PartyId)
                {
                    if (!m.Characters.TryGetValue(charId, out var c)) continue;
                    if (string.IsNullOrWhiteSpace(c.TraitId)) continue;

                    var trait = TraitData.GetDef(c.TraitId);

                    // 1) Add passivo por segundo (recurso ou conhecimento)
                    if (trait.AddPerSecond != 0)
                    {
                        var resId = trait.ResourceId ?? trait.KnowledgeId;
                        if (!string.IsNullOrWhiteSpace(resId) && m.Resources.TryGetValue(resId, out var r))
                        {
                            r.PerSecond += trait.AddPerSecond; // usa o mesmo campo que contr./upgrades já somam
                        }
                    }

                    // 2) Multiplicador de ganho de um resource/knowledge específico
                    if (trait.GainMult != 1.0)
                    {
                        var resId = trait.ResourceId ?? trait.KnowledgeId;
                        if (!string.IsNullOrWhiteSpace(resId) && m.Resources.TryGetValue(resId, out var r))
                        {
                            r.PerSecond *= trait.GainMult;
                        }
                    }
                }
            }
        }
    }
}
