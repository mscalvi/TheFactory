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
            #region Stage s00
            ["l000"] = new LocalModel
            {
                Id = "l000",
                Name = "Escrivaninha das Ideias",
                Description = "",
                Lore = "",
                Icon = "icons/locals/l000.svg",
                Image = "images/locals/l000.svg",
                Level = null,
                UnlockId = "s00",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                StageId = "s00",
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region Stage s01
            ["l010"] = new LocalModel
            {
                Id = "l010",
                Name = "Murada Cairu",
                Description = "",
                Lore = "",
                Icon = "icons/locals/l010.svg",
                Image = "images/locals/l010.svg",
                Level = null,
                UnlockId = "s01",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                StageId = "s01",
                Modifiers = new List<ModifierModel>(),
            },
            ["l011"] = new LocalModel
            {
                Id = "l011",
                Name = "Pontas Cantarolantes",
                Description = "",
                Lore = "",
                Icon = "icons/locals/l011.svg",
                Image = "images/locals/l011.svg",
                Level = 1,
                UnlockId = "ul011",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                StageId = "s01",
                Modifiers = new List<ModifierModel>(),
            },
            ["l012"] = new LocalModel
            {
                Id = "l012",
                Name = "Coração da Ilha",
                Description = "",
                Lore = "",
                Icon = "icons/locals/l012.svg",
                Image = "images/locals/l012.svg",
                Level = 1,
                UnlockId = "ul012",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                StageId = "s01",
                Modifiers = new List<ModifierModel>(),
            },
            ["l013"] = new LocalModel
            {
                Id = "l013",
                Name = "Bosque da Raposa",
                Description = "",
                Lore = "",
                Icon = "icons/locals/l013.svg",
                Image = "images/locals/l013.svg",
                Level = 1,
                UnlockId = "ul013",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                StageId = "s01",
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region Stage s02
            ["l020"] = new LocalModel
            {
                Id = "l020",
                Name = "Praia Fechada",
                Description = "",
                Lore = "",
                Icon = "icons/locals/l020.svg",
                Image = "images/locals/l020.svg",
                Level = null,
                UnlockId = "s02",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                StageId = "s02",
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
                ItemType = local.ItemType,
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

            // Ordena usando StringComparer.Ordinal (l010, l011, l012, l013, etc.)
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