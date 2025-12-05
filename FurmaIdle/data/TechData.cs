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
            #region t01 - Cultural (k01)
            ["t0101"] = new TechModel
            {
                Id = "t0101",
                Name = "Fundação de Guildas",
                Icon = "icons/techs/t0101.svg",
                Description = "",
                Lore = "",
                UnlockId = "uh0101",
                Level = 1,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region t02 - Geográfico (k02)
            ["t0201"] = new TechModel
            {
                Id = "t0201",
                Name = "Vilas Litorâneas",
                Icon = "icons/techs/t0201.svg",
                Description = "",
                Lore = "",
                UnlockId = "uh0201",
                Level = 1,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            ["t0202"] = new TechModel
            {
                Id = "t0202",
                Name = "Litorais Rochosos",
                Icon = "icons/techs/t0202.svg",
                Description = "",
                Lore = "",
                UnlockId = "uh0202",
                Level = 2,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region t03 - Sobrevivência (k03)
            ["t0301"] = new TechModel
            {
                Id = "t0301",
                Name = "Vida em Muradas",
                Icon = "icons/techs/t0301.svg",
                Description = "",
                Lore = "",
                UnlockId = "uh0301",
                Level = 1,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            ["t0302"] = new TechModel
            {
                Id = "t0302",
                Name = "Sobrevivência na Selva",
                Icon = "icons/techs/t0302.svg",
                Description = "",
                Lore = "",
                UnlockId = "uh0302",
                Level = 2,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region t04 - Navegação (k04)
            ["t0401"] = new TechModel
            {
                Id = "t0401",
                Name = "Fluxo das Correntezas",
                Icon = "icons/techs/t0401.svg", 
                Description = "",
                Lore = "",
                UnlockId = "uh0401",
                Level = 1,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            ["t0402"] = new TechModel
            {
                Id = "t0402",
                Name = "Construção de Pequenas Embarcações",
                Icon = "icons/techs/t0402.svg",
                Description = "",
                Lore = "",
                UnlockId = "uh0402",
                Level = 2,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region t05 - Caça (k05)
            ["t0501"] = new TechModel
            {
                Id = "t0501",
                Name = "Caça de Pequenos Animais",
                Icon = "icons/techs/t0501.svg",
                Description = "",
                Lore = "",
                UnlockId = "uh0501",
                Level = 1,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),
            },
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