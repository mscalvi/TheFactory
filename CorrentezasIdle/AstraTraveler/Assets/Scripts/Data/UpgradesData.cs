using System.Collections.Generic;
using UnityEngine;

public static class UpgradesData
{
    public static Dictionary<string, UpgradeModel> All = new();

    private static readonly string[] Files =
    {
    "Data/upweapons",
    "Data/upammos",
    "Data/upship",
    "Data/upincome",
    "Data/upmeta"
    };

    public static void Load()
    {
        All.Clear();

        foreach (var file in Files)
        {
            ColumnsToLoad(
                CSVLoaderService.Load(file)
            );
        }
    }

    private static void ColumnsToLoad(List<Dictionary<string, string>> rows)
    {
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

            if (All.ContainsKey(model.Id))
            {
                Debug.LogError($"Upgrade duplicado: {model.Id}");
                continue;
            }

            All.Add(model.Id, model);

            All[model.Id] = model;
        }
    }
}