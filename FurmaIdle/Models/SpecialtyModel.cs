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
        public string? TargetId { get; set; }
        public double? EffectValue { get; set; }
        public EffectHelper.EffectOperation? EffectOperation { get; set; }
        public EffectHelper.EffectType? EffectType { get; set; }
        public double? Duration { get; set; }

        // Custo
        public string PricingId { get; set; }
        public int Cost { get; set; }
    }
}
