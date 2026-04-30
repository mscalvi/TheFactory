using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EventService : MonoBehaviour
{
    private GameState GameState;

    private UnlockService UnlockService;


    public void Initialize(GameState gameState, UnlockService unlockService)
    {
        GameState = gameState;

        UnlockService = unlockService;
    }

    private void EventHandler(EventInstance eventInstance)
    {
        switch (eventInstance.EventFrequency)
        {
            case EventHelper.EventFrequency.Unique:

                break;
            case EventHelper.EventFrequency.Common:
                break;
            case EventHelper.EventFrequency.Uncommon:
                break;
            case EventHelper.EventFrequency.Rare:
                break;
            case EventHelper.EventFrequency.Legendary:
                break;
        }
    }

    // Events
    void OnEnable()
    {
        GameEvents.OnEventTrigger += EventHandler;
    }

    void OnDisable()
    {
        GameEvents.OnEventTrigger -= EventHandler;
    }
}
