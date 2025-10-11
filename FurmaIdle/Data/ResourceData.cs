using System.Collections.Generic;
using FurmaIdle.Helpers;
using FurmaIdle.Models;

namespace FurmaIdle.Data
{
    public static class ResourceData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> Order = new() { "r001", "r100" };

        internal static readonly Dictionary<string, ResourceModel> All = new()
        {
            ["r001"] = new ResourceModel
            {
                Id = "r001",
                Name = "Talho",
                Image = "images/icons/resources/r001.png",
                Unlocked = true,
                Avaliable = true,
                Total = 0,
                Actual = 0,
                PerSecond = 0,
                ResourceType = ResourceEnum.ResourceType.Coin,
                Persistence = ResetPersistenceEnum.ResetPersistence.ExpeditionOnly,
                UpgUnlockId = null,
                CharacterCap = 0
            },

            ["r100"] = new ResourceModel
            {
                Id = "r100",
                Name = "Mantimentos",
                Image = "images/icons/resources/r100.png",
                Unlocked = false,
                Avaliable = false,
                Total = 0,
                Actual = 0,
                PerSecond = 0,
                ResourceType = ResourceEnum.ResourceType.Resource,
                Persistence = ResetPersistenceEnum.ResetPersistence.ExpeditionOnly,
                UpgUnlockId = "mx02",
                CharacterCap = 50
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
                Avaliable = coin.Avaliable,
                Total = coin.Total,
                Actual = coin.Actual,
                PerSecond = coin.PerSecond,
                ResourceType = coin.ResourceType,
                Persistence = coin.Persistence,
                UpgUnlockId = coin.UpgUnlockId,
                CharacterCap = coin.CharacterCap,
            };
        }

        public static Dictionary<string, ResourceModel> CreateInitialResources()
        {
            var CoinsCollection = new Dictionary<string, ResourceModel>(All.Count);
            foreach (var id in Order)
            {
                if (!All.TryGetValue(id, out var coin)) continue;

                CoinsCollection[id] = new ResourceModel
                {
                    Id = coin.Id,
                    Name = coin.Name,
                    Image = coin.Image,
                    Unlocked = coin.Unlocked,
                    Avaliable = coin.Avaliable,
                    Total = coin.Total,
                    Actual = coin.Actual,
                    PerSecond = coin.PerSecond,
                    ResourceType = coin.ResourceType,
                    Persistence = coin.Persistence,
                    UpgUnlockId = coin.UpgUnlockId,
                    CharacterCap = coin.CharacterCap,
                };
            }
            return CoinsCollection;
        }
    }
}
