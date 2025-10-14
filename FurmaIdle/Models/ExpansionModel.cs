using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class ExpansionModel
    {
        public string Id { get; set; }
        public PricingHelper.PricingId PricingId { get; set; }
        public string UnlockId { get; set; }
        public int Level { get; set; }
    }
}
