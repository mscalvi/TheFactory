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
            #region Knowledge Gain Traits (tr01 - tr03)
            ["tr01"] = new TraitModel
            {
                Id = "tr01",
                Description = "Aumenta o ganho de Conhecimento Cultural",
                TargetId = "k01",
                EffectValue = 1.05,
                EffectOperation = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.KnowledgeGain,
            },
            ["tr02"] = new TraitModel
            {
                Id = "tr02",
                Description = "Aumenta o ganho de Conhecimento Geográfico",
                TargetId = "k02",
                EffectValue = 1.05,
                EffectOperation = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.KnowledgeGain,
            },
            ["tr03"] = new TraitModel
            {
                Id = "tr03",
                Description = "Aumenta o ganho de Conhecimento em Sobrevivência",
                TargetId = "k03",
                EffectValue = 1.05,
                EffectOperation = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.KnowledgeGain,
            },
            #endregion

            #region Resource Gain Trait (tr04)
            ["tr04"] = new TraitModel
            {
                Id = "tr04",
                Description = "Gera Mantimentos por segundo",
                TargetId = "r01",
                EffectValue = 0.5,
                EffectOperation = EffectHelper.EffectOperation.Additive,
                EffectType = EffectHelper.EffectType.ResourceGain,
            },
            #endregion

            #region Cost Reduction Trait (tr05)
            ["tr05"] = new TraitModel
            {
                Id = "tr05",
                Description = "Reduz o custo de novos Personagens",
                TargetId = "aCharacters",
                EffectValue = 0.95,
                EffectOperation = EffectHelper.EffectOperation.Multiplicative,
                EffectType = EffectHelper.EffectType.CharacterCost,
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
                EffectOperation = trait.EffectOperation,
                EffectType = trait.EffectType,
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