using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class StageModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public List<string> Images { get; set; }
        public string Description { get; set; }
        public string? UnlockId { get; set; }
        public int? Level { get; set; }

        public Dictionary<string, int>? ActiveContracts { get; set; } = new(StringComparer.Ordinal);
        public List<int> lockedContracts { get; set; } = new();
        public Dictionary<string, double> ActiveContractsProgress { get; set; } = new Dictionary<string, double>();
        public ExpeditionModel Expedition { get; set; } = new ExpeditionModel();

        public PricingHelper.PricingId? PricingId { get; set; }
        public UnlockHelper.State State { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; }

        public int PartySizeStart { get; set; }
        public int PartySizeActual { get; set; }
        public int PartySizeMax { get; set; }

        public int StartContractLevel { get; set; }
        public int ActualContractLevel { get; set; }
        public int MaxContractLevel { get; set; }

        public string CoinId { get; set; }
        public string ClickId { get; set; }

        // Modifier
        public List<ModifierModel> Modifiers { get; set; }
    }
}
