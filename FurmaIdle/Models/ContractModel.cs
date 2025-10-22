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
        public PricingHelper.PricingId PricingId { get; set; }

        // Stats
        public string? UnlockId { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; }
        public UnlockHelper.State State { get; set; }
        public double AddMod { get; set; }
        public double MultMod { get; set; }

        // Factor
        public double GainFactor { get; set; }
        public double TimeFactor { get; set; }
        public double PriceFactor { get; set; }

        // Info
        public int Level { get; set; }
        public string? KnowledgeFactor1 { get; set; }
        public string? KnowledgeFactor2 { get; set; }
        public string? KnowledgeFactor3 { get; set; }

    }
}
