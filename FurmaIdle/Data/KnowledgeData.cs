using FurmaIdle.Models;

namespace FurmaIdle.Data
{
    public class KnowledgeData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> Order = new()
        {
            "k10", "k11", "k12", "k20", "k30"
        };

        // Catálogo imutável em runtime. Preencha com seus destinos reais.
        internal static readonly Dictionary<string, KnowledgeModel> All = new()
        {
            ["k10"] = new KnowledgeModel
            {
                Id = "k10",
                Name = "Cultural",
                GainBase = 100000,
                GainCurve = 0.85,
                GainMultiplier = 1,
                KnowCoinGain = 0.15,
                KnowCoinGainPenaltie = 0.65,
                Image = "images/icons/knowledge/k10.png",
                Avaliable = true
            },
            ["k11"] = new KnowledgeModel
            {
                Id = "k11",
                Name = "Geográfico",
                GainBase = 100000,
                GainCurve = 0.85,
                GainMultiplier = 1,
                KnowCoinGain = 0.15,
                KnowCoinGainPenaltie = 0.65,
                Image = "images/icons/knowledge/k11.png",
                Avaliable = true
            },
            ["k12"] = new KnowledgeModel
            {
                Id = "k12",
                Name = "Sobrevivência",
                GainBase = 100000,
                GainCurve = 0.85,
                GainMultiplier = 1,
                KnowCoinGain = 0.15,
                KnowCoinGainPenaltie = 0.65,
                Image = "images/icons/knowledge/k12.png",
                Avaliable = true
            },
            ["k20"] = new KnowledgeModel
            {
                Id = "k20",
                Name = "Navegação",
                GainBase = 1000000,
                GainCurve = 0.84,
                GainMultiplier = 1,
                KnowCoinGain = 0.16,
                KnowCoinGainPenaltie = 0.7,
                Image = "images/icons/knowledge/k20.png",
                Avaliable = true
            },
            ["k30"] = new KnowledgeModel
            {
                Id = "k30",
                Name = "Alquimia",
                GainBase = 10000000,
                GainCurve = 0.83,
                GainMultiplier = 1,
                KnowCoinGain = 0.17,
                KnowCoinGainPenaltie = 0.85,
                Image = "images/icons/knowledge/k30.png",
                Avaliable = true
            },
        };

        // Retorna um clone defensivo da definição (evita mutação acidental do catálogo)
        public static KnowledgeModel GetDef(string id)
        {
            var know = All[id];
            return new KnowledgeModel
            {
                Id = know.Id,
                Name = know.Name,
                GainBase = know.GainBase,
                GainCurve = know.GainCurve,
                GainMultiplier = know.GainMultiplier,
                GainFactor = know.GainFactor,
                KnowCoinGain = know.KnowCoinGain,
                KnowCoinGainPenaltie = know.KnowCoinGainPenaltie,
                Image = know.Image,
                Avaliable = know.Avaliable,
                Points = know.Points,
                Persistence = know.Persistence,
            };
        }

        // Criação do dicionário inicial de jogo com base em Order e flag Unlocked
        public static Dictionary<string, KnowledgeModel> CreateInitialKnowledges()
        {
            var dict = new Dictionary<string, KnowledgeModel>(All.Count);
            foreach (var id in Order)
            {
                if (!All.TryGetValue(id, out var know)) continue;

                dict[id] = new KnowledgeModel
                {
                    Id = know.Id,
                    Name = know.Name,
                    GainBase = know.GainBase,
                    GainCurve = know.GainCurve,
                    GainMultiplier = know.GainMultiplier,
                    GainFactor = know.GainFactor,
                    KnowCoinGain = know.KnowCoinGain,
                    KnowCoinGainPenaltie = know.KnowCoinGainPenaltie,
                    Image = know.Image,
                    Avaliable = know.Avaliable,
                    Points = know.Points,
                    Persistence = know.Persistence,
                };
            }
            return dict;
        }
    }
}
