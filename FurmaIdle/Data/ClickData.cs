using FurmaIdle.Helpers;
using FurmaIdle.Models;

namespace FurmaIdle.Data
{
    public class ClickData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> ShowOrder = new();

        internal static readonly Dictionary<string, ClickModel> All = new()
        {
            #region Initial Stage (s00)
            ["i00"] = new ClickModel
            {
                Id = "i00",
                StageId = "s00",
                BaseGain = 1,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion
            #region NoExistent Stage (s01)
            ["i01"] = new ClickModel
            {
                Id = "i01",
                StageId = "s01",
                BaseGain = 1,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion
        };

        // --- Métodos Reutilizáveis do Padrão ---

        public static ClickModel GetDef(string id)
        {
            if (!All.TryGetValue(id, out var click))
            {
                throw new KeyNotFoundException($"Click with ID '{id}' not found.");
            }

            // Retorna uma nova instância (cópia) para não modificar a definição estática
            return new ClickModel
            {
                Id = click.Id,
                StageId = click.StageId,
                BaseGain = click.BaseGain,
                Modifiers = click.Modifiers,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = All?.Keys.AsEnumerable() ?? Enumerable.Empty<string>();

            ShowOrder.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        public static Dictionary<string, ClickModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, ClickModel>(All.Count);

            if (ShowOrder.Count == 0) PopulateOrder();

            foreach (var id in ShowOrder)
            {
                if (!All.TryGetValue(id, out var click)) continue;

                dict[id] = GetDef(id);
            }
            return dict;
        }
    }
}
