using FurmaIdle.Models;
using System.Reflection.Emit;
using static FurmaIdle.Data.UpgradeCostEnum;

namespace FurmaIdle.Data
{
    public static partial class UpgradeData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> Order;

        internal static readonly Dictionary<string, UpgradeModel> All = new()
        {
            #region t10
            ["m100"] = Build(new UpgradeModel
            {
                Id = "m100",
                Name = "Utensílios da Guilda",
                Image = "images/icons/upgrades/m100.png",
                TechId = "t10",
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1C1,
            }),
            ["m101"] = Build(new UpgradeModel
            {
                Id = "m101",
                Name = "Disciplina da Guilda",
                Image = "images/icons/upgrades/m101.png",
                TechId = "t10",
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1C1,
            }),
            ["m102"] = Build(new UpgradeModel
            {
                Id = "m102",
                Name = "Coletividade da Guilda",
                Image = "images/icons/upgrades/m102.png",
                TechId = "t10",
                Range = 2,
                CostCode = UpgradeCostCode.Quantidade1C1,
            }),
            ["m103"] = Build(new UpgradeModel
            {
                Id = "m103",
                Name = "União da Guilda",
                Image = "images/icons/upgrades/m103.png",
                TechId = "t10",
                Range = 1,
                CostCode = UpgradeCostCode.Geracao1R1,
            }),
            ["m104"] = Build(new UpgradeModel
            {
                Id = "m104",
                Name = "Escritório da Guilda",
                Image = "images/icons/upgrades/m104.png",
                TechId = "t10",
                Range = 1,
                CostCode = UpgradeCostCode.LimiteContrato1T1,
            }),
            #endregion
            #region t11
            ["m110"] = Build(new UpgradeModel
            {
                Id = "m110",
                Name = "Facas de Escamar",
                Image = "images/icons/upgrades/m110.png",
                TechId = "t11",
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1C2,
            }),
            ["m111"] = Build(new UpgradeModel
            {
                Id = "m111",
                Name = "Reservas de Escambo",
                Image = "images/icons/upgrades/m111.png",
                TechId = "t11",
                Range = 2,
                CostCode = UpgradeCostCode.Quantidade1C1,
            }),
            ["m112"] = Build(new UpgradeModel
            {
                Id = "m112",
                Name = "Cordas Grossas",
                Image = "images/icons/upgrades/m112.png",
                TechId = "t11",
                Range = 3,
                CostCode = UpgradeCostCode.Quantidade1C1,
            }),
            ["m113"] = Build(new UpgradeModel
            {
                Id = "m113",
                Name = "Água Aproveitada",
                Image = "images/icons/upgrades/m113.png",
                TechId = "t11",
                Range = 1,
                CostCode = UpgradeCostCode.Tempo1C1,
            }),
            ["m114"] = Build(new UpgradeModel
            {
                Id = "m114",
                Name = "Armazéns Salinos",
                Image = "images/icons/upgrades/m114.png",
                TechId = "t11",
                Range = 1,
                CostCode = UpgradeCostCode.Tempo1C1,
            }),
            #endregion
            #region t12
            ["m120"] = Build(new UpgradeModel
            {
                Id = "m120",
                Name = "Manutenção Coletiva",
                Image = "images/icons/upgrades/m120.png",
                TechId = "t12",
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1C3,
            }),
            ["m121"] = Build(new UpgradeModel
            {
                Id = "m121",
                Name = "Rotina Rígida",
                Image = "images/icons/upgrades/m121.png",
                TechId = "t12",
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1C3,
            }),
            ["m122"] = Build(new UpgradeModel
            {
                Id = "m122",
                Name = "Necessidades Ocultas",
                Image = "images/icons/upgrades/m122.png",
                TechId = "t12",
                Range = 2,
                CostCode = UpgradeCostCode.Quantidade1C2,
            }),
            ["m123"] = Build(new UpgradeModel
            {
                Id = "m123",
                Name = "União de Moradores",
                Image = "images/icons/upgrades/m123.png",
                TechId = "t12",
                Range = 1,
                CostCode = UpgradeCostCode.Tempo1C3,
            }),
            ["m124"] = Build(new UpgradeModel
            {
                Id = "m124",
                Name = "Vigilância Constante",
                Image = "images/icons/upgrades/m124.png",
                TechId = "t12",
                Range = 1,
                CostCode = UpgradeCostCode.Tempo1C3,
            }),
            #endregion
            #region t13
            ["m130"] = Build(new UpgradeModel
            {
                Id = "m130",
                Name = "Lascas Naturais",
                Image = "images/icons/upgrades/m130.png",
                TechId = "t13",
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1C4,
            }),
            ["m131"] = Build(new UpgradeModel
            {
                Id = "m121",
                Name = "Piscinas Naturais",
                Image = "images/icons/upgrades/m131.png",
                TechId = "t13",
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1C4,
            }),
            ["m132"] = Build(new UpgradeModel
            {
                Id = "m132",
                Name = "Cascas Comestíveis",
                Image = "images/icons/upgrades/m132.png",
                TechId = "t131",
                Range = 3,
                CostCode = UpgradeCostCode.Quantidade1C2,
            }),
            ["m133"] = Build(new UpgradeModel
            {
                Id = "m133",
                Name = "Água Forte",
                Image = "images/icons/upgrades/m133.png",
                TechId = "t13",
                Range = 4,
                CostCode = UpgradeCostCode.Quantidade1C2,
            }),
            ["m134"] = Build(new UpgradeModel
            {
                Id = "m134",
                Name = "Boias de Segurança",
                Image = "images/icons/upgrades/m134.png",
                TechId = "t13",
                Range = 5,
                CostCode = UpgradeCostCode.Quantidade1C1,
            }),
            #endregion
            #region t14
            ["m140"] = Build(new UpgradeModel
            {
                Id = "m140",
                Name = "Ingredientes Frescos",
                Image = "images/icons/upgrades/m140.png",
                TechId = "t14",
                Range = 2,
                CostCode = UpgradeCostCode.Quantidade1C4,
            }),
            ["m141"] = Build(new UpgradeModel
            {
                Id = "m121",
                Name = "Caça de Cerdos",
                Image = "images/icons/upgrades/m141.png",
                TechId = "t14",
                Range = 2,
                CostCode = UpgradeCostCode.Quantidade1C4,
            }),
            ["m142"] = Build(new UpgradeModel
            {
                Id = "m142",
                Name = "Cintos Utilitários",
                Image = "images/icons/upgrades/m142.png",
                TechId = "t141",
                Range = 3,
                CostCode = UpgradeCostCode.Quantidade1C3,
            }),
            ["m143"] = Build(new UpgradeModel
            {
                Id = "m143",
                Name = "Armadilhas para Grandes Presas",
                Image = "images/icons/upgrades/m143.png",
                TechId = "t14",
                Range = 3,
                CostCode = UpgradeCostCode.Quantidade1C3,
            }),
            ["m144"] = Build(new UpgradeModel
            {
                Id = "m144",
                Name = "Rotina de Caça",
                Image = "images/icons/upgrades/m144.png",
                TechId = "t14",
                Range = 1,
                CostCode = UpgradeCostCode.Tempo1C4,
            }),
            #endregion
            #region x01
            ["mx10"] = Build(new UpgradeModel
            {
                Id = "mx10",
                Name = "Conhecimento da Equipe 1",
                Image = "images/icons/upgrades/mx10.png",
                TechId = "x01",
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1T1,
            }),
            ["mx11"] = Build(new UpgradeModel
            {
                Id = "mx11",
                Name = "Colaboração da Equipe 1",
                Image = "images/icons/upgrades/mx11.png",
                TechId = "x01",
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1T1,
            }),
            ["mx12"] = Build(new UpgradeModel
            {
                Id = "mx12",
                Name = "Eficiência da Base 1",
                Image = "images/icons/upgrades/mx12.png",
                TechId = "x01",
                Range = 1,
                CostCode = UpgradeCostCode.Geracao1T1,
            }),
            #endregion
            #region x02
            ["mx20"] = Build(new UpgradeModel
            {
                Id = "mx20",
                Name = "Conhecimento da Equipe 2",
                Image = "images/icons/upgrades/mx20.png",
                TechId = "x02",
                Range = 2,
                CostCode = UpgradeCostCode.Quantidade1T1,
            }),
            ["mx11"] = Build(new UpgradeModel
            {
                Id = "mx11",
                Name = "Colaboração da Equipe 1",
                Image = "images/icons/upgrades/mx11.png",
                TechId = "x01",
                Range = 2,
                CostCode = UpgradeCostCode.Quantidade1T1,
            }),
            ["mx12"] = Build(new UpgradeModel
            {
                Id = "mx12",
                Name = "Eficiência da Base 1",
                Image = "images/icons/upgrades/mx12.png",
                TechId = "x01",
                Range = 2,
                CostCode = UpgradeCostCode.Geracao1T1,
            }),
            #endregion
        };

        private static UpgradeModel Build(UpgradeModel m)
        {
            var (res, @base, growth) = UpgradeCostMap.Get(m.CostCode);
            m.CostResourceId = res;
            m.CostBase = @base;
            m.CostGrowth = growth;
            m.Unlocked = false;
            return m;
        }
        public static void PopulateOrderFromAll()
        {
            var ids = All.Keys.OrderBy(id => id, StringComparer.Ordinal);
            Order.Clear();
            Order.AddRange(ids);
        }

        public static UpgradeModel GetDef(string id)
        {
            var up = All[id];
            return new UpgradeModel
            {
                Id = up.Id,
                Name = up.Name,
                Image = up.Image,
                Unlocked = up.Unlocked,
                TechId = up.TechId,
                Range = up.Range,
                CostCode = up.CostCode,
                CostResourceId = up.CostResourceId,
                CostBase = up.CostBase,
                CostGrowth = up.CostGrowth
            };
        }

        public static Dictionary<string, UpgradeModel> CreateInitialUpgrades()
        {
            PopulateOrderFromAll();

            var CoinsCollection = new Dictionary<string, UpgradeModel>(capacity: All.Count);
            foreach (var id in Order)
            {
                if (!All.TryGetValue(id, out var up)) continue;
                if (!up.Unlocked) continue;

                CoinsCollection[id] = new UpgradeModel
                {
                    Id = up.Id,
                    Name = up.Name,
                    Image = up.Image,
                    Unlocked = up.Unlocked,
                    TechId = up.TechId,
                    Range = up.Range,
                    CostCode = up.CostCode,
                    CostResourceId = up.CostResourceId,
                    CostBase = up.CostBase,
                    CostGrowth = up.CostGrowth
                };
            }
            return CoinsCollection;
        }
    }
}
