using FurmaIdle.Models; // Assumindo que ResourceModel está aqui
using FurmaIdle.Helpers; // Assumindo que UnlockHelper está aqui
using System.Collections.Generic;
using System.Linq;

namespace FurmaIdle.Data
{
    public class ResourceData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> ShowOrder = new();

        internal static readonly Dictionary<string, ResourceModel> All = new()
        {
            #region Initial Resources
            ["r01"] = new ResourceModel
            {
                Id = "r01",
                Name = "Mantimentos",
                UnlockId = "ux104",
                RsPerChar = 30,
                RsPerSecond = 0.0,
                Icon = "icons/resources/r01.svg",
                Image = "images/resources/r01.svg",
                Lore = "",
                Description = "",
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion
        };

        // --- Métodos Reutilizáveis do Padrão ---

        public static ResourceModel GetDef(string id)
        {
            if (!All.TryGetValue(id, out var resource))
            {
                throw new KeyNotFoundException($"Resource with ID '{id}' not found.");
            }

            return new ResourceModel
            {
                Id = resource.Id,
                Name = resource.Name,
                UnlockId = resource.UnlockId,
                RsPerChar = resource.RsPerChar,
                RsPerSecond = resource.RsPerSecond,
                Icon = resource.Icon,
                Image = resource.Image,
                Lore = resource.Lore,
                Description = resource.Description,
                Persistence = resource.Persistence,
                State = resource.State,
                Modifiers= resource.Modifiers,
                UseState = resource.UseState,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = All?.Keys.AsEnumerable() ?? Enumerable.Empty<string>();
            ShowOrder.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        public static Dictionary<string, ResourceModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, ResourceModel>(All.Count);
            if (ShowOrder.Count == 0) PopulateOrder();
            foreach (var id in ShowOrder)
            {
                if (!All.TryGetValue(id, out var resource)) continue;
                dict[id] = GetDef(id);
            }
            return dict;
        }
    }
}