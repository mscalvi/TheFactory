using FurmaIdle.Helpers;
using FurmaIdle.Models;

namespace FurmaIdle.Data
{
    public class RouteData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> ShowOrder = new();

        internal static readonly Dictionary<string, RouteModel> All = new()
        {
            #region Ilha de Vera (s01)
            ["z0102"] = new RouteModel
            {
                Id = "z0102",
                Name = "Ilha de Vera - Ilha Maravasta",
                Description = "",
                Lore = "",
                Icon = "icons/routes/z0102.svg",
                Image = "",
                UnlockId = "uz0102",
                State = UnlockHelper.State.Blocked,
                RouteState = UnlockHelper.RouteState.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
                PointA = "s01",
                PointB = "s02",
                Distance = 3000,
            },
            #endregion
        };

        // --- Métodos Reutilizáveis do Padrão ---

        public static RouteModel GetDef(string id)
        {
            if (!All.TryGetValue(id, out var route))
            {
                throw new KeyNotFoundException($"Route with ID '{id}' not found.");
            }

            // Retorna uma nova instância (cópia) para não modificar a definição estática
            return new RouteModel
            {
                Id = route.Id,
                Name = route.Name,
                ItemType = route.ItemType,
                Description = route.Description,
                Lore = route.Lore,
                Icon = route.Icon,
                Image = route.Image,
                UnlockId = route.UnlockId,
                State = route.State,
                RouteState = route.RouteState,
                Persistence = route.Persistence,
                Modifiers = route.Modifiers,
                PointA = route.PointA,
                PointB = route.PointB,
                Distance = route.Distance,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = All?.Keys.AsEnumerable() ?? Enumerable.Empty<string>();

            // Ordena usando StringComparer.Ordinal (s01, s01, etc.)
            ShowOrder.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        public static Dictionary<string, RouteModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, RouteModel>(All.Count);

            if (ShowOrder.Count == 0) PopulateOrder();

            foreach (var id in ShowOrder)
            {
                if (!All.TryGetValue(id, out var route)) continue;

                dict[id] = GetDef(id);
            }
            return dict;
        }
    }
}
