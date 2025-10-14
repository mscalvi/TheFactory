using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class TraitModel
    {
        public string Id { get; set; }
        public string Description { get; set; }

        // Effect
        public string TargetId { get; set; }
        public string EffectValue { get; set; }
        public EnumHelper.EffectOperation EffectOperation { get; set; }
        public EnumHelper.EffectType EffectType { get; set; }
    }
}
