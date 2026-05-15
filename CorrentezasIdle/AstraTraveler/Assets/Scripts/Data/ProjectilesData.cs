using System.Collections.Generic;

public static class ProjectilesData
{
    public static Dictionary<string, ProjectileModel> All = new();

    public static void Load()
    {
        All.Clear();

        var rows = CSVLoaderService.Load("Data/Projectiles");

        foreach (var row in rows)
        {
            ProjectileModel model = new();

            model.Id = row["Id"];

            model.NameEN = row["NameEN"];
            model.NamePT = row["NamePT"];

            model.Type = System.Enum.Parse<WeaponHelper.AmmoType>(row["Type"]);

            model.SpritePath = System.Enum.Parse<WeaponHelper.PathType>(row["SpritePath"]);

            model.BehaviorType = System.Enum.Parse<WeaponHelper.BehaviorType>(row["BehaviorType"]);

            All[model.Id] = model;
        }
    }
}