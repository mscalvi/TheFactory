using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class TraitModel
    {
        public string Id { get; set; }
        public string Description { get; set; }

        // Effect
        public string TargetId? { get; set; }
        public double EffectValue { get; set; }
        public EffectHelper.EffectOperation EffectOperation { get; set; }
        public EffectHelper.EffectType EffectType { get; set; }
    }
}
