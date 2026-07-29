using System.Collections.Generic;

public static class EventsData
{
    public static Dictionary<string, EventModel> All = new();

    public static void Load()
    {
        All.Clear();

        var rows = CSVLoaderService.Load("Data/Events");

        foreach (var row in rows)
        {
            EventModel model = new();

            model.Id = row["Id"];

            model.NameEN = row["NameEN"];
            model.NamePT = row["NamePT"];

            model.DescriptionEN = row["DescriptionEN"];
            model.DescriptionPT = row["DescriptionPT"];

            model.Target = row["Target"];
            model.Trigger = row["Trigger"];

            model.Frequency = ParseHelper.Enum<GameHelper.ItemRarity>(row["Frequency"]);

            model.EventType = ParseHelper.Enum<EventHelper.EventType>(row["EventType"]);

            model.UnlockId = row["UnlockId"];

            model.UnlockStatus = ParseHelper.Enum<UnlockHelper.UnlockStatus>(row["UnlockStatus"]);

            All[model.Id] = model;
        }
    }
}