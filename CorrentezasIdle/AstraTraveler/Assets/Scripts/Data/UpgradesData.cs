using System.Collections.Generic;

public static class UpgradesData
{
    public static Dictionary<string, UpgradeModel> All = new();

    public static void Load()
    {
        All.Clear();

        var rows = CSVLoaderService.Load("Data/Upgrades");

        foreach (var row in rows)
        {
            UpgradeModel model = new();

            model.Id = row["Id"];

            model.NameEN = row["NameEN"];
            model.NamePT = row["NamePT"];

            model.DescriptionEN = row["DescriptionEN"];
            model.DescriptionPT = row["DescriptionPT"];

            model.Scope =
                ParseHelper.Enum<UpgradeHelper.UpgradeScope>(
                    row["Scope"]
                );

            model.UpgradeType = System.Enum.Parse<UpgradeHelper.UpgradeType>(row["UpgradeType"]);
            model.EffectType = System.Enum.Parse<UpgradeHelper.EffectType>(row["EffectType"]);

            model.ExpeditionMenu = System.Enum.Parse<UpgradeHelper.UpgradeMenu>(row["ExpeditionMenu"]);
            model.Building = System.Enum.Parse<UpgradeHelper.UpgradeBuilding>(row["Building"]);

            model.UpgradeValue = ParseHelper.Double(row["UpgradeValue"]);

            model.TargetType = System.Enum.Parse<UpgradeHelper.TargetType>(row["TargetType"]);

            model.TargetId = row["TargetId"];

            model.MaxBuy = ParseHelper.Int(row["MaxBuy"]);

            model.Cost = ParseHelper.Double(row["Cost"]);
            model.CostGrowth = ParseHelper.Double(row["CostGrowth"]);

            model.Currency = System.Enum.Parse<CurrencyHelper.CurrencyType>(row["Currency"]);

            model.UnlockId = row["UnlockId"];
            model.UnlockStatus = System.Enum.Parse<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}