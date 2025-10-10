using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using static FurmaIdle.Models.CharacterModel;

namespace FurmaIdle.Helpers
{
    // Helpers/ContractsHelper.cs
    public static class ContractsHelper
    {
        public sealed record ContractButton(int Level, string Label);

        public static IEnumerable<CharacterModel> GetActiveCharacters(GameModel current, string stageId)
            => current?.Characters?.Values?
                   .Where(c => c.CharState == CharStateEnum.CharState.OnStage
                            && c.CharStageId == stageId)
               ?? Enumerable.Empty<CharacterModel>();

        public static IReadOnlyList<int> GetKnownLevelsFor(CharacterModel ch)
        {
            IEnumerable<string> ids =
                (ch?.KnowContractsIds as IEnumerable<string>) ?? Array.Empty<string>();

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

        public static IReadOnlyList<int> GetContractLevelsForStage(IGameService game, string stageId)
        {
            if (game is null || string.IsNullOrWhiteSpace(stageId))
                return Array.Empty<int>();

            IEnumerable<CharacterModel> allChars =
                (game.Current?.Characters != null)
                    ? game.Current.Characters.Values
                    : Enumerable.Empty<CharacterModel>();

            var activeChars = allChars
                .Where(c => c.CharState == CharStateEnum.CharState.OnStage &&
                            string.Equals(c.CharStageId, stageId, StringComparison.Ordinal));

            var levels = new SortedSet<int>();

            foreach (var ch in activeChars)
            {
                // SOMENTE conhecidos:
                IEnumerable<string> known = ch.KnowContractsIds ?? Enumerable.Empty<string>();

                foreach (var id in known)
                {
                    if (string.IsNullOrWhiteSpace(id)) continue;
                    if (!ContractData.All.TryGetValue(id, out var def)) continue;

                    levels.Add(def.Level);
                }
            }

            return levels.ToList();
        }

        public static IReadOnlyList<ContractModel> GetContractsForLevel(IGameService game, string stageId, int level)
        {
            if (game is null || string.IsNullOrWhiteSpace(stageId))
                return Array.Empty<ContractModel>();

            var active = (game.Current?.Characters?.Values ?? Enumerable.Empty<CharacterModel>())
                .Where(c => c.CharState == CharStateEnum.CharState.OnStage
                         && string.Equals(c.CharStageId, stageId, StringComparison.Ordinal));

            var ids = new HashSet<string>(StringComparer.Ordinal);

            foreach (var ch in active)
            {
                foreach (var id in ch.KnowContractsIds ?? Enumerable.Empty<string>())
                {
                    if (ContractData.All.TryGetValue(id, out var def) && def.Level == level)
                        ids.Add(id);
                }
            }

            return ids
                .Select(id => ContractData.All[id])
                .OrderBy(c => c.Id, StringComparer.Ordinal)
                .ToList();
        }

    }

}
