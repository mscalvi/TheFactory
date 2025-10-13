using FurmaIdle.Models;

namespace FurmaIdle.Data
{
    public class ExpansionData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> Order = new()
        {
            "x00", "x01"
        };

        internal static readonly Dictionary<string, ExpansionModel> All = new()
        {
            ["x00"] = new ExpansionModel
            {
                Id = "x00",
                Name = "Expansão 1",
                Image = "images/icons/expansions/x00.jpg",
                CoinCostId = "r001",
                Cost = 500000,
                Unlocked = true,
                Avaliable = false,
                DestUnlockId = "d02"
            },

            ["x01"] = new ExpansionModel
            {
                Id = "x01",
                Name = "Expansão 2",
                Image = "images/icons/expansions/x01.jpg",
                CoinCostId = "r001",
                Cost = 50000000,
                Unlocked = true,
                Avaliable = false,
                DestUnlockId = "d03"
            }
        };

        public static ExpansionModel GetDef(string id)
        {
            var expansion = All[id];
            return new ExpansionModel
            {
                Id = expansion.Id,
                Name = expansion.Name,
                Image = expansion.Image,
                CoinCostId = expansion.CoinCostId,
                Cost = expansion.Cost,
                Unlocked = expansion.Unlocked,
                Avaliable = expansion.Avaliable,
                DestUnlockId = expansion.DestUnlockId
            };
        }

        public static Dictionary<string, ExpansionModel> CreateInitialStages()
        {
            var dictionary = new Dictionary<string, ExpansionModel>(All.Count);
            foreach (var id in Order)
            {
                if (!All.TryGetValue(id, out var expansion)) continue;

                dictionary[id] = new ExpansionModel
                {
                    Id = expansion.Id,
                    Name = expansion.Name,
                    Image = expansion.Image,
                    CoinCostId = expansion.CoinCostId,
                    Cost = expansion.Cost,
                    Unlocked = expansion.Unlocked,
                    Avaliable = expansion.Avaliable,
                    DestUnlockId = expansion.DestUnlockId
                };
            }
            return dictionary;
        }
    }
}
