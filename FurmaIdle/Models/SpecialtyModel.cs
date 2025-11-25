using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class SpecialtyModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Lore { get; set; }
        public string Image { get; set; }
        public string Icon { get; set; }
        public VersionHelper.UseState UseState { get; set; } = VersionHelper.UseState.InUse;

        // Effect
        public string TargetId { get; set; }
        public double EffectValue { get; set; }
        public EffectHelper.EffectOperation EffectOp { get; set; }
        public EffectHelper.EffectType EffectType { get; set; }
        public EffectHelper.EffectSupertype EffectSupertype { get; set; }

        public UnlockHelper.Persistence Persistence { get; set; }
        public double Duration { get; set; }

        // Custo
        public string PricingId { get; set; }
        public int Cost { get; set; }

        // Modifier
        public List<ModifierModel> Modifiers { get; set; }
    }
}
