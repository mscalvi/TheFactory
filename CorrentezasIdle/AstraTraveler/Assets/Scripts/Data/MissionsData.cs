using System.Collections.Generic;

public static class MissionsData
{
    public static Dictionary<string, MissionModel> All = new();

    public static void Load()
    {
        All.Clear();

        var rows = CSVLoaderService.Load("Data/Missions");

        foreach (var row in rows)
        {
            MissionModel model = new();

            model.Id = row["Id"];

            model.NamePT = row["NamePT"];
            model.NameEN = row["NameEN"];

            model.DescriptionPT = row["DescriptionPT"];
            model.DescriptionEN = row["DescriptionEN"];

            model.Level = int.Parse(row["Level"]);

            model.MissionRarity = System.Enum.Parse<GameHelper.ItemRarity>(row["MissionRarity"]);

            model.RewardType1 = System.Enum.Parse<CurrencyHelper.CurrencyType>(row["RewardType1"]);
            model.RewardType2 = System.Enum.Parse<CurrencyHelper.CurrencyType>(row["RewardType2"]);
            model.RewardType3 = System.Enum.Parse<CurrencyHelper.CurrencyType>(row["RewardType3"]);
            model.RewardType4 = System.Enum.Parse<CurrencyHelper.CurrencyType>(row["RewardType4"]);

            model.MissionType = System.Enum.Parse<MissionHelper.MissionType>(row["MissionType"]);

            model.UnlockId = row["UnlockId"];

            model.UnlockStatus = System.Enum.Parse<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}