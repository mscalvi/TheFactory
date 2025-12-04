using FurmaIdle.Models;
using FurmaIdle.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace FurmaIdle.Data
{
    public class ExpansionData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> ShowOrder = new();

        internal static readonly Dictionary<string, ExpansionModel> All = new()
        {
            #region Expansions Stage 0 (x00)
            ["x000"] = new ExpansionModel
            {
                Id = "x000",
                Name = "Início da História",
                Icon = "icons/expansions/x000.svg",
                UnlockId = null,
                Level = 1,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                Modifiers = new List<ModifierModel>(),
                NextExpansion = "x010",
            },
            #endregion

            #region Expansions Stage 1 (x10 - x13)
            ["x010"] = new ExpansionModel
            {
                Id = "x010",
                Name = "Primeiros Recrutas",
                Icon = "icons/expansions/x010.svg",
                UnlockId = "s01",
                Level = 1,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                Modifiers = new List<ModifierModel>(),
                NextExpansion = "x011",
            },
            ["x011"] = new ExpansionModel
            {
                Id = "x011",
                Name = "Apoiando a Murada Cairu",
                Icon = "icons/expansions/x011.svg",
                UnlockId = "ux011",
                Level = 1,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                Modifiers = new List<ModifierModel>(),
                NextExpansion = "x012",
            },
            ["x012"] = new ExpansionModel
            {
                Id = "x012",
                Name = "Exploradores da Ilha de Vera",
                Icon = "icons/expansions/x012.svg",
                UnlockId = "ux012",
                Level = 2,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                Modifiers = new List<ModifierModel>(),
                NextExpansion = "x013",
            },
            ["x013"] = new ExpansionModel
            {
                Id = "x13",
                Name = "Mestres da Ilha de Vera",
                Icon = "icons/expansions/x013.svg",
                UnlockId = "ux013",
                Level = 3,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                Modifiers = new List<ModifierModel>(),
                NextExpansion = "x021",
            },
            #endregion

            #region Expansions Stage 2 (x20 - x2?)
            ["x021"] = new ExpansionModel
            {
                Id = "x021",
                Name = "Primeiro Contato",
                Icon = "icons/expansions/x021.svg",
                UnlockId = "ux021",
                Level = 1,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                Modifiers = new List<ModifierModel>(),
                NextExpansion = "x022",
            },
            #endregion
        };

        // --- Métodos Reutilizáveis do Padrão ---

        public static ExpansionModel GetDef(string id)
        {
            if (!All.TryGetValue(id, out var expansion))
            {
                throw new KeyNotFoundException($"Expansion with ID '{id}' not found.");
            }

            // Retorna uma nova instância (cópia) para não modificar a definição estática
            return new ExpansionModel
            {
                Id = expansion.Id,
                Name = expansion.Name,
                ItemType = expansion.ItemType,
                Icon = expansion.Icon,
                UnlockId = expansion.UnlockId,
                PricingId = expansion.PricingId,
                Level = expansion.Level,
                Persistence = expansion.Persistence,
                State = expansion.State,
                Modifiers = expansion.Modifiers,
                ExpansionStats = new StatsModel(),
                NextExpansion = expansion.NextExpansion,
                UseState = expansion.UseState,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = All?.Keys.AsEnumerable() ?? Enumerable.Empty<string>();

            // Ordena usando StringComparer.Ordinal (x11, x12, x13, etc.)
            ShowOrder.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        public static Dictionary<string, ExpansionModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, ExpansionModel>(All.Count);

            if (ShowOrder.Count == 0) PopulateOrder();

            foreach (var id in ShowOrder)
            {
                if (!All.TryGetValue(id, out var expansion)) continue;

                // Cria o estado inicial do modelo clonado
                dict[id] = GetDef(id);
            }
            return dict;
        }
    }
}