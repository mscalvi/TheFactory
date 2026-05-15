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

        foreach (var mission in DataState.missions.Values)
        {
            if (mission.UnlockId == upgrade.Id)
            {
                mission.UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
            }
        }

        if (upgrade.Id.StartsWith("uu"))
        {
            StudyUpgrade(upgrade);

            foreach (var acquistion in GameState.DataState.acquisitions.Values)
            {
                if (acquistion.UnlockId == upgrade.Id)
                {
                    acquistion.UnlockStatus = UnlockHelper.UnlockStatus.Available;
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
        var ship = GameState.ExpeditionState.Ship;

        foreach (var upgradeData in DataState.upgrades)
        {
            if (upgradeData.Value.UnlockId == tripulationInstance.Id)
            {
                upgradeData.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }

        switch (tripulationInstance.Type)
        {
            case TripulationHelper.Type.Shipbuilder:
                GameState.ProgressState.Shipbuilder = true;
                break;
            case TripulationHelper.Type.Hunter:
                GameState.ProgressState.Hunter = true;
                break;
            case TripulationHelper.Type.Merchant:
                GameState.ProgressState.Merchant = true;
                break;
            case TripulationHelper.Type.Alchemist:
                GameState.ProgressState.Alchemist = true;
                break;
            case TripulationHelper.Type.Fisherman:
                GameState.ProgressState.Fisherman = true;
                break;
            case TripulationHelper.Type.Coach:
                GameState.ProgressState.Coach = true;
                break;
            case TripulationHelper.Type.Weaponsmith:
                GameState.ProgressState.Weaponsmith = true;
                break;
        }

        foreach (var acquistion in GameState.DataState.acquisitions)
        {
            if (acquistion.Value.UnlockType == tripulationInstance.Type)
            {
                acquistion.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }

        GameState.ExpeditionState.ActiveTripulation.Add(tripulationInstance);

        GameState.ExpeditionState.ActiveRecruits.Clear();

        GameState.DataState.tripulations[tripulationInstance.Id].UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
    }

    private void StudyUpgrade(UpgradeInstance upgrade)
    {
        switch (upgrade.Id)
        {
            case "uub001":
                GameState.UnlockState.Acquisitions = true;
                break;
            case "uub002":
                GameState.UnlockState.Alchemy = true;
                break;
            case "uub003":
                GameState.UnlockState.Training = true;
                break;
            case "uub004":
                GameState.UnlockState.Recruiting = true;
                break;
        }
    }

    private void AcquisitonUpgrade(AcquisitionInstance acq) 
    {
        string acqId = acq.Id.Substring(0,2);

        switch (acqId)
        {
            case "a1":
                foreach (var building in GameState.DataState.buildings)
                {
                    if (building.Value.UnlockId == acq.Id)
                    {
                        UnlockBuilding(building.Value);
                        Debug.Log(building.Value.NamePT + " -> " + building.Value.UnlockStatus);
                    }
                }
                break;
        }

        foreach (var upgrade in GameState.DataState.upgrades.Values)
        {
            if (upgrade.UnlockId == acq.Id)
            {
                upgrade.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }

        foreach (var acquistion in GameState.DataState.acquisitions.Values)
        {
            if (acquistion.UnlockId == acq.Id)
            {
                acquistion.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }
    }

    private void OnEnable()
    {
        GameEvents.OnAcquisitionFinished += AcquisitonUpgrade;
        GameEvents.OnTripulationPurchase += UnlockTripulation;
    }

    private void OnDisable()
    {
        GameEvents.OnAcquisitionFinished -= AcquisitonUpgrade;
        GameEvents.OnTripulationPurchase -= UnlockTripulation;
    }
}
