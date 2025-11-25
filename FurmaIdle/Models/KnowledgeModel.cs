using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class KnowledgeModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Lore { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public VersionHelper.UseState UseState { get; set; } = VersionHelper.UseState.InUse;

        // Status
        public string? UnlockId { get; set; }
        public UnlockHelper.State State { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; }


        // Gain
        public string GainCoinId { get; set; }
        public int GainCoinBase { get; set; }
        public double GainCoinCurve { get; set; }

        // Boost
        public double GenerationFactor { get; set; }
        public double GenerationPenaltie { get; set; }
        public IncomeHelper.CoinBurst GenerationCoin { get; set;}

        // Modifier
        public List<ModifierModel> Modifiers { get; set; }
    }
}
