using FurmaIdle.Models; // Assumindo que LocalModel está aqui
using FurmaIdle.Helpers; // Assumindo que UnlockHelper e PricingHelper estão aqui
using System.Collections.Generic;
using System.Linq;

namespace FurmaIdle.Data
{
    public class LocalData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> ShowOrder = new();

        internal static readonly Dictionary<string, LocalModel> All = new()
        {
            #region Initial Local (l00)
            ["l00"] = new LocalModel
            {
                Id = "l00",
                Name = "Murada Cairu",
                Description = "",
                Icon = "icons/locals/l00.jpg",
                Level = null,
                UnlockId = "ul00",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.untilExpansion,
                StageId = "s00",
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region Unlockable Locals (l01 - l03)
            ["l01"] = new LocalModel
            {
                Id = "l01",
                Name = "Pontes Cantarolantes",
                Description = "",
                Icon = "icons/locals/l01.jpg",
                Level = 1,
                UnlockId = "ul01",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.untilExpansion,
                StageId = "s00",
                Modifiers = new List<ModifierModel>(),
            },
            ["l02"] = new LocalModel
            {
                Id = "l02",
                Name = "Coração da Ilha",
                Description = "",
                Icon = "icons/locals/l02.jpg",
                Level = 1,
                UnlockId = "ul02",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.untilExpansion,
                StageId = "s00",
                Modifiers = new List<ModifierModel>(),
            },
            ["l03"] = new LocalModel
            {
                Id = "l03",
                Name = "Bosque da Raposa",
                Description = "",
                Icon = "icons/locals/l03.jpg",
                Level = 1,
                UnlockId = "ul03",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.untilExpansion,
                StageId = "s00",
                Modifiers = new List<ModifierModel>(),
            },
            #endregion
        };

        // --- Métodos Reutilizáveis do Padrão ---

        public static LocalModel GetDef(string id)
        {
            if (!All.TryGetValue(id, out var local))
            {
                throw new KeyNotFoundException($"Local with ID '{id}' not found.");
            }

            // Retorna uma nova instância (cópia) para não modificar a definição estática
            return new LocalModel
            {
                Id = local.Id,
                Name = local.Name,
                Description = local.Description,
                Icon = local.Icon,
                Level = local.Level,
                UnlockId = local.UnlockId,
                State = local.State,
                Persistence = local.Persistence,
                StageId = local.StageId,
                Modifiers = local.Modifiers,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = All?.Keys.AsEnumerable() ?? Enumerable.Empty<string>();

            // Ordena usando StringComparer.Ordinal (l00, l01, l02, l03, etc.)
            ShowOrder.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        public static Dictionary<string, LocalModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, LocalModel>(All.Count);

            if (ShowOrder.Count == 0) PopulateOrder();

            foreach (var id in ShowOrder)
            {
                if (!All.TryGetValue(id, out var local)) continue;

                // Cria o estado inicial do modelo clonado
                dict[id] = GetDef(id);
            }
            return dict;
        }
    }
}