using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Diagnostics.Contracts;

namespace FurmaIdle.Services
{
    public interface IContractService
    {
        double GetContractNextPrice(string contractId, string stageId);
        (string coinId, double perCycle, double perSecond) GetContractProduction(string contractId, string stageId);
        List<ContractService.ContractButton> BuildButtons(string stageId);
    }
    public sealed class ContractService : IContractService
    {
        private readonly ILocateService _locate;
        private readonly ICurrentGameService _game;
        private readonly IExpeditionService _expedition;

        public ContractService(ILocateService locate, ICurrentGameService game, IExpeditionService expedition)
        {
            _locate = locate;
            _game = game;
            _expedition = expedition;
        }

        // Cálculos

        public double GetContractNextPrice(string contractId, string stageId)
        {
            ContractModel Contract = _locate.LocateContract(contractId);
            ExpeditionModel Expedition = _locate.LocateExpedition(stageId);
            var Quantity = 0;
            double Price = 0;

            if (!ContractHelper.GetContractBase(Contract, out var value)) return double.PositiveInfinity;

            Expedition.ContractsActiveId.TryGetValue(contractId, out Quantity);

            Price = value.BaseCost * Math.Pow(value.Growth, Quantity);
                        
            return Math.Ceiling(Price);
        }

        public (string, double, double) GetContractProduction(string contractId, string stageId)
        {
            ContractModel Contract = _locate.LocateContract(contractId);
            ExpeditionModel Expedition = _locate.LocateExpedition(stageId);
            var Quantity = 0;
            double Production = 0;
            double CoinsPerCycle;
            double CoinsPerSecond;
            string CoinId;

            ContractHelper.GetContractBase(Contract, out var ammount);
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

        public List<ContractButton> BuildButtons(string StageId)
        {
            var result = new List<ContractButton>();

            StageModel Stage = _locate.LocateStage(StageId);
            ExpeditionModel Expedition = _locate.LocateExpedition(Stage.ActiveExpedition.Id);
            List<CharacterModel> ActiveCharacters = _expedition.GetActiveCharacters(Expedition);

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

