using FurmaIdle.Models; // Assumindo que TechModel está aqui
using FurmaIdle.Helpers; // Assumindo que UnlockHelper está aqui
using System.Collections.Generic;
using System.Linq;

namespace FurmaIdle.Data
{
    public class TechData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> ShowOrder = new();

        internal static readonly Dictionary<string, TechModel> All = new()
        {
            #region Cultural (k01)
            ["t011"] = new TechModel
            {
                Id = "t011",
                Name = "Fundação de Guildas",
                Icon = "icons/techs/t011.svg",
                Description = "",
                Lore = "",
                UnlockId = "uh0101",
                Level = 1,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region Geográfico (k02)
            ["t021"] = new TechModel
            {
                Id = "t021",
                Name = "Vilas Litorâneas",
                Icon = "icons/techs/t021.svg",
                Description = "",
                Lore = "",
                UnlockId = "uh0201",
                Level = 1,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            ["t022"] = new TechModel
            {
                Id = "t022",
                Name = "Litorais Rochosos",
                Icon = "icons/techs/t022.svg",
                Description = "",
                Lore = "",
                UnlockId = "uh0202",
                Level = 2,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region T03x - Sobrevivência (k03)
            ["t031"] = new TechModel
            {
                Id = "t031",
                Name = "Vida em Muradas",
                Icon = "icons/techs/t031.svg",
                Description = "",
                Lore = "",
                UnlockId = "uh0301",
                Level = 1,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            ["t032"] = new TechModel
            {
                Id = "t032",
                Name = "Sobrevivência na Selva",
                Icon = "icons/techs/t032.svg",
                Description = "",
                Lore = "",
                UnlockId = "uh0302",
                Level = 2,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region T04x - Navegação (k04)
            ["t041"] = new TechModel
            {
                Id = "t041",
                Name = "Fluxo das Correntezas",
                Icon = "icons/techs/t041.svg", 
                Description = "",
                Lore = "",
                UnlockId = "uh0401",
                Level = 1,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            ["t042"] = new TechModel
            {
                Id = "t042",
                Name = "Construção de Pequenos Barcos",
                Icon = "icons/techs/t042.svg",
                Description = "",
                Lore = "",
                UnlockId = "uh0402",
                Level = 2,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region T05x - Caça (k05)
            ["t051"] = new TechModel
            {
                Id = "t051",
                Name = "Caça de Pequenos Animais",
                Icon = "icons/techs/t051.svg",
                Description = "",
                Lore = "",
                UnlockId = "uh0501",
                Level = 1,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            ["t052"] = new TechModel
            {
                Id = "t052",
                Name = "Caça de Grandes Herbívoros",
                Icon = "icons/techs/t052.svg",
                Description = "",
                Lore = "",
                UnlockId = "uh0502",
                Level = 2,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            ["t053"] = new TechModel
            {
                Id = "t053",
                Name = "Caça de Pequenos Predadores",
                Icon = "icons/techs/t053.svg",
                Description = "",
                Lore = "",
                UnlockId = "uh0503",
                Level = 3,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            }
            #endregion
        };

        // --- Métodos Reutilizáveis do Padrão ---

        public static TechModel GetDef(string id)
        {
            if (!All.TryGetValue(id, out var tech))
            {
                throw new KeyNotFoundException($"Tech with ID '{id}' not found.");
            }

            return new TechModel
            {
                Id = tech.Id,
                Name = tech.Name,
                ItemType = tech.ItemType,
                Icon = tech.Icon,
                Description = tech.Description,
                Lore = tech.Lore,
                UnlockId = tech.UnlockId,
                Level = tech.Level,
                State = tech.State,
                Persistence = tech.Persistence,
                Modifiers = tech.Modifiers,
                UseState = tech.UseState,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = All?.Keys.AsEnumerable() ?? Enumerable.Empty<string>();
            ShowOrder.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        public static Dictionary<string, TechModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, TechModel>(All.Count);
            if (ShowOrder.Count == 0) PopulateOrder();
            foreach (var id in ShowOrder)
            {
                if (!All.TryGetValue(id, out var tech)) continue;
                dict[id] = GetDef(id);
            }
            return dict;
        }
    }
}