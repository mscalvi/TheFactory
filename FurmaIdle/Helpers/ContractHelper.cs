using FurmaIdle.Models;
using FurmaIdle.Services;

namespace FurmaIdle.Helpers
{
    public class ContractHelper
    {
        public required string ResourceId { get; init; }
        public required double CoinsPerCycle { get; init; }
        public required double SecondsPerCycle { get; init; }
        public required double BaseCost { get; init; }
        public required double Growth { get; init; }

        public static readonly Dictionary<int, ContractHelper> ByLevel = new()
        {
            [1] = new() { ResourceId = "m01", BaseCost = 10, Growth = 1.12, CoinsPerCycle = 2, SecondsPerCycle = 2 },
            [2] = new() { ResourceId = "m01", BaseCost = 100, Growth = 1.13, CoinsPerCycle = 25, SecondsPerCycle = 10 },
            [3] = new() { ResourceId = "m01", BaseCost = 2000, Growth = 1.14, CoinsPerCycle = 150, SecondsPerCycle = 20 },
            [4] = new() { ResourceId = "m01", BaseCost = 50000, Growth = 1.19, CoinsPerCycle = 500, SecondsPerCycle = 40 },
            [5] = new() { ResourceId = "m01", BaseCost = 100000, Growth = 1.21, CoinsPerCycle = 2000, SecondsPerCycle = 90 },
            [6] = new() { ResourceId = "m01", BaseCost = 5000000, Growth = 1.23, CoinsPerCycle = 8000, SecondsPerCycle = 150 },
        };
        public static bool GetContractBase(ContractModel contract, out ContractHelper value)
            => ByLevel.TryGetValue(contract.Level, out value);
    }
}
