using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordesService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState gameState)
    {
        GameState = gameState;
    }

    private void DayRecordeCheck()
    {
        if (GameState.RecordsState.MaxDaysTraveling < GameState.ExpeditionState.DayCounter)
        {
            GameState.RecordsState.MaxDaysTraveling = GameState.ExpeditionState.DayCounter;
            GameEvents.NewDayRecord?.Invoke();
        }
    }


    // Events
    void OnEnable()
    {
        ExpeditionEvents.OnExpeditionEnd += DayRecordeCheck;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnExpeditionEnd -= DayRecordeCheck;
    }
}
