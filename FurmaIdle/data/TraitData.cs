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
            #region Knowledge Gain Traits (o01 - o03)
            ["o01"] = new TraitModel
            {
                Id = "o01",
                Description = "Aumenta o ganho de Conhecimento Cultural",
                TargetId = "k01",
                EffectValue = 1.05,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.KnowledgeGain,
                Modifiers = new List<ModifierModel>(),
            },
            ["o02"] = new TraitModel
            {
                Id = "o02",
                Description = "Aumenta o ganho de Conhecimento Geográfico",
                TargetId = "k02",
                EffectValue = 1.05,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.KnowledgeGain,
                Modifiers = new List<ModifierModel>(),
            },
            ["o03"] = new TraitModel
            {
                Id = "o03",
                Description = "Aumenta o ganho de Conhecimento em Sobrevivência",
                TargetId = "k03",
                EffectValue = 1.05,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.KnowledgeGain,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region Resource Gain Trait (to04)
            ["o04"] = new TraitModel
            {
                Id = "o04",
                Description = "Gera Mantimentos por segundo",
                TargetId = "r01",
                EffectValue = 0.5,
                EffectOp = EffectHelper.EffectOperation.Additive,
                EffectType = EffectHelper.EffectType.ResourceGain,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region Cost Reduction Trait (to05)
            ["o05"] = new TraitModel
            {
                Id = "o05",
                Description = "Reduz o custo de novos Personagens",
                TargetId = "aCharacters",
                EffectValue = 0.95,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.CharacterCost,
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
                Persistence = UnlockHelper.Persistence.untilExpedition,
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