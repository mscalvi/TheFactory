using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EventTriggerService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;
    private ExpeditionState ExpeditionState;


    public void Initialize(GameState gameState, DataState dataState, ExpeditionState expeditionState)
    {
        GameState = gameState;

        ExpeditionState = expeditionState;

        DataState = dataState;
    }

    private void CheckDayFixEvents()
    {
        if (!GameState.ProgressState.m000)
        {
            return;
        }

        if (!GameState.UnlockState.Click)
        {
            if (ExpeditionState.DayCounter == 3)
            {
                TriggerEvent(DataState.events["e001"]);
                return;
            }
        }

        if (!GameState.UnlockState.Weapons)
        {
            if (ExpeditionState.DayCounter == 10)
            {
                TriggerEvent(DataState.events["e002"]);
                return;
            }
        }

        if (!GameState.UnlockState.Currency)
        {
            if (ExpeditionState.DayCounter == 20)
            {
                TriggerEvent(DataState.events["e003"]);
                return;
            }
        }

        // Conferir probabilidade
        // Chamar
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
        ExpeditionEvents.OnNightFinish += CheckDayFixEvents;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnNightFinish -= CheckDayFixEvents;
    }
}
