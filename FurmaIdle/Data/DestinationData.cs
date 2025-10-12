// Data/DestinationData.cs
using System.Collections.Generic;
using FurmaIdle.Models;

namespace FurmaIdle.Data
{
    public static class DestinationData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> Order = new()
        {
            "d00", "d01", "d02", "d03"
        };

        // Catálogo imutável em runtime. Preencha com seus destinos reais.
        internal static readonly Dictionary<string, DestinationModel> All = new()
        {
            ["d00"] = new DestinationModel
            {
                 Id = "d00",
                 Name = "Murada de Cairu",
                 Cost = 0,
                 CostResourceId = "r001",
                 Image = "images/icons/destinations/d00.png",
                 Unlocked = true,
                 Avaliable = false,
                 StageId = "s00"
            },

            ["d01"] = new DestinationModel
            {
                Id = "d01",
                Name = "Pontas Cantarolantes",
                Cost = 80000,
                CostResourceId = "r001",
                Image = "images/icons/destinations/d01.png",
                Unlocked = false,
                Avaliable = true,
                StageId = "s00"
            },

            ["d02"] = new DestinationModel
            {
                Id = "d02",
                Name = "Coração da Ilha",
                Cost = 160000,
                CostResourceId = "r001",
                Image = "images/icons/destinations/d02.png",
                Unlocked = false,
                Avaliable = true,
                StageId = "s00"
            },

            ["d03"] = new DestinationModel
            {
                Id = "d03",
                Name = "Bosque da Raposa",
                Cost = 1000000,
                CostResourceId = "r001",
                Image = "images/icons/destinations/d03.png",
                Unlocked = false,
                Avaliable = true,
                StageId = "s00"
            }
        };

        // Retorna um clone defensivo da definição (evita mutação acidental do catálogo)
        public static DestinationModel GetDef(string id)
        {
            var d = All[id];
            return new DestinationModel
            {
                Id = d.Id,
                Name = d.Name,
                Cost = d.Cost,
                CostResourceId = d.CostResourceId,
                Image = d.Image,
                Unlocked = d.Unlocked,
                Avaliable = d.Avaliable,
                StageId = d.StageId
            };
        }

        // Criação do dicionário inicial de jogo com base em Order e flag Unlocked
        public static Dictionary<string, DestinationModel> CreateInitialDestinations()
        {
            var dict = new Dictionary<string, DestinationModel>(All.Count);
            foreach (var id in Order)
            {
                if (!All.TryGetValue(id, out var d)) continue;

                dict[id] = new DestinationModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Cost = d.Cost,
                    CostResourceId = d.CostResourceId,
                    Image = d.Image,
                    Unlocked = d.Unlocked,
                    Avaliable = d.Avaliable,
                    StageId = d.StageId
                };
            }
            return dict;
        }

        // Helpers análogos ao GetResourceId do StageData
        public static string GetCostResourceId(string destinationId)
            => All.TryGetValue(destinationId, out var d) ? d.CostResourceId : "r001";

        public static string GetStageId(string destinationId)
            => All.TryGetValue(destinationId, out var d) ? d.StageId : "s00";
    }
}
