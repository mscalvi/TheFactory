using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRecordsService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState gameState)
    {
        GameState = gameState;
    }

    private void DayRecordCheck()
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
        ExpeditionEvents.OnExpeditionEnd += DayRecordCheck;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnExpeditionEnd -= DayRecordCheck;
    }
}
