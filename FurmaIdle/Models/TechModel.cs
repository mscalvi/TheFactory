using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class TechModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public string UnlockId { get; set; }
        public int Level { get; set; }
        public string PricingId { get; set; }
        public EnumHelper.State State { get; set; }
        public EnumHelper.Persistence Persistence { get; set; }
    }
}
