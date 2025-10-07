// Data/StageData.cs
using System.Collections.Generic;
using FurmaIdle.Models;

namespace FurmaIdle.Data
{
    public static class StageData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> Order = new()
        {
            "s00"
        };

        internal static readonly Dictionary<string, StageModel> All = new()
        {
            ["s00"] = new StageModel
            {
                Id = "s00",
                Name = "Stage Inicial",
                Image = "images/d00.jpg",
                ResourceId = "r01",
                Unlocked = true,
                Sort = 1
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
                ResourceId = stage.ResourceId,
                Unlocked = stage.Unlocked,
                Sort = stage.Sort
            };
        }

        public static Dictionary<string, StageModel> CreateInitialStages()
        {
            var dictionary = new Dictionary<string, StageModel>(All.Count);
            foreach (var id in Order)
            {
                if (!All.TryGetValue(id, out var stage) || !stage.Unlocked) continue;

                dictionary[id] = new StageModel
                {
                    Id = stage.Id,
                    Name = stage.Name,
                    Image = stage.Image,
                    ResourceId = stage.ResourceId,
                    Unlocked = stage.Unlocked,
                    Sort = stage.Sort
                };
            }
            return dictionary;
        }

        public static string GetResourceId(string stageId)
            => All.TryGetValue(stageId, out var s) ? s.ResourceId : "r01";
    }
}
