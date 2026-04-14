using Unity.VisualScripting;
using UnityEngine;

public class EventInstance
{
    public EventModel Model;

    public string Id;
    public string Title;
    public string Info;

    public EventInstance(EventModel model)
    {
        Id = model.Id;
        Title = model.Title;
        Info = model.Info;
    }
}