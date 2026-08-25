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
        if (FirstRunCheck()) return;

        ComputeRecorde();

        NewEnemiesUnlock();

        NewUpgradesUnlock();

        NewMissionsUnlock();

        NewCurrencyUnlocked();
    }

    private bool FirstRunCheck()
    {
        if (GameState.ExpeditionState.ExpeditionsDone <= 1)
            return true;

        return false;
    }

    private void ComputeRecorde()
    {
        Debug.Log($"Recorde Atual: {GameState.ProgressState.MaxDaysTraveling} dias");
        Debug.Log($"Run: {GameState.ExpeditionState.DayCounter} dias");

        int currentDays = GameState.ExpeditionState.DayCounter;
        int previousMax = GameState.ProgressState.MaxDaysTraveling;

        if (currentDays <= previousMax)
            return;

        int newThreshold5 = (currentDays + 5) / 10;
        int oldThreshold5 = (previousMax + 5) / 10;
        int expeditionUpgradesEarned = newThreshold5 - oldThreshold5;

        if (expeditionUpgradesEarned > 0)
        {
            GameState.ProgressState.UnlockableExpeditionUpgrades += expeditionUpgradesEarned;
            Debug.Log($"Upgrades de Expedição ganhos: {expeditionUpgradesEarned}");
        }

        int newThreshold10 = (currentDays + 0) / 10;
        int oldThreshold10 = (previousMax + 0) / 10;
        int companyUpgradesEarned = newThreshold10 - oldThreshold10;

        if (companyUpgradesEarned > 0)
        {
            GameState.ProgressState.UnlockableCompanyUpgrades += companyUpgradesEarned;
            Debug.Log($"Upgrades de Companhia ganhos: {companyUpgradesEarned}");
        }

        GameState.ProgressState.MaxDaysTraveling = currentDays;
        GameEvents.NewDayRecord?.Invoke();
    }

    private void NewEnemiesUnlock()
    {

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
    }

    private void NewUpgradesUnlock()
    {
        foreach (var upgrade in GameState.DataState.upgrades.Values)
        {
            int.TryParse(upgrade.UnlockId, out int unlockDay);

            if (unlockDay <= GameState.ProgressState.MaxDaysTraveling)
            {
                if (upgrade.UnlockStatus == UnlockHelper.UnlockStatus.Available || upgrade.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked || upgrade.UnlockStatus == UnlockHelper.UnlockStatus.Blocked)
                    continue;


                upgrade.UnlockStatus = UnlockHelper.UnlockStatus.Blocked;

                Debug.Log($"Upgrade Conhecido: {upgrade.NamePT} - Dia {unlockDay}");
            }
        }
    }

    private void NewMissionsUnlock()
    {
        //if (GameState.ProgressState.Missions == true)
        //{
        //    foreach (var mission in GameState.DataState.missions.Values)
        //    {
        //        int.TryParse(mission.UnlockId, out int unlockDay);

        //        if (unlockDay <= GameState.ProgressState.MaxDaysTraveling)
        //        {
        //            if (mission.UnlockStatus == UnlockHelper.UnlockStatus.Available || mission.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
        //                continue;

        //            mission.UnlockStatus = UnlockHelper.UnlockStatus.Available;
        //            Debug.Log($"Missão Desbloqueada: {mission.NamePT} - Dia {unlockDay}");
        //        }
        //    }
        //}
    }

    private void NewCurrencyUnlocked()
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
