using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class KnowledgeModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }

        // Status
        public string? UnlockId { get; set; }
        public UnlockHelper.State State { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; }

        // Gain
        public string GainCoinId { get; set; }
        public int GainCoinBase { get; set; }
        public double GainCoinCurve { get; set; }
        public double GainFactorCurve { get; set; }

        // Boost
        public double GenerationFactor { get; set; }
        public double GenerationPenaltie { get; set; }
    }
}
