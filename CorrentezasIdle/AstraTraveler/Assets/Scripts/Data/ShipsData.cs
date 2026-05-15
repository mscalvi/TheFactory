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

            model.Life = int.Parse(row["Life"]);
            model.Speed = int.Parse(row["Speed"]);
            model.Armor = int.Parse(row["Armor"]);
            model.Resistence = int.Parse(row["Resistence"]);

            model.Size = int.Parse(row["Size"]);
            model.Tripulation = int.Parse(row["Tripulation"]);

            model.WeaponSlots = int.Parse(row["WeaponSlots"]);

            model.UnlockId = row["UnlockId"];
            model.UnlockStatus = System.Enum.Parse<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}