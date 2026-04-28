using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Events")]
public class EventModel : ScriptableObject
{
    public string Id;
    public string Title;
    public string Info;

    public string Target;

    public EventHelper.EventFrequency EventFrequency;
    public EventHelper.EventType EventType;
}
