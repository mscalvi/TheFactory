using System.Collections.Generic;
using FurmaIdle.Models;

namespace FurmaIdle.Data
{
    public static class ResourceData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> Order = new() { "r001", "r100" };
        public static readonly List<string> CoinsOrder = new() { "r001" };
        public static readonly List<string> ResourcesOrder = new() { "r100" };

        internal static readonly Dictionary<string, ResourceModel> All = new()
        {
            ["r001"] = new ResourceModel
            {
                Id = "r001",
                Name = "Talho",
                Image = "images/icons/resources/r001.png",
                Unlocked = true,
                Total = 0,
                Actual = 0,
                PerSecond = 0,
                Type = "coin",
                Sort = 1
            },

            ["r100"] = new ResourceModel
            {
                Id = "r100",
                Name = "Mantimentos",
                Image = "images/icons/resources/r100.png",
                Unlocked = true,
                Total = 0,
                Actual = 0,
                PerSecond = 0,
                Type = "resource",
                Sort = 1
            }
        };

        public static ResourceModel GetDef(string id)
        {
            var coin = All[id];
            return new ResourceModel
            {
                Id = coin.Id,
                Name = coin.Name,
                Image = coin.Image,
                Unlocked = coin.Unlocked,
                Total = coin.Total,
                Actual = coin.Actual,
                PerSecond = coin.PerSecond,
                Type = coin.Type,
                Sort = coin.Sort
            };
        }

        public static Dictionary<string, ResourceModel> CreateInitialResources()
        {
            var CoinsCollection = new Dictionary<string, ResourceModel>(capacity: All.Count);
            foreach (var id in Order)
            {
                if (!All.TryGetValue(id, out var coin)) continue;
                if (!coin.Unlocked) continue;

                CoinsCollection[id] = new ResourceModel
                {
                    Id = coin.Id,
                    Name = coin.Name,
                    Image = coin.Image,
                    Unlocked = coin.Unlocked,
                    Total = coin.Total,
                    Actual = coin.Actual,
                    PerSecond = coin.PerSecond,
                    Type = coin.Type,
                    Sort = coin.Sort
                };
            }
            return CoinsCollection;
        }
    }
}
