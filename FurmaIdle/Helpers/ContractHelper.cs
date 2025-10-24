using FurmaIdle.Data;
using FurmaIdle.Models;
using FurmaIdle.Services;

namespace FurmaIdle.Helpers
{
    public class ContractHelper
    {
        #region Contract Generation
        public required string CoinId { get; init; }
        public required double CoinsPerCycle { get; init; }
        public required double SecondsPerCycle { get; init; }

        public static readonly Dictionary<int, ContractHelper> ByLevel = new()
        {
            [1] = new() { CoinId = "m01", CoinsPerCycle = 2, SecondsPerCycle = 2 },
            [2] = new() { CoinId = "m01", CoinsPerCycle = 25, SecondsPerCycle = 10 },
            [3] = new() { CoinId = "m01", CoinsPerCycle = 150, SecondsPerCycle = 20 },
            [4] = new() { CoinId = "m01", CoinsPerCycle = 500, SecondsPerCycle = 40 },
            [5] = new() { CoinId = "m01", CoinsPerCycle = 2000, SecondsPerCycle = 90 },
            [6] = new() { CoinId = "m01", CoinsPerCycle = 8000, SecondsPerCycle = 150 },
        };
        public static bool GetContractBase(ContractModel contract, out ContractHelper value)
            => ByLevel.TryGetValue(contract.Level, out value);
        #endregion

        #region Contract Generation
        public static (string CoinId, double CoinsPerCycle, double SecondsPerCycle) ProdParams(ContractModel c)
        {
            if (!GetContractBase(c, out var bal)) return ("", 0, 1);
            return (bal.CoinId, bal.CoinsPerCycle, bal.SecondsPerCycle * c.TimeFactor);
        }

        // Produção por segundo considerando quantidade atual no Stage
        public static string CoinIdOf(ContractModel c)
        {
            var (coin, _, _) = ProdParams(c);
            return coin;
        }

        // Produção efetiva por segundo (com qty e modificadores)
        public static double ProdPerSecond(ContractModel contract, StageModel stage)
        {
            stage.ActiveContracts.TryGetValue(contract.Id, out var qty);
            if (qty <= 0) return 0;

            var (_, cps, spc) = ProdParams(contract);
            if (!(cps > 0) || !(spc > 0)) return 0;

            var perCycle = (cps + contract.AddMod) * contract.MultMod * contract.GainFactor;

            return (perCycle / spc) * qty;
        }
        #endregion
    }
}
