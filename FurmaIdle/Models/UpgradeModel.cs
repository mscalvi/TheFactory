using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class UpgradeModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public string Icon { get; set; }
        public string Lore { get; set; }
        public string Description { get; set; }
        public string UnlockId { get; set; }
        public int ActualBuy { get; set; } = 0;
        public int MaxBuy { get; set; }
        public PricingHelper.PricingId PricingId { get; set; }
        public UnlockHelper.State State { get; set; }
        public UnlockHelper.Persistence StatePersistence { get; set; }

        // Efeito
        public string TargetId { get; set; }
        public EffectHelper.EffectType EffectType { get; set; }
        public EffectHelper.EffectOperation EffectOp { get; set; }
        public double EffectValue { get; set; }

    }
}
