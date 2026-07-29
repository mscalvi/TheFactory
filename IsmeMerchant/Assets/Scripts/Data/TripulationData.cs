using System.Collections.Generic;

public static class TripulationData
{
    public static Dictionary<string, TripulationModel> All = new();

    public static void Load()
    {
        All.Clear();

        var rows = CSVLoaderService.Load("Data/Tripulations");

        foreach (var row in rows)
        {
            TripulationModel model = new();

            model.Id = row["Id"];

            model.Name = row["Name"];
            model.DescriptionEN = row["DescriptionEN"];
            model.DescriptionPT = row["DescriptionPT"];

            model.Type = ParseHelper.Enum<TripulationHelper.Type>(row["Type"]);

            model.Rarity = ParseHelper.Enum<GameHelper.ItemRarity>(row["Rarity"]);

            model.Str = ParseHelper.Int(row["Str"]);
            model.Dex = ParseHelper.Int(row["Dex"]);
            model.Int = ParseHelper.Int(row["Int"]);
            model.Luk = ParseHelper.Int(row["Luk"]);
            model.Cha = ParseHelper.Int(row["Cha"]);
            model.Con = ParseHelper.Int(row["Con"]);

            model.UnlockId = row["UnlockId"];
            model.UnlockStatus = ParseHelper.Enum<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}