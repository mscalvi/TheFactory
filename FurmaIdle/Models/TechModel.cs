using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class TechModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public string Lore { get; set; }
        public string UnlockId { get; set; }
        public int Level { get; set; }
        public PricingHelper.PricingId PricingId { get; set; }
        public UnlockHelper.State State { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; }
        public VersionHelper.UseState UseState { get; set; } = VersionHelper.UseState.InUse;

        // Modifier
        public List<ModifierModel> Modifiers { get; set; }
    }
}
