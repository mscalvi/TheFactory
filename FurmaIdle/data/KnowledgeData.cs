using FurmaIdle.Models;
using FurmaIdle.Helpers; 

namespace FurmaIdle.Data
{
    public class KnowledgeData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> ShowOrder = new();

        internal static readonly Dictionary<string, KnowledgeModel> All = new()
        {
            #region Initial Knowledge (Unlocked)
            ["k01"] = new KnowledgeModel
            {
                Id = "k01",
                Name = "Cultural",
                Image = "image/knowledges/k01.jpg",
                UnlockId = "uk01",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),

                // Knowledge Gain
                GainCoinId = "m01",
                GainCoinBase = 500000,
                GainCoinCurve = 0.85,
                GainFactorCurve = 0.6,

                // Generation Boost
                GenerationFactor = 0.15,
                GenerationPenaltie = 0.65,
            },
            ["k02"] = new KnowledgeModel
            {
                Id = "k02",
                Name = "Geográfico",
                Image = "image/knowledges/k01.jpg",
                UnlockId = "uk02",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),

                // Knowledge Gain
                GainCoinId = "m01",
                GainCoinBase = 500000,
                GainCoinCurve = 0.85,
                GainFactorCurve = 0.6,

                // Generation Boost
                GenerationFactor = 0.15,
                GenerationPenaltie = 0.65,
            },
            ["k03"] = new KnowledgeModel
            {
                Id = "k03",
                Name = "Sobrevivência",
                Image = "image/knowledges/k02.jpg",
                UnlockId = "uk03",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),

                // Knowledge Gain
                GainCoinId = "m01",
                GainCoinBase = 500000,
                GainCoinCurve = 0.85,
                GainFactorCurve = 0.6,

                // Generation Boost
                GenerationFactor = 0.15,
                GenerationPenaltie = 0.65,
            },
            #endregion

            #region Unlockable Knowledge (Blocked)
            ["k04"] = new KnowledgeModel
            {
                Id = "k04",
                Name = "Navegação",
                Image = "image/knowledges/k03.jpg",
                UnlockId = "uk04",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),

                // Knowledge Gain
                GainCoinId = "m01",
                GainCoinBase = 5000000,
                GainCoinCurve = 0.84,
                GainFactorCurve = 0.6,

                // Generation Boost
                GenerationFactor = 0.16,
                GenerationPenaltie = 0.7,
            },
            ["k05"] = new KnowledgeModel
            {
                Id = "k05",
                Name = "Caça",
                Image = "image/knowledges/k04.jpg",
                UnlockId = "uk05",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),

                // Knowledge Gain
                GainCoinId = "m01",
                GainCoinBase = 50000000,
                GainCoinCurve = 0.83,
                GainFactorCurve = 0.6,

                // Generation Boost
                GenerationFactor = 0.17,
                GenerationPenaltie = 0.75,
            }
            #endregion
        };

        // --- Métodos Reutilizáveis do Padrão ---

        public static KnowledgeModel GetDef(string id)
        {
            if (!All.TryGetValue(id, out var knowledge))
            {
                throw new KeyNotFoundException($"Knowledge with ID '{id}' not found.");
            }

            // Retorna uma nova instância (cópia) para não modificar a definição estática
            return new KnowledgeModel
            {
                Id = knowledge.Id,
                Name = knowledge.Name,
                Image = knowledge.Image,
                UnlockId = knowledge.UnlockId,
                State = knowledge.State,
                Persistence = knowledge.Persistence,
                GainCoinId = knowledge.GainCoinId,
                GainCoinBase = knowledge.GainCoinBase,
                GainCoinCurve = knowledge.GainCoinCurve,
                GainFactorCurve = knowledge.GainFactorCurve,
                GenerationFactor = knowledge.GenerationFactor,
                GenerationPenaltie = knowledge.GenerationPenaltie,
                Modifiers = knowledge.Modifiers,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = All?.Keys.AsEnumerable() ?? Enumerable.Empty<string>();

            // Ordena usando StringComparer.Ordinal (k01, k02, k03, etc.)
            ShowOrder.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        public static Dictionary<string, KnowledgeModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, KnowledgeModel>(All.Count);

            if (ShowOrder.Count == 0) PopulateOrder();

            foreach (var id in ShowOrder)
            {
                if (!All.TryGetValue(id, out var knowledge)) continue;

                // Cria o estado inicial do modelo clonado
                dict[id] = GetDef(id);
            }
            return dict;
        }
    }
}