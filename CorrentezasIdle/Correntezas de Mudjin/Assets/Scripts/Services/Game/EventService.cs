using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EventService : MonoBehaviour
{
    private GameState GameState;

    private TripulationService TripulationService;
    private UnlockService UnlockService;


    public void Initialize(GameState gameState, TripulationService tripulationService, UnlockService unlockService)
    {
        GameState = gameState;

        TripulationService = tripulationService;

        UnlockService = unlockService;
    }

    private void EventHandler(EventInstance eventInstance)
    {
        switch (eventInstance.EventFrequency)
        {
            case EventHelper.EventFrequency.Unique:
                MainEventHandler(eventInstance);
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

    private void MainEventHandler(EventInstance eventInstance)
    {
        switch (eventInstance.Id)
        {
            case "e001":
                UnlockService.UnlockTripulation(GameState.DataState.tripulations[eventInstance.Target]);
                GameState.UnlockState.Click = true;
                TripulationService.AddTripulationToCrew(GameState.DataState.tripulations[eventInstance.Target]);
                GameEvents.OnMechanicUnlock?.Invoke("Click");
                break;
            case "e002":
                UnlockService.UnlockTripulation(GameState.DataState.tripulations[eventInstance.Target]);
                GameState.UnlockState.Weapons = true;
                TripulationService.AddTripulationToCrew(GameState.DataState.tripulations[eventInstance.Target]);
                GameEvents.OnMechanicUnlock?.Invoke("Weapons");
                break;
            case "e003":
                UnlockService.UnlockTripulation(GameState.DataState.tripulations[eventInstance.Target]);
                GameState.UnlockState.Currency = true;
                TripulationService.AddTripulationToCrew(GameState.DataState.tripulations[eventInstance.Target]);
                GameEvents.OnMechanicUnlock?.Invoke("Income");
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
