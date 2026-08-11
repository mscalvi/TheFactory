using System.Collections.Generic;

public static class LabsData
{
    public static Dictionary<string, LabModel> All = new();

    public static void Load()
    {
        All.Clear();

        var rows = CSVLoaderService.Load("Data/Labs");

        foreach (var row in rows)
        {
            LabModel model = new();

            model.Id = row["Id"];

            model.NamePT = row["NamePT"];
            model.NameEN = row["NameEN"];

            model.DescriptionPT = row["DescriptionPT"];
            model.DescriptionEN = row["DescriptionEN"];

            model.Type = ParseHelper.Enum<AlchemyHelper.LabType>(row["Type"]);

            model.Level = ParseHelper.Int(row["Level"]);

            model.UnlockId = row["UnlockId"];

            model.UnlockStatus = ParseHelper.Enum<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}