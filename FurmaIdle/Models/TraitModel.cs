using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class TraitModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public VersionHelper.UseState UseState { get; set; } = VersionHelper.UseState.InUse;

        // Effect
        public string? TargetId { get; set; }
        public double EffectValue { get; set; }
        public EffectHelper.EffectOperation EffectOp { get; set; }
        public EffectHelper.EffectType EffectType { get; set; }
        public EffectHelper.EffectSupertype EffectSupertype { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; } = UnlockHelper.Persistence.untilExpedition;



        // Modifier
        public List<ModifierModel> Modifiers { get; set; }
    }
}
