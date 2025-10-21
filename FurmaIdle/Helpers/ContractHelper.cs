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
            return (bal.CoinId, bal.CoinsPerCycle, bal.SecondsPerCycle);
        }

        // Produção por segundo considerando quantidade atual no Stage
        public static double ProdPerSecond(ContractModel c, StageModel s)
        {
            s.ActiveContracts.TryGetValue(c.Id, out var Quant);
            var (_, cps, spc) = ProdParams(c);
            if (!(cps > 0) || !(spc > 0) || Quant <= 0) return 0;
            return (cps / spc) * Quant;
        }
        #endregion
    }
}
