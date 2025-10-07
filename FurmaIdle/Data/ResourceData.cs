using System.Collections.Generic;
using FurmaIdle.Models;

namespace FurmaIdle.Data
{
    public static class ResourceData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> CoinsOrder = new()
        {
            "r01" // Talho            
        };

        public static readonly Dictionary<string, ResourceModel> All = new()
        {
            {
                "r01",
                new ResourceModel
                {
                    Id = "r01",
                    Name = "Talho",
                    Icon = "images/icons/r01.png",
                    Unlocked = true,
                    Total = 0,
                    Actual = 0,
                    PerSecond = 0,
                    Sort = 1
                }
            }
        };

        public static ResourceModel GetDef(string id)
        {
            var coin = All[id];
            return new ResourceModel
            {
                Id = coin.Id,
                Name = coin.Name,
                Icon = coin.Icon,
                Unlocked = coin.Unlocked,
                Total = coin.Total,
                Actual = coin.Actual,
                PerSecond = coin.PerSecond,
                Sort = coin.Sort
            };
        }

        public static Dictionary<string, ResourceModel> CreateInitialResources()
        {
            var CoinsCollection = new Dictionary<string, ResourceModel>(capacity: All.Count);
            foreach (var id in CoinsOrder)
            {
                if (!All.TryGetValue(id, out var coin)) continue;
                if (!coin.Unlocked) continue;

                CoinsCollection[id] = new ResourceModel
                {
                    Id = coin.Id,
                    Name = coin.Name,
                    Icon = coin.Icon,
                    Unlocked = coin.Unlocked,
                    Total = coin.Total,
                    Actual = coin.Actual,
                    PerSecond = coin.PerSecond,
                    Sort = coin.Sort
                };
            }
            return CoinsCollection;
        }
    }
}
