using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class ModifierModel
    {
        public string ApplyerId { get; set; }
        public EffectHelper.EffectType State { get; set; }
        public EffectHelper.EffectScope Scope { get; set; }
        public EffectHelper.EffectOperation Operation { get; set; }
        public EffectHelper.EffectTarget Target { get; set; }
        public double Value { get; set; }
    }
}
