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
            #region Expansion Levels (x00 - x03)
            ["x00"] = new ExpansionModel
            {
                Id = "x00",
                Name = "Primeiros Recrutas",
                UnlockId = null,
                Level = 1,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                Modifiers = new List<ModifierModel>(),
                NextExpansion = "x01",
            },
            ["x01"] = new ExpansionModel
            {
                Id = "x01",
                Name = "Apoiando a Murada Cairu",
                UnlockId = "ue01",
                Level = 1,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                Modifiers = new List<ModifierModel>(),
                NextExpansion = "x02",
            },
            ["x02"] = new ExpansionModel
            {
                Id = "x02",
                Name = "Mestres da Ilha de Vera",
                UnlockId = "ue02",
                Level = 2,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                Modifiers = new List<ModifierModel>(),
                NextExpansion = "x03",
            },
            ["x03"] = new ExpansionModel
            {
                Id = "x03",
                Name = "Correntezas de Vera",
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
                UnlockId = expansion.UnlockId,
                PricingId = expansion.PricingId,
                Level = expansion.Level,
                Persistence = expansion.Persistence,
                State = expansion.State,
                Modifiers = expansion.Modifiers,
                ExpansionStats = new StatsModel(),
                NextExpansion = expansion.NextExpansion,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = All?.Keys.AsEnumerable() ?? Enumerable.Empty<string>();

            // Ordena usando StringComparer.Ordinal (x01, x02, x03, etc.)
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