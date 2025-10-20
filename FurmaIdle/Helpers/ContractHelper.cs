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
        public required long BaseCost { get; init; }
        public required double Growth { get; init; }

        public static readonly Dictionary<int, ContractHelper> ByLevel = new()
        {
            [1] = new() { CoinId = "m01", BaseCost = 10, Growth = 1.12, CoinsPerCycle = 2, SecondsPerCycle = 2 },
            [2] = new() { CoinId = "m01", BaseCost = 100, Growth = 1.13, CoinsPerCycle = 25, SecondsPerCycle = 10 },
            [3] = new() { CoinId = "m01", BaseCost = 2000, Growth = 1.14, CoinsPerCycle = 150, SecondsPerCycle = 20 },
            [4] = new() { CoinId = "m01", BaseCost = 50000, Growth = 1.19, CoinsPerCycle = 500, SecondsPerCycle = 40 },
            [5] = new() { CoinId = "m01", BaseCost = 100000, Growth = 1.21, CoinsPerCycle = 2000, SecondsPerCycle = 90 },
            [6] = new() { CoinId = "m01", BaseCost = 5000000, Growth = 1.23, CoinsPerCycle = 8000, SecondsPerCycle = 150 },
        };
        public static bool GetContractBase(ContractModel contract, out ContractHelper value)
            => ByLevel.TryGetValue(contract.Level, out value);
        #endregion

        #region Contract Pricing
        public static long NextPrice(ContractModel c, StageModel s)
        {
            s.ActiveContracts.TryGetValue(c.Id, out var Quant);
            if (!GetContractBase(c, out var bal)) return long.MaxValue;
            var price = bal.BaseCost * Math.Pow(bal.Growth, Quant);
            return (long)Math.Ceiling(price);
        }
        public static (string resId, double cps, double spc) ProdParams(ContractModel c)
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

        #region Contract Construction
        public sealed record ContractButton(int Level, string Label);
        public static IEnumerable<CharacterModel> GetActiveCharacters(GameModel current, string stageId)
            => current?.Characters?.Values?
                   .Where(c => c.CharState == UnlockHelper.CharState.OnStage
                            && c.InStageId == stageId)
               ?? Enumerable.Empty<CharacterModel>();

        public static IReadOnlyList<int> GetKnownLevelsFor(CharacterModel ch)
        {
            IEnumerable<string> ids =
                (ch?.ContractsIds as IEnumerable<string>) ?? Array.Empty<string>();

            var levels = ids
                .Select(id => ContractData.All.TryGetValue(id, out var def) ? def.Level : (int?)null)
                .Where(l => l.HasValue)
                .Select(l => l!.Value)
                .Distinct()
                .OrderBy(l => l)
                .ToList();

            return levels;
        }

        public static List<ContractButton> BuildButtons(GameModel current, string stageId, int maxLevels = 3)
        {
            var levels =
                GetActiveCharacters(current, stageId)
                    .SelectMany(GetKnownLevelsFor)
                    .Where(l => l >= 1 && l <= maxLevels)
                    .Distinct()
                    .OrderBy(l => l)
                    .Take(maxLevels)
                    .ToList();

            return levels
               .Select(l => new ContractButton(l, $"Contrato Nível {l}"))
               .ToList();
        }

        public static IReadOnlyList<int> GetContractLevelsForStage(ICurrentGameService game, string stageId)
        {
            if (game is null || string.IsNullOrWhiteSpace(stageId))
                return Array.Empty<int>();

            IEnumerable<CharacterModel> allChars =
                (game.CurrentGame?.Characters != null)
                    ? game.CurrentGame.Characters.Values
                    : Enumerable.Empty<CharacterModel>();

            var activeChars = allChars
                .Where(c => c.CharState == UnlockHelper.CharState.OnStage &&
                            string.Equals(c.InStageId, stageId, StringComparison.Ordinal));

            var levels = new SortedSet<int>();

            foreach (var ch in activeChars)
            {
                IEnumerable<string> known = ch.ContractsIds ?? Enumerable.Empty<string>();

                foreach (var id in known)
                {
                    if (string.IsNullOrWhiteSpace(id)) continue;
                    if (!ContractData.All.TryGetValue(id, out var def)) continue;

                    game.CurrentGame.Contracts.TryGetValue(id, out var contract);

                    if (contract == null) continue;
                    if (contract.State != UnlockHelper.State.Available) continue;

                    levels.Add(def.Level);
                }
            }

            return levels.ToList();
        }

        public static IReadOnlyList<ContractModel> GetContractsForLevel(
            ICurrentGameService game, string stageId, int level)
        {
            if (game is null || string.IsNullOrWhiteSpace(stageId))
                return Array.Empty<ContractModel>();

            var active = (game.CurrentGame?.Characters?.Values ?? Enumerable.Empty<CharacterModel>())
                .Where(c => c.CharState == UnlockHelper.CharState.OnStage
                         && string.Equals(c.InStageId, stageId, StringComparison.Ordinal));

            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (var ch in active)
            {
                foreach (var id in ch.ContractsIds ?? Enumerable.Empty<string>())
                {
                    if (ContractData.All.TryGetValue(id, out var def) && def.Level == level)
                        ids.Add(id);
                }
            }

            var dict = game.CurrentGame?.Contracts;
            return ids
                .Select(id =>
                {
                    if (dict != null && dict.TryGetValue(id, out var model) && model is not null)
                        return model;
                    return ContractData.GetDef(id);
                })
                .OrderBy(c => c.Id, StringComparer.Ordinal)
                .ToList();
        }

        #endregion

    }
}
