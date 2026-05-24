using System.Collections.Generic;

public static class AmmoData
{
    public static Dictionary<string, AmmoModel> All = new();

    public static void Load()
    {
        All.Clear();

        var rows = CSVLoaderService.Load("Data/Ammos");

        foreach (var row in rows)
        {
            AmmoModel model = new();

            model.Id = row["Id"];

            model.NamePT = row["NamePT"];
            model.NameEN = row["NameEN"];

            model.DescriptionPT = row["DescriptionPT"];
            model.DescriptionEN = row["DescriptionEN"];

            model.ProjectileId = row["ProjectileId"];

            model.Type = System.Enum.Parse<WeaponHelper.AmmoType>(row["Type"]);

            model.Damage = double.Parse(row["Damage"]);

            model.Recharge = double.Parse(row["Recharge"]);

            model.Ammount = int.Parse(row["Ammount"]);

            model.Special = System.Enum.Parse<WeaponHelper.SpecialType>(row["Special"]);

            model.ProjectileSpeed = float.Parse(row["ProjectileSpeed"]);

            model.UnlockId = row["UnlockId"];

            model.UnlockStatus = System.Enum.Parse<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}