using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public interface IUpgradeService
    {
        void Recalculate(IEnumerable<UpgradeModel> purchased);
        double GainMult(string contractId);    
        double TimeMult(string contractId);    
        double GenAdd(string resourceId);
        int CapAdd(string teamIdOrTechId);
    }

    public sealed class UpgradeService : IUpgradeService
    {
        private readonly Dictionary<string, double> _gainMult = new();
        private readonly Dictionary<string, double> _timeMult = new();
        private readonly Dictionary<string, double> _genAdd = new();
        private readonly Dictionary<string, int> _capAdd = new();

        public void Recalculate(IEnumerable<UpgradeModel> purchased)
        {
            _gainMult.Clear(); _timeMult.Clear(); _genAdd.Clear(); _capAdd.Clear();

            // defaults
            double GetGain(string id) => _gainMult.TryGetValue(id, out var v) ? v : 1.0;
            double GetTime(string id) => _timeMult.TryGetValue(id, out var v) ? v : 1.0;

            foreach (var up in purchased)
                foreach (var ef in up.Effects)
                {
                    switch (ef.Target)
                    {
                        case EffectTarget.ContractGain:
                            _gainMult[ef.ScopeId] = Apply(GetGain(ef.ScopeId), ef);
                            break;
                        case EffectTarget.ContractTime:
                            _timeMult[ef.ScopeId] = Apply(GetTime(ef.ScopeId), ef);
                            break;
                        case EffectTarget.ResourceGen:
                            _genAdd[ef.ScopeId] = (_genAdd.TryGetValue(ef.ScopeId, out var g) ? g : 0) + ef.Value;
                            break;
                        case EffectTarget.ContractCap:
                            _capAdd[ef.ScopeId] = (_capAdd.TryGetValue(ef.ScopeId, out var c) ? c : 0) + (int)ef.Value;
                            break;
                    }
                }
        }

        public double GainMult(string contractId) => _gainMult.TryGetValue(contractId, out var m) ? m : 1.0;
        public double TimeMult(string contractId) => _timeMult.TryGetValue(contractId, out var m) ? m : 1.0;
        public double GenAdd(string resourceId) => _genAdd.TryGetValue(resourceId, out var a) ? a : 0.0;
        public int CapAdd(string scopeId) => _capAdd.TryGetValue(scopeId, out var a) ? a : 0;

        private static double Apply(double current, UpgradeEffectModel ef) => ef.Op switch
        {
            EffectOp.Multiplicative => current * ef.Value,     // 1.10, 0.90, etc.
            EffectOp.Additive => current + ef.Value,     // cuidado: aqui current parte de 1.0
            EffectOp.Override => ef.Value,
            _ => current
        };
    }

}
