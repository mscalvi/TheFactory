using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductData
{
    public static Dictionary<string, ProductModel> All = new();

    public static void Load()
    {
        All.Clear();

        var rows = CSVLoaderService.Load("Data/Products");

        foreach (var row in rows)
        {
            ProductModel model = new();

            model.Id = row["Id"];

            model.NamePT = row["NamePT"];
            model.NameEN = row["NameEN"];

            model.DescriptionPT = row["DescriptionPT"];
            model.DescriptionEN = row["DescriptionEN"];

            model.Ing1 = ParseHelper.Enum<AlchemyHelper.IngredientType>(row["Ing1"]);
            model.Ing2 = ParseHelper.Enum<AlchemyHelper.IngredientType>(row["Ing2"]);
            model.Ing3 = ParseHelper.Enum<AlchemyHelper.IngredientType>(row["Ing3"]);
            model.Ing4 = ParseHelper.Enum<AlchemyHelper.IngredientType>(row["Ing4"]);
            model.Ing5 = ParseHelper.Enum<AlchemyHelper.IngredientType>(row["Ing5"]);

            model.Cost1 = ParseHelper.Double(row["Cost1"]);
            model.Cost2 = ParseHelper.Double(row["Cost2"]);
            model.Cost3 = ParseHelper.Double(row["Cost3"]);
            model.Cost4 = ParseHelper.Double(row["Cost4"]);
            model.Cost5 = ParseHelper.Double(row["Cost5"]);

            model.IncomeAmmount = ParseHelper.Double(row["IncomeAmmount"]);
            model.IncomeType = ParseHelper.Enum<CurrencyHelper.CurrencyType>(row["IncomeType"]);

            model.Time = ParseHelper.Double(row["Time"]);

            model.Level = ParseHelper.Int(row["Level"]);

            model.UnlockId = row["UnlockId"];

            model.LabType = ParseHelper.Enum<AlchemyHelper.LabType>(row["LabType"]);

            model.UnlockStatus = ParseHelper.Enum<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}
