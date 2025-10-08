using FurmaIdle.Models;
using System.Linq;
using System.Reflection.Emit;
using static FurmaIdle.Data.UpgradeCostEnum;

namespace FurmaIdle.Data
{
    public static partial class UpgradeData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> Order = new();

        internal static readonly Dictionary<string, UpgradeModel> All = new()
        {
            #region t10
            ["m100"] = Build(new UpgradeModel
            {
                Id = "m100",
                Name = "Utensílios da Guilda",
                Image = "images/icons/upgrades/m100.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t10",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1C1,
                Effects = new(){
                    new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c10", Value=1.10, Op=EffectOp.Multiplicative }
                }
            }),
            ["m101"] = Build(new UpgradeModel
            {
                Id = "m101",
                Name = "Disciplina da Guilda",
                Image = "images/icons/upgrades/m101.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t10",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1C1,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c11", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m102"] = Build(new UpgradeModel
            {
                Id = "m102",
                Name = "Coletividade da Guilda",
                Image = "images/icons/upgrades/m102.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t10",
                Avaliable = false,
                Range = 2,
                CostCode = UpgradeCostCode.Quantidade1C1,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c11", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m103"] = Build(new UpgradeModel
            {
                Id = "m103",
                Name = "União da Guilda",
                Image = "images/icons/upgrades/m103.png",
                Description = "Aumenta a Geração do Recurso em 0.1/s",
                Lore = "",
                Unlocked = false,
                TechId = "t10",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.Geracao1R1,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ResourceGen, ScopeId="r100", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m104"] = Build(new UpgradeModel
            {
                Id = "m104",
                Name = "Escritório da Guilda",
                Image = "images/icons/upgrades/m104.png",
                Description = "Aumenta o Limite de Contratos por Personagem",
                Lore = "",
                Unlocked = false,
                TechId = "t10",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.LimiteContrato1T1,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractCap, ScopeId="all", Value=1, Op=EffectOp.Additive }
    }
            }),
            #endregion
            #region t11
            ["m110"] = Build(new UpgradeModel
            {
                Id = "m110",
                Name = "Facas de Escamar",
                Image = "images/icons/upgrades/m110.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t11",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1C2,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c21", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m111"] = Build(new UpgradeModel
            {
                Id = "m111",
                Name = "Reservas de Escambo",
                Image = "images/icons/upgrades/m111.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t11",
                Avaliable = false,
                Range = 2,
                CostCode = UpgradeCostCode.Quantidade1C1,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c10", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m112"] = Build(new UpgradeModel
            {
                Id = "m112",
                Name = "Cordas Grossas",
                Image = "images/icons/upgrades/m112.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t11",
                Avaliable = false,
                Range = 3,
                CostCode = UpgradeCostCode.Quantidade1C1,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c11", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m113"] = Build(new UpgradeModel
            {
                Id = "m113",
                Name = "Água Aproveitada",
                Image = "images/icons/upgrades/m113.png",
                Description = "Diminui o Tempo de execução do Contrato em x0.9",
                Lore = "",
                Unlocked = false,
                TechId = "t11",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.Tempo1C1,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractTime, ScopeId="c10", Value=0.90, Op=EffectOp.Multiplicative }
    }
            }),
            ["m114"] = Build(new UpgradeModel
            {
                Id = "m114",
                Name = "Armazéns Salinos",
                Image = "images/icons/upgrades/m114.png",
                Description = "Diminui o Tempo de execução do Contrato em x0.9",
                Lore = "",
                Unlocked = false,
                TechId = "t11",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.Tempo1C1,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractTime, ScopeId="c11", Value=0.90, Op=EffectOp.Multiplicative }
    }
            }),
            #endregion
            #region t12
            ["m120"] = Build(new UpgradeModel
            {
                Id = "m120",
                Name = "Manutenção Coletiva",
                Image = "images/icons/upgrades/m120.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t12",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1C3,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c31", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m121"] = Build(new UpgradeModel
            {
                Id = "m121",
                Name = "Rotina Rígida",
                Image = "images/icons/upgrades/m121.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t12",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1C3,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c32", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m122"] = Build(new UpgradeModel
            {
                Id = "m122",
                Name = "Necessidades Ocultas",
                Image = "images/icons/upgrades/m122.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t12",
                Avaliable = false,
                Range = 2,
                CostCode = UpgradeCostCode.Quantidade1C2,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c20", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m123"] = Build(new UpgradeModel
            {
                Id = "m123",
                Name = "União de Moradores",
                Image = "images/icons/upgrades/m123.png",
                Description = "Diminui o Tempo de execução do Contrato em x0.9",
                Lore = "",
                Unlocked = false,
                TechId = "t12",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.Tempo1C3,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractTime, ScopeId="c31", Value=0.90, Op=EffectOp.Multiplicative }
    }
            }),
            ["m124"] = Build(new UpgradeModel
            {
                Id = "m124",
                Name = "Vigilância Constante",
                Image = "images/icons/upgrades/m124.png",
                Description = "Diminui o Tempo de execução do Contrato em x0.9",
                Lore = "",
                Unlocked = false,
                TechId = "t12",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.Tempo1C3,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractTime, ScopeId="c32", Value=0.90, Op=EffectOp.Multiplicative }
    }
            }),
            #endregion
            #region t13
            ["m130"] = Build(new UpgradeModel
            {
                Id = "m130",
                Name = "Lascas Naturais",
                Image = "images/icons/upgrades/m130.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t13",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1C4,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c41", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m131"] = Build(new UpgradeModel
            {
                Id = "m131",
                Name = "Piscinas Naturais",
                Image = "images/icons/upgrades/m131.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t13",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1C4,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c42", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m132"] = Build(new UpgradeModel
            {
                Id = "m132",
                Name = "Cascas Comestíveis",
                Image = "images/icons/upgrades/m132.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t13",
                Avaliable = false,
                Range = 3,
                CostCode = UpgradeCostCode.Quantidade1C2,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c21", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m133"] = Build(new UpgradeModel
            {
                Id = "m133",
                Name = "Água Forte",
                Image = "images/icons/upgrades/m133.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t13",
                Avaliable = false,
                Range = 4,
                CostCode = UpgradeCostCode.Quantidade1C2,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c22", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m134"] = Build(new UpgradeModel
            {
                Id = "m134",
                Name = "Boias de Segurança",
                Image = "images/icons/upgrades/m134.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t13",
                Avaliable = false,
                Range = 5,
                CostCode = UpgradeCostCode.Quantidade1C1,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c11", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            #endregion
            #region t14
            ["m140"] = Build(new UpgradeModel
            {
                Id = "m140",
                Name = "Ingredientes Frescos",
                Image = "images/icons/upgrades/m140.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t14",
                Avaliable = false,
                Range = 2,
                CostCode = UpgradeCostCode.Quantidade1C4,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c40", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m141"] = Build(new UpgradeModel
            {
                Id = "m121",
                Name = "Caça de Cerdos",
                Image = "images/icons/upgrades/m141.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t14",
                Avaliable = false,
                Range = 2,
                CostCode = UpgradeCostCode.Quantidade1C4,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c44", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m142"] = Build(new UpgradeModel
            {
                Id = "m142",
                Name = "Cintos Utilitários",
                Image = "images/icons/upgrades/m142.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t141",
                Avaliable = false,
                Range = 3,
                CostCode = UpgradeCostCode.Quantidade1C3,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c31", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m143"] = Build(new UpgradeModel
            {
                Id = "m143",
                Name = "Armadilhas para Grandes Presas",
                Image = "images/icons/upgrades/m143.png",
                Description = "Aumenta o Ganho do Contrato em x1.1",
                Lore = "",
                Unlocked = false,
                TechId = "t14",
                Avaliable = false,
                Range = 3,
                CostCode = UpgradeCostCode.Quantidade1C3,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="c32", Value=1.10, Op=EffectOp.Multiplicative }
    }
            }),
            ["m144"] = Build(new UpgradeModel
            {
                Id = "m144",
                Name = "Rotina de Caça",
                Image = "images/icons/upgrades/m144.png",
                Description = "Diminui o Tempo de execução do Contrato em x0.9",
                Lore = "",
                Unlocked = false,
                TechId = "t14",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.Tempo1C4,
                Effects = new(){
        new UpgradeEffectModel { Target=EffectTarget.ContractTime, ScopeId="c44", Value=0.90, Op=EffectOp.Multiplicative }
    }
            }),
            #endregion
            #region x01
            ["mx10"] = Build(new UpgradeModel
            {
                Id = "mx10",
                Name = "Conhecimento da Equipe 1",
                Image = "images/icons/upgrades/mx10.png",
                Description = "Aumenta o Ganho de todos os Contratos em x1.15 por Tecnologia Desbloqueada",
                Lore = "",
                Unlocked = false,
                TechId = "x01",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1T1,
                Effects = new(){
                    new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="all", Value=1.15, Op=EffectOp.Multiplicative }
                }
            }),
            ["mx11"] = Build(new UpgradeModel
            {
                Id = "mx11",
                Name = "Colaboração da Equipe 1",
                Image = "images/icons/upgrades/mx11.png",
                Description = "Aumenta o Ganho de todos os Contratos em x1.15 por Personagem Contratado",
                Lore = "",
                Unlocked = false,
                TechId = "x01",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.Quantidade1T1,
                Effects = new(){
                    new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="all", Value=1.15, Op=EffectOp.Multiplicative }
                }
            }),
            ["mx12"] = Build(new UpgradeModel
            {
                Id = "mx12",
                Name = "Eficiência da Base 1",
                Image = "images/icons/upgrades/mx12.png",
                Description = "Aumenta o Ganho de todos os Contratos em x1.15 por Personagem Contratado",
                Lore = "",
                Unlocked = false,
                TechId = "x01",
                Avaliable = false,
                Range = 1,
                CostCode = UpgradeCostCode.Geracao1T1,
                Effects = new(){
                    new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="all", Value=1.15, Op=EffectOp.Multiplicative }
                }
            }),
            #endregion
            #region x02
            ["mx20"] = Build(new UpgradeModel
            {
                Id = "mx20",
                Name = "Conhecimento da Equipe 2",
                Image = "images/icons/upgrades/mx20.png",
                Description = "Aumenta o Ganho de todos os Contratos em x1.15 por Tecnologia Desbloqueada",
                Lore = "",
                Unlocked = false,
                TechId = "x02",
                Avaliable = false,
                Range = 2,
                CostCode = UpgradeCostCode.Quantidade1T1,
                Effects = new(){
                    new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="all", Value=1.15, Op=EffectOp.Multiplicative }
                }
            }),
            ["mx21"] = Build(new UpgradeModel
            {
                Id = "mx21",
                Name = "Colaboração da Equipe 1",
                Image = "images/icons/upgrades/mx11.png",
                Description = "Aumenta o Ganho de todos os Contratos em x1.15 por Personagem Contratado",
                Lore = "",
                Unlocked = false,
                TechId = "x01",
                Avaliable = false,
                Range = 2,
                CostCode = UpgradeCostCode.Quantidade1T1,
                Effects = new(){
                    new UpgradeEffectModel { Target=EffectTarget.ContractGain, ScopeId="all", Value=1.15, Op=EffectOp.Multiplicative }
                }
            }),
            ["mx22"] = Build(new UpgradeModel
            {
                Id = "mx22",
                Name = "Eficiência da Base 1",
                Image = "images/icons/upgrades/mx12.png",
                Description = "Aumenta a Geração de Recursos em 0.1/s por Personagem na Base",
                Lore = "",
                Unlocked = false,
                TechId = "x01",
                Avaliable = false,
                Range = 2,
                CostCode = UpgradeCostCode.Geracao1T1,
                Effects = new(){
                    new UpgradeEffectModel { Target=EffectTarget.ResourceGen, ScopeId="all", Value=0.10, Op=EffectOp.Additive }
                }
            }),
            #endregion
        };

        private static UpgradeModel Build(UpgradeModel m)
        {
            var (res, @base, growth) = UpgradeCostMap.Get(m.CostCode);
            m.CostResourceId = res;
            m.CostBase = @base;
            m.CostGrowth = growth;
            return m;
        }

        public static void PopulateOrderFromAll()
        {
            Order.Clear();
            IEnumerable<string> keys = (All == null)
                ? Enumerable.Empty<string>()
                : All.Keys.AsEnumerable();

            Order.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
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
                Avaliable = up.Avaliable,
                Range = up.Range,
                CostCode = up.CostCode,
                CostResourceId = up.CostResourceId,
                CostBase = up.CostBase,
                CostGrowth = up.CostGrowth,
                Description = up.Description,
                Lore = up.Lore,
                Effects = up.Effects
            };
        }

        public static Dictionary<string, UpgradeModel> CreateInitialUpgrades()
        {
            var CoinsCollection = new Dictionary<string, UpgradeModel>(capacity: All.Count);

            if (Order.Count == 0) PopulateOrderFromAll();

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
                    Avaliable = up.Avaliable,
                    Range = up.Range,
                    CostCode = up.CostCode,
                    CostResourceId = up.CostResourceId,
                    CostBase = up.CostBase,
                    CostGrowth = up.CostGrowth,
                    Description = up.Description,
                    Lore = up.Lore,
                    Effects = up.Effects
                };
            }
            return CoinsCollection;
        }
    }
}
