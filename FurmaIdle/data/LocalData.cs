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
            #region Stage s01
            ["l10"] = new LocalModel
            {
                Id = "l10",
                Name = "Murada Cairu",
                Description = "",
                Lore = "",
                Icon = "icons/locals/l10.svg",
                Image = "images/locals/l10.svg",
                Level = null,
                UnlockId = null,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                StageId = "s01",
                Modifiers = new List<ModifierModel>(),
            },
            ["l11"] = new LocalModel
            {
                Id = "l11",
                Name = "Pontas Cantarolantes",
                Description = "",
                Lore = "",
                Icon = "icons/locals/l11.svg",
                Image = "images/locals/l11.svg",
                Level = 1,
                UnlockId = "ul11",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                StageId = "s01",
                Modifiers = new List<ModifierModel>(),
            },
            ["l12"] = new LocalModel
            {
                Id = "l12",
                Name = "Coração da Ilha",
                Description = "",
                Lore = "",
                Icon = "icons/locals/l12.svg",
                Image = "images/locals/l12.svg",
                Level = 1,
                UnlockId = "ul12",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                StageId = "s01",
                Modifiers = new List<ModifierModel>(),
            },
            ["l13"] = new LocalModel
            {
                Id = "l13",
                Name = "Bosque da Raposa",
                Description = "",
                Lore = "",
                Icon = "icons/locals/l13.svg",
                Image = "images/locals/l13.svg",
                Level = 1,
                UnlockId = "ul13",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                StageId = "s01",
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
                Lore = local.Lore,
                Icon = local.Icon,
                Image = local.Image,
                Level = local.Level,
                UnlockId = local.UnlockId,
                State = local.State,
                Persistence = local.Persistence,
                StageId = local.StageId,
                Modifiers = local.Modifiers,
                UseState = local.UseState,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = All?.Keys.AsEnumerable() ?? Enumerable.Empty<string>();

            // Ordena usando StringComparer.Ordinal (l10, l11, l12, l13, etc.)
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