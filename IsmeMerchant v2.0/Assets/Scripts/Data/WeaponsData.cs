using System.Collections.Generic;

public static class WeaponsData
{
    public static Dictionary<string, WeaponModel> All = new();

    public static void Load()
    {
        All.Clear();

        var rows = CSVLoaderService.Load("Data/Weapons");

        foreach (var row in rows)
        {
            WeaponModel model = new();

            model.Id = row["Id"];

            model.NamePT = row["NamePT"];
            model.NameEN = row["NameEN"];

            model.DescriptionPT = row["DescriptionPT"];
            model.DescriptionEN = row["DescriptionEN"];

            model.Damage = ParseHelper.Double(row["Damage"]);
            model.Range = ParseHelper.Double(row["Range"]);
            model.AttackSpeed = ParseHelper.Double(row["AttackSpeed"]);
            model.Precision = ParseHelper.Double(row["Precision"]);
            model.CriticalDamage = ParseHelper.Double(row["CriticalDamage"]);

            model.Special = ParseHelper.Enum<WeaponHelper.SpecialType>(row["Special"]);

            model.Angle = ParseHelper.Int(row["Angle"]);
            model.AngleMin = ParseHelper.Int(row["AngleMin"]);
            model.AngleMax = ParseHelper.Int(row["AngleMax"]);

            model.AmmoType = ParseHelper.Enum<WeaponHelper.AmmoType>(row["AmmoType"]);

            model.UnlockId = row["UnlockId"];
            model.UnlockStatus = ParseHelper.Enum<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}