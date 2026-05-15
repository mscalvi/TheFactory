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

            model.Type = System.Enum.Parse<TripulationHelper.Type>(row["Type"]);

            model.Rarity = System.Enum.Parse<GameHelper.ItemRarity>(row["Rarity"]);

            model.Str = int.Parse(row["Str"]);
            model.Dex = int.Parse(row["Dex"]);
            model.Int = int.Parse(row["Int"]);
            model.Luk = int.Parse(row["Luk"]);
            model.Cha = int.Parse(row["Cha"]);
            model.Con = int.Parse(row["Con"]);

            model.UnlockId = row["UnlockId"];
            model.UnlockStatus = System.Enum.Parse<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}