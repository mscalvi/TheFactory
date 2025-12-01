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
            [0] = new() {CoinsPerCycle = 1, SecondsPerCycle = 5 },
            [1] = new() {CoinsPerCycle = 2, SecondsPerCycle = 3 },
            [2] = new() {CoinsPerCycle = 50, SecondsPerCycle = 10 },
            [3] = new() {CoinsPerCycle = 300, SecondsPerCycle = 20 },
            [4] = new() {CoinsPerCycle = 4000, SecondsPerCycle = 40 },
            [5] = new() {CoinsPerCycle = 20000, SecondsPerCycle = 90 },
            [6] = new() {CoinsPerCycle = 180000, SecondsPerCycle = 150 },
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

        [Flags]
        public enum Context
        {
            None = 0,
            Walled = 1 << 0, 
            UrbanSmall = 1 << 1, 
            UrbanLarge = 1 << 2,
            WarZone = 1 << 3,
            Wild = 1 << 4,
            Infected = 1 << 5,
            Port = 1 << 6,
        }
    }
}
