using Unity.VisualScripting;
using UnityEngine;

public class EventInstance
{
    public EventModel Model;

    public string Id;
    public string Title;
    public string Info;
    public string Target;
    public EventHelper.EventFrequency EventFrequency;
    public EventHelper.EventType EventType;

    public EventInstance(EventModel model)
    {
        Id = model.Id;
        Title = model.Title;
        Info = model.Info;
        Target = model.Target;

        EventFrequency = model.EventFrequency;
        EventType = model.EventType;
    }
}