using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class ContractModel
    {
        // Basics
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public string Lore { get; set; } = "";
        public PricingHelper.PricingId PricingId { get; set; }
        public VersionHelper.UseState UseState { get; set; } = VersionHelper.UseState.InUse;

        // Stats
        public string UnlockId { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; }
        public UnlockHelper.State State { get; set; }
        public UnlockHelper.ContractState GameUseState { get; set; }

        // Modifiers
        public List<ModifierModel> Modifiers { get; set; }

        // Info
        public int Level { get; set; }
        public string CoinId { get; set; }
        public string? KnowledgeFactor1 { get; set; } = "";
        public string? KnowledgeFactor2 { get; set; } = "";

    }
}
