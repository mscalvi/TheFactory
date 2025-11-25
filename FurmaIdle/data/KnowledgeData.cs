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
                Image = "images/knowledges/k01.svg",
                Description = "",
                Lore = "",
                UnlockId = "uk01",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),

                // Knowledge Gain
                GainCoinId = "m01",
                GainCoinBase = 100000,
                GainCoinCurve = 0.85,

                // Generation Boost
                GenerationFactor = 0.1,
                GenerationPenaltie = 0.65,
                GenerationCoin = IncomeHelper.CoinBurst.m01,
            },
            ["k02"] = new KnowledgeModel
            {
                Id = "k02",
                Name = "Geográfico",
                Image = "images/knowledges/k02.svg",
                Description = "",
                Lore = "",
                UnlockId = "uk02",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),

                // Knowledge Gain
                GainCoinId = "m01",
                GainCoinBase = 100000,
                GainCoinCurve = 0.85,

                // Generation Boost
                GenerationFactor = 0.1,
                GenerationPenaltie = 0.65,
                GenerationCoin = IncomeHelper.CoinBurst.m01,
            },
            ["k03"] = new KnowledgeModel
            {
                Id = "k03",
                Name = "Sobrevivência",
                Image = "images/knowledges/k03.svg",
                Description = "",
                Lore = "",
                UnlockId = "uk03",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),

                // Knowledge Gain
                GainCoinId = "m01",
                GainCoinBase = 100000,
                GainCoinCurve = 0.85,

                // Generation Boost
                GenerationFactor = 0.1,
                GenerationPenaltie = 0.65,
                GenerationCoin = IncomeHelper.CoinBurst.m01,
            },
            #endregion

            #region Unlockable Knowledge (Blocked)
            ["k04"] = new KnowledgeModel
            {
                Id = "k04",
                Name = "Navegação",
                Image = "images/knowledges/k04.svg",
                Description = "",
                Lore = "",
                UnlockId = "uk04",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),

                // Knowledge Gain
                GainCoinId = "m01",
                GainCoinBase = 1000000,
                GainCoinCurve = 0.82,

                // Generation Boost
                GenerationFactor = 0.12,
                GenerationPenaltie = 0.7,
                GenerationCoin = IncomeHelper.CoinBurst.m02,
            },
            ["k05"] = new KnowledgeModel
            {
                Id = "k05",
                Name = "Caça",
                Image = "images/knowledges/k05.svg",
                Description = "",
                Lore = "",
                UnlockId = "uk05",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                Modifiers = new List<ModifierModel>(),

                // Knowledge Gain
                GainCoinId = "m01",
                GainCoinBase = 10000000,
                GainCoinCurve = 0.8,

                // Generation Boost
                GenerationFactor = 0.15,
                GenerationPenaltie = 0.75,
                GenerationCoin = IncomeHelper.CoinBurst.m02,
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
                Description = knowledge.Description,
                Lore = knowledge.Lore,
                UnlockId = knowledge.UnlockId,
                State = knowledge.State,
                Persistence = knowledge.Persistence,
                GainCoinId = knowledge.GainCoinId,
                GainCoinBase = knowledge.GainCoinBase,
                GainCoinCurve = knowledge.GainCoinCurve,
                GenerationFactor = knowledge.GenerationFactor,
                GenerationPenaltie = knowledge.GenerationPenaltie,
                GenerationCoin = knowledge.GenerationCoin,
                Modifiers = knowledge.Modifiers,
                UseState = knowledge.UseState,
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