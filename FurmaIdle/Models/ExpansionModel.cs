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
    }
}
