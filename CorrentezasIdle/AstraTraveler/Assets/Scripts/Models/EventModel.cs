using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventModel
{
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
}
