using System.Collections.Generic;

public static class BuildingsData
{
    public static Dictionary<string, BuildingModel> All = new();

    public static void Load()
    {
        All.Clear();

        var rows = CSVLoaderService.Load("Data/Buildings");

        foreach (var row in rows)
        {
            BuildingModel model = new();

            model.Id = row["Id"];

            model.NamePT = row["NamePT"];
            model.NameEN = row["NameEN"];

            model.DescriptionPT = row["DescriptionPT"];
            model.DescriptionEN = row["DescriptionEN"];

            model.Type = ParseHelper.Enum<UpgradeHelper.UpgradeBuilding>(row["Type"]);

            model.Level = ParseHelper.Int(row["Level"]);

            model.UnlockId = row["UnlockId"];

            model.UnlockStatus = ParseHelper.Enum<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}