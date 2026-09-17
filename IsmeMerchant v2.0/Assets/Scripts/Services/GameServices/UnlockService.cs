using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnlockService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    public void Initialize(GameState gameState)
    {
        GameState = gameState;

        DataState = GameState.DataState;
    }

    private void UnlockCurrency(CurrencyInstance currency)
    {
        GameState.DataState.currencies[currency.Type].UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
    }

    public void UnlockUpgrade(UpgradeInstance upgrade)
    {
        foreach (var upgradeData in DataState.upgrades)
        {
            if (upgradeData.Value.UnlockId == upgrade.Id)
            {
                upgradeData.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }

        foreach (var mission in DataState.missions.Values)
        {
            if (mission.UnlockId == upgrade.Id)
            {
                mission.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }

        if(upgrade.ActualBuy >= upgrade.MaxBuy)
        {
            GameState.DataState.upgrades[upgrade.Id].UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
            GameEvents.OnCurrencyChange?.Invoke(upgrade.Currency, CurrencyHelper.CurrencyScope.Company);
            GameEvents.OnCurrencyChange?.Invoke(upgrade.Currency, CurrencyHelper.CurrencyScope.Expedition);
        }
    }

    private void UnlockBuilding(BuildingInstance buildingInstance)
    {
        if (buildingInstance.Level == 0)
        {
            buildingInstance.Level = 1;

            foreach (var upgradeData in DataState.upgrades)
            {
                if (upgradeData.Value.UnlockId == buildingInstance.Id)
                {
                    upgradeData.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
                }
            }

            GameState.DataState.buildings[buildingInstance.Id].UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
        }
        else
        {
            buildingInstance.Level++;
        }
    }

    public List<UpgradeInstance> UpgradeOptions(UpgradeHelper.UpgradeScope Scope)
    {
        List<UpgradeInstance> list = new List<UpgradeInstance>();

        foreach (var upgradeData in DataState.upgrades.Values)
        {
            if (upgradeData.UnlockStatus == UnlockHelper.UnlockStatus.Available || upgradeData.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
                continue;

            if (upgradeData.Scope != Scope)
                continue;

            if (upgradeData.UnlockStatus == UnlockHelper.UnlockStatus.Blocked)
                list.Add(upgradeData);
        }

        return list
                .OrderBy(_ => Guid.NewGuid())
                .Take(4)
                .ToList();
    }

    private void OnEnable()
    {
        GameEvents.StartedProduction += ProductionStarted;
    }

    private void OnDisable()
    {
        GameEvents.StartedProduction -= ProductionStarted;
    }

    private void ProductionStarted(ProductInstance product)
    {
        if (!GameState.ProgressState.MarcosTut)
        {
            UnlockCurrency(GameState.DataState.currencies[CurrencyHelper.CurrencyType.Marcos]);
            GameState.ProgressState.MarcosTut = true;
        }
    }
}
