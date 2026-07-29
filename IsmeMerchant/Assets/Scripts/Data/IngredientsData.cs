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

            model.Type = ParseHelper.Enum<IngredientHelper.IngredientType>(row["Type"]);

            model.Rarity = ParseHelper.Enum<GameHelper.ItemRarity>(row["Rarity"]);

            model.Class = ParseHelper.Enum<IngredientHelper.IngredientClass>(row["Class"]);

            model.UnlockId = row["UnlockId"];

            model.UnlockStatus = ParseHelper.Enum<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}