using System.Collections;
using System.Collections.Generic;
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

    public void UnlockUpgrade(UpgradeInstance upgrade)
    {
        foreach (var upgradeData in DataState.upgrades)
        {
            if (upgradeData.Value.UnlockId == upgrade.Id)
            {
                upgradeData.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }

        if (upgrade.Id.StartsWith("ub"))
        {
            foreach (var building in DataState.buildings)
            {
                if (building.Value.Id == upgrade.TargetId)
                {                    
                    UnlockBuilding(building.Value);
                    GameEvents.OnBuildingUnlock?.Invoke();
                }
            }
        }

        if (upgrade.Id.StartsWith("ut"))
        {
            foreach (var tripulation in DataState.tripulations)
            {
                if (tripulation.Value.Id == upgrade.TargetId)
                {
                    UnlockTripulation(tripulation.Value);
                    GameEvents.OnTripulationUnlock?.Invoke();
                }
            }
        }

        if(upgrade.ActualBuy >= upgrade.MaxBuy)
        {
            GameState.DataState.upgrades[upgrade.Id].UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
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

        GameState.DataState.buildings[buildingInstance.Id].UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
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
