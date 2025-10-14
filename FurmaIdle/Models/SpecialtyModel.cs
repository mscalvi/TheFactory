using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class SpecialtyModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        // Effect
        public string TargetId { get; set; }
        public string EffectValue { get; set; }
        public EnumHelper.EffectOperation EffectOperation { get; set; }
        public EnumHelper.EffectType EffectType { get; set; }
        public int Duration { get; set; }

        // Custo
        public string CostId { get; set; }
        public int CostValue { get; set; }
    }
}
