using System.Collections.Generic;

public static class ConstructionData
{
    public static Dictionary<string, ConstructionModel> All = new();

    public static void Load()
    {
        All.Clear();

        var rows = CSVLoaderService.Load("Data/Constructions");

        foreach (var row in rows)
        {
            ConstructionModel model = new();

            model.Id = row["Id"];

            model.NamePT = row["NamePT"];
            model.NameEN = row["NameEN"];

            model.DescriptionPT = row["DescriptionPT"];
            model.DescriptionEN = row["DescriptionEN"];

            model.Type = ParseHelper.Enum<TripulationHelper.Type>(row["Type"]);

            model.TargetType = ParseHelper.Enum<UpgradeHelper.TargetType>(row["TargetType"]);

            model.Cost = ParseHelper.Double(row["Cost"]);

            model.Currency = ParseHelper.Enum<CurrencyHelper.CurrencyType>(row["Currency"]);

            model.Time = ParseHelper.Float(row["Time"]);

            model.Level = ParseHelper.Int(row["Level"]);

            model.UnlockId = row["UnlockId"];

            model.UnlockStatus = ParseHelper.Enum<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}