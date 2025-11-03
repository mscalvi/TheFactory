using FurmaIdle.Data;
using FurmaIdle.Models;
using FurmaIdle.Services;
using System.Diagnostics.Contracts;

namespace FurmaIdle.Helpers
{
    public class ContractHelper
    {
        public required double CoinsPerCycle { get; init; }
        public required double SecondsPerCycle { get; init; }

        public static readonly Dictionary<int, ContractHelper> m01ByLevel = new()
        {
            [1] = new() {CoinsPerCycle = 2, SecondsPerCycle = 2 },
            [2] = new() {CoinsPerCycle = 25, SecondsPerCycle = 10 },
            [3] = new() {CoinsPerCycle = 150, SecondsPerCycle = 20 },
            [4] = new() {CoinsPerCycle = 500, SecondsPerCycle = 40 },
            [5] = new() {CoinsPerCycle = 2000, SecondsPerCycle = 90 },
            [6] = new() {CoinsPerCycle = 8000, SecondsPerCycle = 150 },
        };

        public static (double CoinsPerCycle, double SecondsPerCycle) GetContractBase(ContractModel contract)
        {
            switch (contract.CoinId)
            {
                case "m01":
                    m01ByLevel.TryGetValue(contract.Level, out var contractInfo);
                    return (contractInfo.CoinsPerCycle, contractInfo.SecondsPerCycle);
            }

            return (0, 0);
        }
    }
}
