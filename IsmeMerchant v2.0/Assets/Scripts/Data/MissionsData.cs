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

            model.Level = ParseHelper.Int(row["Level"]);

            model.MissionRarity = ParseHelper.Enum<GameHelper.ItemRarity>(row["MissionRarity"]);

            model.RewardType1 = ParseHelper.Enum<CurrencyHelper.CurrencyType>(row["RewardType1"]);
            model.RewardType2 = ParseHelper.Enum<CurrencyHelper.CurrencyType>(row["RewardType2"]);
            model.RewardType3 = ParseHelper.Enum<CurrencyHelper.CurrencyType>(row["RewardType3"]);
            model.RewardType4 = ParseHelper.Enum<CurrencyHelper.CurrencyType>(row["RewardType4"]);

            model.MissionType = ParseHelper.Enum<MissionHelper.MissionType>(row["MissionType"]);

            model.UnlockId = row["UnlockId"];

            model.UnlockStatus = ParseHelper.Enum<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}