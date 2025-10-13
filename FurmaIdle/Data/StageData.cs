// Data/StageData.cs
using FurmaIdle.Models;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace FurmaIdle.Data
{
    public static class StageData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> Order = new()
        {
            "s00", "s01"
        };

        internal static readonly Dictionary<string, StageModel> All = new()
        {
            ["s00"] = new StageModel
            {
                Id = "s00",
                Name = "Ilha de Vera",
                Image = "images/icons/stages/s00.jpg",
                ClickImage = "images/stages/s00.jpg",
                ResourceId = "r001",
                Unlocked = true,
                Avaliable = false,
                DestinationId = "d00",
                Sort = 1,
                PartyCap = 4,
                ContractsSlots = 5
            },

            ["s01"] = new StageModel
            {
                Id = "s01",
                Name = "As Correntezas",
                Image = "images/icons/stages/s01.jpg",
                ClickImage = "images/stages/s01.jpg",
                ResourceId = "r001",
                Unlocked = true,
                Avaliable = false,
                DestinationId = "d10",
                Sort = 2,
                PartyCap = 2,
                ContractsSlots = 2
            }
        };

        public static StageModel GetDef(string id)
        {
            var stage = All[id];
            return new StageModel
            {
                Id = stage.Id,
                Name = stage.Name,
                Image = stage.Image,
                ClickImage = stage.ClickImage,
                ResourceId = stage.ResourceId,
                Unlocked = stage.Unlocked,
                Avaliable = stage.Avaliable,
                Sort = stage.Sort,
                PartyCap = stage.PartyCap,
                ContractsSlots = stage.ContractsSlots
            };
        }

        public static Dictionary<string, StageModel> CreateInitialStages()
        {
            var dictionary = new Dictionary<string, StageModel>(All.Count);
            foreach (var id in Order)
            {
                if (!All.TryGetValue(id, out var stage)) continue;

                dictionary[id] = new StageModel
                {
                    Id = stage.Id,
                    Name = stage.Name,
                    Image = stage.Image,
                    ClickImage = stage.ClickImage,
                    ResourceId = stage.ResourceId,
                    Unlocked = stage.Unlocked,
                    Avaliable = stage.Avaliable,
                    Sort = stage.Sort,
                    PartyCap = stage.PartyCap,
                    ContractsSlots = stage.ContractsSlots
                };
            }
            return dictionary;
        }

        public static string GetResourceId(string stageId)
            => All.TryGetValue(stageId, out var s) ? s.ResourceId : "r001";
    }
}
