using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    public void Initialize(GameState gameState, DataState dataState)
    {
        GameState = gameState;

        DataState = dataState;
    }

    public void UnlockUpgrade(UpgradeInstance upgrade)
    {
        if (upgrade.Id == "uuu01")
        {
            GameState.UnlockState.Missions = true;
        }

        foreach (var upgradeData in DataState.upgrades)
        {
            if (upgradeData.Value.UnlockId == upgrade.Id)
            {
                upgradeData.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }

        if (upgrade.Id.StartsWith("uub"))
        {
            foreach (var building in DataState.buildings)
            {
                if (building.Value.Id == upgrade.TargetId)
                {                    
                    UnlockBuilding(building.Value);
                    building.Value.UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
                    GameEvents.OnBuildingUnlock?.Invoke();
                }
            }
        }

        if (upgrade.Id.StartsWith("uut"))
        {
            foreach (var tripulation in DataState.tripulations)
            {
                if (tripulation.Value.Id == upgrade.TargetId)
                {
                    UnlockTripulation(tripulation.Value);
                    tripulation.Value.UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
                    //GameEvents.OnTripulationUnlock?.Invoke();
                }
            }
        }
    }

    public void UnlockBuilding(BuildingInstance buildingInstance)
    {
        foreach (var upgradeData in DataState.upgrades)
        {
            if (upgradeData.Value.UnlockId == buildingInstance.Id)
            {
                upgradeData.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }
    }

    public void UnlockTripulation(TripulationInstance tripulationInstance)
    {
        foreach (var upgradeData in DataState.upgrades)
        {
            if (upgradeData.Value.UnlockId == tripulationInstance.Id)
            {
                upgradeData.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }

        GameState.DataState.tripulations[tripulationInstance.Id].UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
    }
}
