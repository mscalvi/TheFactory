using FurmaIdle.Models;
using FurmaIdle.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace FurmaIdle.Data
{
    public class TraitData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> ShowOrder = new();

        internal static readonly Dictionary<string, TraitModel> All = new()
        {
            #region Traits
            ["a0001"] = new TraitModel
            {
                Id = "a0001",
                Description = "Diminui o tempo para Servir Bebidas.",
                TargetId = "c201",
                EffectValue = 0.80,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.ContractTime,
                EffectSupertype = EffectHelper.EffectSupertype.Time,
                Modifiers = new List<ModifierModel>(),
            },
            ["a0101"] = new TraitModel
            {
                Id = "a0101",
                Description = "Diminui o tempo para Organizar Ferramentas.",
                TargetId = "c302",
                EffectValue = 0.80,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.ContractTime,
                EffectSupertype = EffectHelper.EffectSupertype.Time,
                Modifiers = new List<ModifierModel>(),
            },
            ["a0102"] = new TraitModel
            {
                Id = "a0102",
                Description = "Diminui o tempo para Carregar o Barco.",
                TargetId = "c102",
                EffectValue = 0.75,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.ContractTime,
                EffectSupertype = EffectHelper.EffectSupertype.Time,
                Modifiers = new List<ModifierModel>(),
            },
            ["a0103"] = new TraitModel
            {
                Id = "a0103",
                Description = "Diminui o custo das Especialidades da Guilda.",
                TargetId = "allSpecialties",
                EffectValue = 0.9,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.SpecialtyCost,
                EffectSupertype = EffectHelper.EffectSupertype.Cost,
                Modifiers = new List<ModifierModel>(),
            },
            ["a0111"] = new TraitModel
            {
                Id = "a0111",
                Description = "Aumenta a geração de Mantimentos da Guilda.",
                TargetId = "r01",
                EffectValue = 0.75,
                EffectOp = EffectHelper.EffectOperation.Additive,
                EffectType = EffectHelper.EffectType.ResourceGain,
                EffectSupertype = EffectHelper.EffectSupertype.Gain,
                Modifiers = new List<ModifierModel>(),
            },
            ["a0121"] = new TraitModel
            {
                Id = "a0121",
                Description = "Aumenta o ganho de Caçar.",
                TargetId = "c405",
                EffectValue = 1.25,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.ContractGain,
                EffectSupertype = EffectHelper.EffectSupertype.Gain,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion
        };

        // --- Métodos Reutilizáveis do Padrão ---

        public static TraitModel GetDef(string id)
        {
            if (!All.TryGetValue(id, out var trait))
            {
                throw new KeyNotFoundException($"Trait with ID '{id}' not found.");
            }

            return new TraitModel
            {
                Id = trait.Id,
                ItemType = trait.ItemType,
                Description = trait.Description,
                TargetId = trait.TargetId,
                EffectValue = trait.EffectValue,
                EffectOp = trait.EffectOp,
                EffectType = trait.EffectType,
                Modifiers = trait.Modifiers,
                EffectSupertype= trait.EffectSupertype,
                Persistence = UnlockHelper.Persistence.untilExpedition,
                UseState = trait.UseState,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = All?.Keys.AsEnumerable() ?? Enumerable.Empty<string>();

            // Ordena usando StringComparer.Ordinal
            ShowOrder.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        public static Dictionary<string, TraitModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, TraitModel>(All.Count);

            if (ShowOrder.Count == 0) PopulateOrder();

            foreach (var id in ShowOrder)
            {
                if (!All.TryGetValue(id, out var trait)) continue;

                // Cria o estado inicial do modelo clonado
                dict[id] = GetDef(id);
            }
            return dict;
        }
    }
}