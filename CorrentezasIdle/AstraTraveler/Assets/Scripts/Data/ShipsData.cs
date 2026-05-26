using System.Collections.Generic;

public static class ShipData
{
    public static Dictionary<string, ShipModel> All = new();

    public static void Load()
    {
        All.Clear();

        var rows = CSVLoaderService.Load("Data/Ships");

        foreach (var row in rows)
        {
            ShipModel model = new();

            model.Id = row["Id"];

            model.NameEN = row["NameEN"];
            model.NamePT = row["NamePT"];
            model.DescriptionEN = row["DescriptionEN"];
            model.DescriptionPT = row["DescriptionPT"];

            model.Life = ParseHelper.Int(row["Life"]);
            model.Speed = ParseHelper.Int(row["Speed"]);
            model.Armor = ParseHelper.Int(row["Armor"]);
            model.Resistence = ParseHelper.Int(row["Resistence"]);

            model.Size = ParseHelper.Int(row["Size"]);
            model.Tripulation = ParseHelper.Int(row["Tripulation"]);

            model.WeaponSlots = ParseHelper.Int(row["WeaponSlots"]);

            model.UnlockId = row["UnlockId"];
            model.UnlockStatus = ParseHelper.Enum<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}