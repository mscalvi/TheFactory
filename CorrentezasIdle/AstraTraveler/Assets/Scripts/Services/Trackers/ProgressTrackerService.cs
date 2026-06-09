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

    private void ExpeditionEndCheck()
    {
        DayRecordeCheck();
        BestiaryCheck();
        ExpeditionCounterCheck();
    }

    private void DayRecordeCheck()
    {
        if (GameState.ProgressState.MaxDaysTraveling < GameState.ExpeditionState.DayCounter)
        {
            if (GameState.ExpeditionState.ExpeditionsDone <= 1)
                return;

            GameState.ProgressState.MaxDaysTraveling = GameState.ExpeditionState.DayCounter;
            GameEvents.NewDayRecord?.Invoke();
        }

        if (GameState.UnlockState.Constructions != true)
        {
            if (GameState.ProgressState.MaxDaysTraveling >= 5)
            {
                GameState.UnlockState.Constructions = true;
                GameEvents.OnMechanicUnlock?.Invoke("Constructions");
            }
        }

        if (GameState.UnlockState.Recruiting != true)
        {
            if (GameState.ProgressState.MaxDaysTraveling >= 10)
            {
                GameState.UnlockState.Recruiting = true;
            }
        }

        foreach (var enemy in GameState.DataState.enemies.Values)
        {
            int.TryParse(enemy.UnlockId, out int unlockDay);

            if (unlockDay <= GameState.ProgressState.MaxDaysTraveling)
            {
                if (enemy.UnlockStatus == UnlockHelper.UnlockStatus.Available || enemy.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
                    continue;

                enemy.UnlockStatus = UnlockHelper.UnlockStatus.Available;
                Debug.Log($"Inimigo Desbloqueado: {enemy.NamePT} - Dia {unlockDay}");
            }
        }

        if(GameState.UnlockState.Company == true)
        {
            foreach (var tripulation in GameState.DataState.tripulations.Values)
            {
                int.TryParse(tripulation.UnlockId, out int unlockDay);

                if (unlockDay <= GameState.ProgressState.MaxDaysTraveling)
                {
                    if (tripulation.UnlockStatus == UnlockHelper.UnlockStatus.Available || tripulation.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
                        continue;

                    tripulation.UnlockStatus = UnlockHelper.UnlockStatus.Available;
                    Debug.Log($"Membro Desbloqueado: {tripulation.Name} - Dia {unlockDay}");
                }
            }
        }

        if(GameState.UnlockState.Missions == true)
        {
            foreach (var mission in GameState.DataState.missions.Values)
            {
                int.TryParse(mission.UnlockId, out int unlockDay);

                if (unlockDay <= GameState.ProgressState.MaxDaysTraveling)
                {
                    if (mission.UnlockStatus == UnlockHelper.UnlockStatus.Available || mission.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
                        continue;

                    mission.UnlockStatus = UnlockHelper.UnlockStatus.Available;
                    Debug.Log($"Missão Desbloqueada: {mission.NamePT} - Dia {unlockDay}");
                }
            }
        }

        if (GameState.UnlockState.Recruiting == true)
        {
            foreach (var currencyData in GameState.DataState.currencies.Values)
            {
                int.TryParse(currencyData.UnlockId, out int unlockDay);

                if (unlockDay <= GameState.ProgressState.MaxDaysTraveling)
                {
                    if (currencyData.UnlockStatus == UnlockHelper.UnlockStatus.Available || currencyData.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
                        continue;

                    currencyData.UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
                    Debug.Log($"Dinheiro Desbloqueado: {currencyData.Type} - Dia {unlockDay}");

                    if(currencyData.Type == CurrencyHelper.CurrencyType.Prestige)
                    {
                        GameEvents.OnMechanicUnlock?.Invoke("Recruiting");
                    }
                }
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

    private void ExpeditionCounterCheck()
    {
        if (GameState.ExpeditionState.ExpeditionsDone > 0)
        {
            if (!GameState.UnlockState.Company)
            {
                GameState.UnlockState.Company = true;
            }
        }

        if (GameState.ExpeditionState.ExpeditionsDone > 2)
        {
            GameState.UnlockState.Missions = true;
        }
    }

    // Events
    void OnEnable()
    {
        ExpeditionEvents.OnExpeditionEnd += ExpeditionEndCheck;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnExpeditionEnd -= ExpeditionEndCheck;
    }
}
