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
        public static bool GetContractBase(ContractModel contract, out ContractHelper value)
            => ByLevel.TryGetValue(contract.Level, out value);

        public static double GetContractNextPrice(string contractId, string stageId)
        {
            ContractModel Contract = LocateHelper.LocateContract(contractId);
            ExpeditionModel Expedition = LocateHelper.LocateExpedition(stageId);
            var Quantity = 0;
            double Price = 0;

            if (!GetContractBase(Contract, out var value)) return double.PositiveInfinity;

            Expedition.ContractsActiveId.TryGetValue(contractId, out Quantity);

            Price = value.BaseCost * Math.Pow(value.Growth, Quantity);
                        
            return Math.Ceiling(Price);
        }

        public static (string, double, double) GetContractProduction(string contractId, string stageId)
        {
            ContractModel Contract = LocateHelper.LocateContract(contractId);
            ExpeditionModel Expedition = LocateHelper.LocateExpedition(stageId);
            var Quantity = 0;
            double Production = 0;
            double CoinsPerCycle;
            double CoinsPerSecond;
            string CoinId;

            GetContractBase(Contract, out var ammount);
            CoinId = ammount.ResourceId;

            Expedition.ContractsActiveId.TryGetValue(contractId, out Quantity);


            if (Quantity <= 0)
            {
                return (CoinId, 0.0, 0.0);
            }

            CoinsPerCycle = ammount.CoinsPerCycle * Quantity;

            CoinsPerSecond = ammount.CoinsPerCycle / ammount.SecondsPerCycle;
            CoinsPerSecond *= Quantity;

            return (CoinId, CoinsPerCycle, CoinsPerSecond);
        }

        // Montagem UI
        public sealed class ContractButton
        {
            public int Level { get; init; }
            public List<ContractModel> Items { get; init; } = new();
        }

        public static List<ContractButton> BuildButtons(string StageId)
        {
            var result = new List<ContractButton>();

            ExpeditionModel Expedition = LocateHelper.LocateExpedition(StageId);
            StageModel Stage = LocateHelper.LocateStage(StageId);
            List<CharacterModel> ActiveCharacters = new List<CharacterModel>();

            ActiveCharacters = ExpeditionHelper.GetActiveCharacters(Expedition);
            var contractIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (var Character in ActiveCharacters)
            {
                foreach (var ContractId in Character.ContractsIds)
                    if (!string.IsNullOrWhiteSpace(ContractId))
                        contractIds.Add(ContractId);
            }

            var byLevel = new Dictionary<int, List<ContractModel>>();
            foreach (var ContractId in contractIds)
            {
                ContractModel? Contract = null;

                Contract = ContractData.GetDef(ContractId); 

                if (Contract.Level > Stage.MaxContractLevel) continue;
                if (Contract.State != UnlockHelper.State.Unlocked) continue;

                if (!byLevel.TryGetValue(Contract.Level, out var list))
                    byLevel[Contract.Level] = list = new List<ContractModel>();

                list.Add(Contract);
            }

            for (int lvl = 1; lvl <= Stage.MaxContractLevel; lvl++)
            {
                byLevel.TryGetValue(lvl, out var list);
                result.Add(new ContractButton
                {
                    Level = lvl,
                    Items = (list ?? new List<ContractModel>())
                        .OrderBy(c => c.Name ?? c.Id, StringComparer.Ordinal)
                        .ToList()
                });

            }

            return result;
        }
    }
}

