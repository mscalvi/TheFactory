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
            ["o02"] = new TraitModel
            {
                Id = "o02",
                Description = "Diminui o Tempo para Servir Bebidas",
                TargetId = "c021",
                EffectValue = 0.80,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.ContractTime,
                EffectSupertype = EffectHelper.EffectSupertype.Time,
                Modifiers = new List<ModifierModel>(),
            },
            ["o01"] = new TraitModel
            {
                Id = "o01",
                Description = "Diminui o Tempo para Organizar Ferramentas",
                TargetId = "c032",
                EffectValue = 0.80,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.ContractTime,
                EffectSupertype = EffectHelper.EffectSupertype.Time,
                Modifiers = new List<ModifierModel>(),
            },
            ["o03"] = new TraitModel
            {
                Id = "o03",
                Description = "Diminui o Tempo para Carregar o Barco",
                TargetId = "c012",
                EffectValue = 0.75,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.ContractTime,
                EffectSupertype = EffectHelper.EffectSupertype.Time,
                Modifiers = new List<ModifierModel>(),
            },
            ["o04"] = new TraitModel
            {
                Id = "o04",
                Description = "Diminui o custo das Especialidades",
                TargetId = "aSpecialties",
                EffectValue = 0.9,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.SpecialtyCost,
                EffectSupertype = EffectHelper.EffectSupertype.Cost,
                Modifiers = new List<ModifierModel>(),
            },
            ["o05"] = new TraitModel
            {
                Id = "o05",
                Description = "Aumenta a geração de Mantimentos",
                TargetId = "r01",
                EffectValue = 0.75,
                EffectOp = EffectHelper.EffectOperation.Additive,
                EffectType = EffectHelper.EffectType.ResourceGain,
                EffectSupertype = EffectHelper.EffectSupertype.Gain,
                Modifiers = new List<ModifierModel>(),
            },
            ["o06"] = new TraitModel
            {
                Id = "o06",
                Description = "Aumenta o Ganho para Caçar",
                TargetId = "c045",
                EffectValue = 1.5,
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