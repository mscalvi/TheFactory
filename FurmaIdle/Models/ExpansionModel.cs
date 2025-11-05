using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class ExpansionModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public PricingHelper.PricingId PricingId { get; set; }
        public string? UnlockId { get; set; }
        public int Level { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; }
        public UnlockHelper.State State { get; set; }
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? FinishedAt { get; set; }
        
        public List<string> inUseContracts { get; set; } = new List<string>();
        public StatsModel ExpansionStats { get; set; } = new();
        public string NextExpansion { get; set; }

        // Modifiers
        public List<ModifierModel> Modifiers { get; set; }
    }
}
