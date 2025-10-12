namespace FurmaIdle.Models
{
    public enum EffectOp { Additive, Multiplicative, Override }
    public enum EffectTarget { ContractGain, ContractTime, ResourceGen, ContractCap, ClicksGain, ResourceUnlock, ResourceCapPerChar, PartyCap }

    public sealed class UpgradeEffectModel
    {
        public EffectTarget Target { get; init; }
        public string ScopeId { get; init; } = "";        
        public double Value { get; init; }
        public EffectOp Op { get; init; } = EffectOp.Multiplicative;
    }

}
