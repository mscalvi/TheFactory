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

        foreach (var enemy in GameState.DataState.enemies.Values)
        {
            int.TryParse(enemy.UnlockId, out int unlockDay);

            if (unlockDay <= GameState.ExpeditionState.DayCounter)
            {
                enemy.UnlockStatus = UnlockHelper.UnlockStatus.Available;
                Debug.Log($"Inimigo Desbloqueado: {enemy.NamePT} - Dia {unlockDay}");
            }
        }

        foreach (var tripulation in GameState.DataState.tripulations.Values)
        {
            int.TryParse(tripulation.UnlockId, out int unlockDay);

            if (unlockDay <= GameState.ExpeditionState.DayCounter)
            {
                tripulation.UnlockStatus = UnlockHelper.UnlockStatus.Available;
                Debug.Log($"Membro Desbloqueado: {tripulation.Name} - Dia {unlockDay}");
            }
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
