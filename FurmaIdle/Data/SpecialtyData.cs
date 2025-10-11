using System.Collections.Generic;
using FurmaIdle.Models;

namespace FurmaIdle.Data
{
    public static class SpecialtyData
    {
        public static readonly Dictionary<string, SpecialtyModel> All = new()
        {
            ["e00"] = new SpecialtyModel
            {
                Id = "e00",
                Name = "Produção Instantânea",
                Target = SpecialtyTarget.Coins,  
                Value = 1.0,                      
                DurationSec = 0,                
                CostResourceId = "r100",
                Cost = 10
            },
            ["e01"] = new SpecialtyModel
            {
                Id = "e01",
                Name = "Melhora Geração de Provisões",
                Target = SpecialtyTarget.Resources,
                Value = 1.2,
                DurationSec = 20,
                CostResourceId = "r100",
                Cost = 15,
                ResourceIdScope = "r100"
            },
            ["e02"] = new SpecialtyModel
            {
                Id = "e02",
                Name = "Melhora Produção",
                Target = SpecialtyTarget.Coins,
                Value = 2.0,
                DurationSec = 20,
                CostResourceId = "r100",
                Cost = 30
            },
            ["e03"] = new SpecialtyModel
            {
                Id = "e03",
                Name = "Diminui Consumo de Provisões",
                Target = SpecialtyTarget.Resources,
                Value = 0.8,
                DurationSec = 20,
                CostResourceId = "r100",
                Cost = 10,
                ResourceIdScope = "r100"
            },
        };

        public static SpecialtyModel GetDef(string id) => All[id];
    }
}
