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
            #region Expansion Levels (x10 - x13)
            ["x10"] = new ExpansionModel
            {
                Id = "x10",
                Name = "Primeiros Recrutas",
                Icon = "icons/expansions/x10.svg",
                UnlockId = null,
                Level = 1,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                Modifiers = new List<ModifierModel>(),
                NextExpansion = "x11",
            },
            ["x11"] = new ExpansionModel
            {
                Id = "x11",
                Name = "Apoiando a Murada Cairu",
                Icon = "icons/expansions/x11.svg",
                UnlockId = "ue01",
                Level = 1,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                Modifiers = new List<ModifierModel>(),
                NextExpansion = "x12",
            },
            ["x12"] = new ExpansionModel
            {
                Id = "x12",
                Name = "Mestres da Ilha de Vera",
                Icon = "icons/expansions/x12.svg",
                UnlockId = "ue02",
                Level = 2,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                Modifiers = new List<ModifierModel>(),
                NextExpansion = "x13",
            },
            ["x13"] = new ExpansionModel
            {
                Id = "x13",
                Name = "Correntezas de Vera",
                Icon = "icons/expansions/x13.svg",
                UnlockId = "ue03",
                Level = 3,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                Modifiers = new List<ModifierModel>(),
                NextExpansion = "x04",
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