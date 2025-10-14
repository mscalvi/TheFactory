using FurmaIdle.Data;
using FurmaIdle.Models;
using System.Diagnostics.Contracts;

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


        // Cálculos
        public static bool GetContractBase(ContractModel contact, out ContractHelper value)
            => ByLevel.TryGetValue(contact.Level, out value);

        public static double GetContractNextPrice(string contractId, int expeditionId)
        {
            ContractModel Contract = ContractData.LocateContract(contractId);
            ExpeditionModel Expedition = StageData.LocateExpedition(expeditionId);
            var Quantity = 0;
            double Price = 0;

            if (!GetContractBase(Contract, out var value)) return double.PositiveInfinity;

            Expedition.ContractsActiveId.TryGetValue(contractId, out Quantity);

            Price = value.BaseCost * Math.Pow(value.Growth, Quantity);
                        
            return Math.Ceiling(Price);
        }

        public static (string CoinId, double CoinsPerSeconde, double CoinsPerCycle) GetContractProduction(string contractId, int expeditionId)
        {
            ContractModel Contract = ContractData.LocateContract(contractId);
            ExpeditionModel Expedition = StageData.LocateExpedition(expeditionId);
            var Quantity = 0;
            double Production = 0;

            if (!GetContractBase(Contract, out var value)) return double.PositiveInfinity;
            Expedition.ContractsActiveId.TryGetValue(contractId, out Quantity);



            var (_, cps, spc) = ProdParams(Contract);
            if (!(cps > 0) || !(spc > 0) || c.Quant <= 0) return 0;
            return (cps / spc) * c.Quant;

            return Production;
        }

        public static (string resId, double cps, double spc) ProdParams(ContractModel c)
        {
            if (!TryGetBalance(c, out var bal)) return ("", 0, 1);
            return (bal.ResourceId, bal.CoinsPerCycle, bal.SecondsPerCycle);
        }
    }
}

