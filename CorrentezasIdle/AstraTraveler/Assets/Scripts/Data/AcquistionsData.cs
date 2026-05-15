using System.Collections.Generic;

public static class AcquisitionsData
{
    public static Dictionary<string, AcquisitionModel> All = new();

    public static void Load()
    {
        All.Clear();

        var rows = CSVLoaderService.Load("Data/Acquisitions");

        foreach (var row in rows)
        {
            AcquisitionModel model = new();

            model.Id = row["Id"];

            model.NamePT = row["NamePT"];
            model.NameEN = row["NameEN"];

            model.DescriptionPT = row["DescriptionPT"];
            model.DescriptionEN = row["DescriptionEN"];

            model.Type = System.Enum.Parse<TripulationHelper.Type>(row["Type"]);

            model.TargetType = System.Enum.Parse<UpgradeHelper.TargetType>(row["TargetType"]);

            model.Cost = double.Parse(row["Cost"]);

            model.Currency = System.Enum.Parse<CurrencyHelper.CurrencyType>(row["Currency"]);

            model.Time = float.Parse(row["Time"]);

            model.UnlockId = row["UnlockId"];

            model.UnlockStatus = System.Enum.Parse<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}