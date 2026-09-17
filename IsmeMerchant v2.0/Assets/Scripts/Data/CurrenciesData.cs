using System.Collections.Generic;

public static class CurrenciesData
{
    public static Dictionary<string, CurrencyModel> All = new();

    public static void Load()
    {
        All.Clear();

        var rows = CSVLoaderService.Load("Data/Currencies");

        foreach (var row in rows)
        {
            CurrencyModel model = new();

            model.Id = row["Id"];

            model.NamePT = row["NamePT"];
            model.NameEN = row["NameEN"];

            model.DescriptionPT = row["DescriptionPT"];
            model.DescriptionEN = row["DescriptionEN"];

            model.Type = ParseHelper.Enum<CurrencyHelper.CurrencyType>(row["Type"]);

            model.Scope = ParseHelper.Enum<CurrencyHelper.CurrencyScope>(row["Scope"]);

            model.UnlockId = row["UnlockId"];

            model.UnlockStatus = ParseHelper.Enum<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}