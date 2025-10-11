using System.Collections.Generic;
using FurmaIdle.Models;

namespace FurmaIdle.Data
{
    public static class TraitData
    {
        public static readonly Dictionary<string, TraitModel> All = new()
        {
            ["tr00"] = new TraitModel
            {
                Id = "tr00",
                Name = "Conhecimento Cultural",
                KnowledgeId = "k10",
                GainMult = 1.05
            },
            ["tr01"] = new TraitModel
            {
                Id = "tr01",
                Name = "Conhecimento de Sobrevivência",
                KnowledgeId = "k12",
                GainMult = 1.05
            },
            ["tr02"] = new TraitModel
            {
                Id = "tr02",
                Name = "Conhecimento Geográfico",
                KnowledgeId = "k11",
                GainMult = 1.05
            },
            ["tr03"] = new TraitModel
            {
                Id = "tr03",
                Name = "Reduz Custo de Contratação",
                CharacterCostMult = 0.95
            },
            ["tr04"] = new TraitModel
            {
                Id = "tr04",
                Name = "Gera Mantimentos",
                ResourceId = "r100",
                AddPerSecond = 0.5
            },
        };

        public static TraitModel GetDef(string id) => All[id];
    }
}
