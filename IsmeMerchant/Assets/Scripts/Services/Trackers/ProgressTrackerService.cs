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
        int ExpeditionUpgradesTreshold = 0;
        int CompanyUpgradesTreshold = 0;

        int LastExpeditionUpgradesTreshold = 0;
        int LastCompanyUpgradesTreshold = 0;

        if (GameState.ProgressState.MaxDaysTraveling < GameState.ExpeditionState.DayCounter)
        {
            ExpeditionUpgradesTreshold = GameState.ExpeditionState.DayCounter / 5;
            CompanyUpgradesTreshold = GameState.ExpeditionState.DayCounter / 10;

            LastExpeditionUpgradesTreshold = GameState.ProgressState.MaxDaysTraveling / 5;
            LastCompanyUpgradesTreshold = GameState.ProgressState.MaxDaysTraveling / 10;

            if(ExpeditionUpgradesTreshold > LastExpeditionUpgradesTreshold)
            {
                GameState.ProgressState.UnlockableExpeditionUpgrades = ExpeditionUpgradesTreshold - LastExpeditionUpgradesTreshold;
                Debug.Log("Upgrade Expedition Disponível");
            }
            else
            {
                GameState.ProgressState.UnlockableExpeditionUpgrades = 0;
            }
            if (CompanyUpgradesTreshold > LastCompanyUpgradesTreshold)
            {
                GameState.ProgressState.UnlockableCompanyUpgrades = CompanyUpgradesTreshold - LastCompanyUpgradesTreshold;
                Debug.Log("Upgrade Company Disponível");
            }
            else
            {
                GameState.ProgressState.UnlockableCompanyUpgrades = 0;
            }

            if (GameState.ExpeditionState.ExpeditionsDone <= 1)
                return;

            GameState.ProgressState.MaxDaysTraveling = GameState.ExpeditionState.DayCounter;
            GameEvents.NewDayRecord?.Invoke();
        }
        else
        {
            return;
        }

        if (GameState.ProgressState.Constructions != true)
        {
            if (GameState.ProgressState.MaxDaysTraveling >= 5)
            {
                GameState.ProgressState.Constructions = true;
                GameEvents.OnMechanicUnlock?.Invoke("Constructions");
            }
        }

        if (GameState.ProgressState.Recruiting != true)
        {
            if (GameState.ProgressState.MaxDaysTraveling >= 10)
            {
                GameState.ProgressState.Recruiting = true;
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

        foreach (var upgrade in GameState.DataState.upgrades.Values)
        {
            int.TryParse(upgrade.UnlockId, out int unlockDay);

            if (unlockDay <= GameState.ProgressState.MaxDaysTraveling)
            {
                if (upgrade.UnlockStatus == UnlockHelper.UnlockStatus.Available || upgrade.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked || upgrade.UnlockStatus == UnlockHelper.UnlockStatus.Blocked)
                    continue;

                foreach (var trigger in GameState.ProgressState.UpgradeTriggers)
                {
                    if (upgrade.UnlockTrigger == trigger.Key)
                    {
                        if (trigger.Value)
                        {
                            upgrade.UnlockStatus = UnlockHelper.UnlockStatus.Blocked;
                            Debug.Log($"Upgrade Conhecido: {upgrade.NamePT} - Dia {unlockDay}");
                        }
                    }
                }
            }
            else
            {
                Debug.Log($"{upgrade.NamePT} - Trigger Desconhecido");
            }
        }

        if (GameState.ProgressState.Company == true)
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

        if(GameState.ProgressState.Missions == true)
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

        if (GameState.ProgressState.Recruiting == true)
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
            if (!GameState.ProgressState.Company)
            {
                GameState.ProgressState.Company = true;
            }
        }

        if (GameState.ExpeditionState.ExpeditionsDone > 2)
        {
            GameState.ProgressState.Missions = true;
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
