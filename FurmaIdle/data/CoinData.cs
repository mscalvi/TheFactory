using FurmaIdle.Models;
using FurmaIdle.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace FurmaIdle.Data
{
    public class CoinsData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> ShowOrder = new();

        internal static readonly Dictionary<string, CoinModel> All = new()
        {
            #region Initial Coins
            ["m01"] = new CoinModel
            {
                Id = "m01",
                Name = "Talhos",
                Image = "images/coins/m01.svg",
                Icon = "icons/coins/m01.svg",
                UnlockId = "s01",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
                Lore = "",
                Description = "",
            },
            ["m02"] = new CoinModel
            {
                Id = "m02",
                Name = "Estranhos",
                Image = "images/coins/m02.svg",
                Icon = "icons/coins/m02.svg",
                UnlockId = "s02",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
                Lore = "",
                Description = "",
            },
            #endregion
        };

        // --- Métodos Reutilizáveis do Padrão ---

        public static CoinModel GetDef(string id)
        {
            if (!All.TryGetValue(id, out var coin))
            {
                throw new KeyNotFoundException($"Coin with ID '{id}' not found.");
            }

            return new CoinModel
            {
                Id = coin.Id,
                Name = coin.Name,
                Image = coin.Image,
                Icon = coin.Icon,
                UnlockId = coin.UnlockId,
                State = coin.State,
                Persistence = coin.Persistence,
                Modifiers = coin.Modifiers,
                Lore = coin.Lore,
                Description = coin.Description,
                UseState = coin.UseState,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = All?.Keys.AsEnumerable() ?? Enumerable.Empty<string>();
            ShowOrder.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        public static Dictionary<string, CoinModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, CoinModel>(All.Count);
            if (ShowOrder.Count == 0) PopulateOrder();
            foreach (var id in ShowOrder)
            {
                if (!All.TryGetValue(id, out var coin)) continue;
                dict[id] = GetDef(id);
            }
            return dict;
        }
    }
}