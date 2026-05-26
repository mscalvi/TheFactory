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

            model.Type = ParseHelper.Enum<WeaponHelper.AmmoType>(row["Type"]);

            model.Damage = ParseHelper.Double(row["Damage"]);

            model.Recharge = ParseHelper.Double(row["Recharge"]);

            model.Ammount = ParseHelper.Int(row["Ammount"]);

            model.Special = ParseHelper.Enum<WeaponHelper.SpecialType>(row["Special"]);

            model.ProjectileSpeed = ParseHelper.Float(row["ProjectileSpeed"]);

            model.UnlockId = row["UnlockId"];

            model.UnlockStatus = ParseHelper.Enum<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}