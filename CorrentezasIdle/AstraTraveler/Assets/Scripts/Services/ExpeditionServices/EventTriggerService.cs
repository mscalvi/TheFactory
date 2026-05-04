using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EventTriggerService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;
    private ExpeditionState ExpeditionState;


    public void Initialize(GameState gameState)
    {
        GameState = gameState;

        ExpeditionState = GameState.ExpeditionState;

        DataState = GameState.DataState;
    }

    private void TriggerEvent(EventInstance eventInstance)
    {
        GameEvents.OnEventTrigger?.Invoke(eventInstance);
    }


    private void EventChance()
    {

    }

    // Events
    void OnEnable()
    {

    }

    void OnDisable()
    {

    }
}
