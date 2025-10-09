using FurmaIdle.Models;

namespace FurmaIdle.Data
{
    public static class TechData
    {
        public static int SchemaVersion => 1;

        // ordem de exibição/seed
        public static readonly List<string> Order = new() { "t10", "t11", "t12", "t13", "t14", "t20", "t30" };

        // catálogo IMUTÁVEL (não use em runtime diretamente)
        internal static readonly Dictionary<string, TechModel> All = new()
        {
            ["t10"] = new TechModel
            {
                Id = "t10",
                Name = "Fundação de Guildas",
                Cost = 1,
                CostKnowledgeId = "k10",
                Image = "images/icons/techonology/t10.jpg",
                Unlocked = false,
                Avaliable = true,
                DestinationId = "d00"
            },
            ["t11"] = new TechModel
            {
                Id = "t11",
                Name = "Vilas Litorâneas",
                Cost = 1,
                CostKnowledgeId = "k11",
                Image = "images/icons/techonology/t11.jpg",
                Unlocked = false,
                Avaliable = true,
                DestinationId = "d00"
            },
            ["t12"] = new TechModel
            {
                Id = "t12",
                Name = "Vida em Muradas",
                Cost = 1,
                CostKnowledgeId = "k12",
                Image = "images/icons/techonology/t12.jpg",
                Unlocked = false,
                Avaliable = true,
                DestinationId = "d00"
            },
            ["t13"] = new TechModel
            {
                Id = "t13",
                Name = "Litorais Rochosos",
                Cost = 10,
                CostKnowledgeId = "k11",
                Image = "images/icons/techonology/t13.jpg",
                Unlocked = false,
                Avaliable = false,
                DestinationId = "d01"
            },
            ["t14"] = new TechModel
            {
                Id = "t14",
                Name = "Sobrevivência na Selva",
                Cost = 50,
                CostKnowledgeId = "k12",
                Image = "images/icons/techonology/t14.jpg",
                Unlocked = false,
                Avaliable = false,
                DestinationId = "d02"
            },
            ["t20"] = new TechModel
            {
                Id = "t20",
                Name = "Construção de Barcos",
                Cost = 1,
                CostKnowledgeId = "k20",
                Image = "images/icons/techonology/t20.jpg",
                Unlocked = false,
                Avaliable = false,
                DestinationId = "d03"
            },
            ["t30"] = new TechModel
            {
                Id = "t30",
                Name = "Insaniophora",
                Cost = 50,
                CostKnowledgeId = "k30",
                Image = "images/icons/techonology/t30.jpg",
                Unlocked = false,
                Avaliable = false,
                DestinationId = "d01"
            }
        };

        public static TechModel GetDef(string id)
        {
            var tech = All[id];
            return new TechModel
            {
                Id = tech.Id,
                Name = tech.Name,
                Cost = tech.Cost,
                CostKnowledgeId = tech.CostKnowledgeId,
                Image = tech.Image,
                Unlocked = tech.Unlocked,
                Avaliable = tech.Avaliable,
                DestinationId = tech.DestinationId
            };
        }

        public static Dictionary<string, TechModel> CreateInitialTechs()
        {
            var dict = new Dictionary<string, TechModel>(All.Count);
            foreach (var id in Order)
            {
                if (!All.TryGetValue(id, out var tech)) continue;

                dict[id] = new TechModel
                {
                    Id = tech.Id,
                    Name = tech.Name,
                    Cost = tech.Cost,
                    CostKnowledgeId = tech.CostKnowledgeId,
                    Image = tech.Image,
                    Unlocked = tech.Unlocked,
                    Avaliable = tech.Avaliable,
                    DestinationId = tech.DestinationId
                };
            }
            return dict;
        }
    }
}
