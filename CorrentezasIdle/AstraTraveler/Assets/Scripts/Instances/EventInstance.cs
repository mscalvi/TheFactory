using Unity.VisualScripting;
using UnityEngine;

public class EventInstance
{
    public EventModel Model;

    public string Id;

    public string TitleEN;
    public string TitlePT;

    public string DescriptionEN;
    public string DescriptionPT;

    public string Target;
    public string Trigger;

    public GameHelper.ItemRarity Frequency;
    public EventHelper.EventType EventType;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public EventInstance(EventModel model)
    {
        Id = model.Id;
        TitleEN = model.NameEN;
        TitlePT = model.NamePT;
        DescriptionEN = model.DescriptionEN;
        DescriptionPT = model.DescriptionPT;

        Target = model.Target;
        Trigger = model.Trigger;

        Frequency = model.Frequency;
        EventType = model.EventType;

        UnlockId = model.UnlockId;
        UnlockStatus = model.UnlockStatus;
    }
}