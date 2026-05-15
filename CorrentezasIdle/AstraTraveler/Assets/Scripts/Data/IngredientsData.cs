using System.Collections.Generic;

public static class IngredientsData
{
    public static Dictionary<string, IngredientModel> All = new();

    public static void Load()
    {
        All.Clear();

        var rows = CSVLoaderService.Load("Data/Ingredients");

        foreach (var row in rows)
        {
            IngredientModel model = new();

            model.Id = row["Id"];

            model.NamePT = row["NamePT"];
            model.NameEN = row["NameEN"];

            model.DescriptionPT = row["DescriptionPT"];
            model.DescriptionEN = row["DescriptionEN"];

            model.Type = System.Enum.Parse<IngredientHelper.IngredientType>(row["Type"]);

            model.Rarity = System.Enum.Parse<GameHelper.ItemRarity>(row["Rarity"]);

            model.Class = System.Enum.Parse<IngredientHelper.IngredientClass>(row["Class"]);

            model.UnlockId = row["UnlockId"];

            model.UnlockStatus = System.Enum.Parse<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}