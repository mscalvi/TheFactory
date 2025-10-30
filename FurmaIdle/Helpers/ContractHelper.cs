using FurmaIdle.Data;
using FurmaIdle.Models;
using FurmaIdle.Services;
using System.Diagnostics.Contracts;

namespace FurmaIdle.Helpers
{
    public class ContractHelper
    {
        #region Contract Identity
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
        public static (string CoinId, double CoinsPerCycle, double SecondsPerCycle) BaseProdParams(ContractModel c)
        {
            if (!GetContractBase(c, out var bal)) return ("", 0, 1);
            return (bal.CoinId, bal.CoinsPerCycle, bal.SecondsPerCycle);
        }
        public static (string CoinId, double CoinsPerCycle, double SecondsPerCycle) RealProdParams(ContractModel c)
        {
            if (!GetContractBase(c, out var bal)) return ("", 0, 1);

            var (_, cps, spc) = BaseProdParams(c);

            var gainMod = GetModifiers(c, EffectHelper.EffectType.ContractGain);
            var timeMod = GetModifiers(c, EffectHelper.EffectType.ContractTime);

            var coinsPerCycle = (cps + gainMod.AddMod) * gainMod.MultMod;
            var timePerCycle = (spc + timeMod.AddMod) * timeMod.MultMod;


            return (bal.CoinId, coinsPerCycle, timePerCycle);
        }

        // Produção efetiva por segundo (com qty e modificadores)
        public static double RealProdPerSecond(ContractModel contract, StageModel stage)
        {
            stage.ActiveContracts.TryGetValue(contract.Id, out var qty);
            if (qty <= 0) return 0;

            var (_, cps, spc) = BaseProdParams(contract);

            var gainMod = GetModifiers(contract, EffectHelper.EffectType.ContractGain);
            var timeMod = GetModifiers(contract, EffectHelper.EffectType.ContractTime);

            var coinsPerCycle = (cps + gainMod.AddMod) * gainMod.MultMod;
            var timePerCycle = (spc + timeMod.AddMod) * timeMod.MultMod;

            return (coinsPerCycle / timePerCycle) * qty;
        }

        // Helpers
        public static string CoinIdOf(ContractModel c)
        {
            var (coin, _, _) = BaseProdParams(c);
            return coin;
        }
        public static (double AddMod, double MultMod) GetModifiers(ContractModel contract, EffectHelper.EffectType type)
        {
            double AddMod = 0;
            double MultMod = 1;

            foreach (var modifier in contract.Modifiers) 
            {
                if (type == modifier.Type)
                {
                    if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                    {
                        AddMod += modifier.Value;
                    }
                    if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                    {
                        MultMod *= modifier.Value;
                    }
                }
            }

            return (AddMod, MultMod);
        }

        #endregion
    }
}
