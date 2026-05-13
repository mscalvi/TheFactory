using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressTrackerService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState gameState)
    {
        GameState = gameState;
    }

    private void DayRecordeCheck()
    {
        if (GameState.ProgressState.MaxDaysTraveling < GameState.ExpeditionState.DayCounter)
        {
            GameState.ProgressState.MaxDaysTraveling = GameState.ExpeditionState.DayCounter;
            GameEvents.NewDayRecord?.Invoke();
        }
    }

    private void BestiaryCheck()
    {
        var Bestiary = GameState.BestiaryState.Bestiary;

        foreach (var entry in Bestiary)
        {
            entry.Value.KilledLastExpedition = entry.Value.KilledExpedition;
            entry.Value.KilledExpedition = 0;
        }
    }

    // Events
    void OnEnable()
    {
        ExpeditionEvents.OnExpeditionEnd += DayRecordeCheck;
        ExpeditionEvents.OnExpeditionEnd += BestiaryCheck;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnExpeditionEnd -= DayRecordeCheck;
        ExpeditionEvents.OnExpeditionEnd -= BestiaryCheck;
    }
}
