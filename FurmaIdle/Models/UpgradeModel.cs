using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class UpgradeModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; } = "";
        public string Lore { get; set; }
        public string Description { get; set; }
        public string UnlockId { get; set; }
        public int ActualBuy { get; set; } = 0;
        public int MaxBuy { get; set; }
        public PricingHelper.PricingId PricingId { get; set; }
        public UnlockHelper.State State { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; }
        public VersionHelper.UseState UseState { get; set; } = VersionHelper.UseState.InUse;

        // Efeito
        public string TargetId { get; set; }
        public EffectHelper.EffectType EffectType { get; set; }
        public EffectHelper.EffectSupertype EffectSupertype { get; set; }
        public EffectHelper.EffectOperation EffectOp { get; set; }
        public double EffectValue { get; set; }

        // Modifier
        public List<ModifierModel> Modifiers { get; set; }

    }
}
