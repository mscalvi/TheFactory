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
            case GameHelper.ItemRarity.Unique:
                break;
            case GameHelper.ItemRarity.Common:
                break;
            case GameHelper.ItemRarity.Uncommon:
                break;
            case GameHelper.ItemRarity.Rare:
                break;
            case GameHelper.ItemRarity.Legendary:
                break;
        }
    }

    private void ExpeditionEndEvent()
    {
        if (GameState.UnlockState.Company)
        {
            if (!GameState.UnlockState.Studies)
            {
                GameState.UnlockState.Studies = true;
                GameState.DataState.missions["m1002"].UnlockStatus = UnlockHelper.UnlockStatus.Available;
                GameState.DataState.missions["m1003"].UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }

        if (!GameState.UnlockState.Company)
        {
            GameState.UnlockState.Company = true;
        }
    }

    // Events
    void OnEnable()
    {
        ExpeditionEvents.OnExpeditionEnd += ExpeditionEndEvent;
        GameEvents.OnEventTrigger += EventHandler;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnExpeditionEnd -= ExpeditionEndEvent;
        GameEvents.OnEventTrigger -= EventHandler;
    }
}
