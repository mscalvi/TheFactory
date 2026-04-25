using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EventService : MonoBehaviour
{
    private GameState GameState;


    public void Initialize(GameState gameState)
    {
        GameState = gameState;
    }

    private void EventHandler(EventInstance eventInstance)
    {
        Debug.Log($"Game EventService - Ok");
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
