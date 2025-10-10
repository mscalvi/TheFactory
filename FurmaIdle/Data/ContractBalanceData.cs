// Data/ContractBalanceData.cs
namespace FurmaIdle.Data
{
    public sealed class ContractBalance
    {
        public required string ResourceId { get; init; }
        public required double CoinsPerCycle { get; init; }
        public required double SecondsPerCycle { get; init; }
        public required double Cost0 { get; init; }
        public required double Growth { get; init; }
    }

    public static class ContractBalanceData
    {
        // chave = Level do contrato
        public static readonly Dictionary<int, ContractBalance> ByLevel = new()
        {
            [1] = new() { ResourceId = "r001", Cost0 = 10, Growth = 1.12, CoinsPerCycle = 2, SecondsPerCycle = 2 },
            [2] = new() { ResourceId = "r001", Cost0 = 100, Growth = 1.13, CoinsPerCycle = 25, SecondsPerCycle = 10 },
            [3] = new() { ResourceId = "r001", Cost0 = 2000, Growth = 1.14, CoinsPerCycle = 150, SecondsPerCycle = 20 },
            [4] = new() { ResourceId = "r001", Cost0 = 50000, Growth = 1.19, CoinsPerCycle = 500, SecondsPerCycle = 40 },
            [5] = new() { ResourceId = "r001", Cost0 = 100000, Growth = 1.21, CoinsPerCycle = 2000, SecondsPerCycle = 90 },
            [6] = new() { ResourceId = "r001", Cost0 = 5000000, Growth = 1.23, CoinsPerCycle = 8000, SecondsPerCycle = 150 },
        };
    }
}
