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

            model.Damage = double.Parse(row["Damage"]);
            model.Range = double.Parse(row["Range"]);
            model.AttackSpeed = double.Parse(row["AttackSpeed"]);
            model.Precision = double.Parse(row["Precision"]);
            model.CriticalDamage = double.Parse(row["CriticalDamage"]);

            model.Special = System.Enum.Parse<WeaponHelper.SpecialType>(row["Special"]);

            model.Angle = int.Parse(row["Angle"]);
            model.AngleMin = int.Parse(row["AngleMin"]);
            model.AngleMax = int.Parse(row["AngleMax"]);

            model.AmmoType = System.Enum.Parse<WeaponHelper.AmmoType>(row["AmmoType"]);

            model.UnlockId = row["UnlockId"];
            model.UnlockStatus = System.Enum.Parse<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}