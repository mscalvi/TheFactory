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
                UnlockId = "ux004", // Possível ID de expansão/tech
                RsActual = 150, // Usando double
                RsFraction = 0.0,
                RsPerSecond = 0.0,
                RsPerChar = 60.0,
                Icon = "icon/resources/r01.png",
                Image = "images/resources/r01.png",
                Lore = "", // Vazio na tabela
                Persistence = UnlockHelper.Persistence.untilExpansion,
                State = UnlockHelper.State.Blocked,
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
                RsActual = resource.RsActual,
                RsFraction = resource.RsFraction,
                RsPerSecond = resource.RsPerSecond,
                RsPerChar = resource.RsPerChar,
                Icon = resource.Icon,
                Image = resource.Image,
                Lore = resource.Lore,
                Persistence = resource.Persistence,
                State = resource.State,
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