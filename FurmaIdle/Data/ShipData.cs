using FurmaIdle.Helpers;
using FurmaIdle.Models;

namespace FurmaIdle.Data
{
    public class ShipData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> ShowOrder = new();

        internal static readonly Dictionary<string, ShipModel> All = new()
        {
            #region s01
            ["n01"] = new ShipModel
            {
                Id = "n01",
                Name = "Barco 01",
                Description = "",
                Lore = "",
                Icon = "icons/ships/n01.svg",
                Image = "images/ships/n01.svg",
                UnlockId = "un01",
                State = UnlockHelper.State.Blocked,
                ShipState = UnlockHelper.ShipState.Blocked,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion
        };

        public static ShipModel GetDef(string id)
        {
            var ship = All[id];
            return new ShipModel
            {
                Id = ship.Id,
                Name = ship.Name,
                Description = ship.Description,
                Lore = ship.Lore,
                Icon = ship.Icon,
                Image = ship.Image,
                UnlockId = ship.UnlockId,
                State = ship.State,
                ShipState = ship.ShipState,
                InStageId = ship.InStageId,
                Persistence = ship.Persistence,
                Modifiers = ship.Modifiers,
                UseState = ship.UseState,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = (All == null)
                ? Enumerable.Empty<string>()
                : All.Keys.AsEnumerable();

            ShowOrder.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        public static Dictionary<string, ShipModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, ShipModel>(All.Count);

            if (ShowOrder.Count == 0) PopulateOrder();

            foreach (var id in ShowOrder)
            {
                if (!All.TryGetValue(id, out var ship)) continue;

                dict[id] = new ShipModel
                {
                    Id = ship.Id,
                    Name = ship.Name,
                    Description = ship.Description,
                    Lore = ship.Lore,
                    Icon = ship.Icon,
                    Image = ship.Image,
                    UnlockId = ship.UnlockId,
                    State = ship.State,
                    ShipState = ship.ShipState,
                    InStageId = ship.InStageId,
                    Persistence = ship.Persistence,
                    Modifiers = ship.Modifiers,
                    UseState = ship.UseState,
                };
            }
            return dict;
        }
    }
}
