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

        if (upgrade.Id.StartsWith("uu"))
        {
            StudyUpgrade(upgrade);
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

        TripulationUpgrade(tripulationInstance);

        GameState.DataState.tripulations[tripulationInstance.Id].UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
    }

    private void StudyUpgrade(UpgradeInstance upgrade)
    {
        switch (upgrade.Id)
        {
            case "uub001":
                GameState.UnlockState.Acquisitions = true;
                GameState.UnlockState.Training = true;
                Debug.Log("Não esquece de arrumar");
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
                BuildingAcquisition(acq);
                break;
        }
    }

    private void BuildingAcquisition(AcquisitionInstance acq)
    {
        foreach (var building in GameState.DataState.buildings)
        {
            if (building.Value.Id == acq.TargetId)
            {
                UnlockBuilding(building.Value);
                Debug.Log(building.Value.Name + " -> " + building.Value.UnlockStatus);
            }
        }

        foreach (var acquistion in GameState.DataState.acquisitions)
        {
            if (acquistion.Value.UnlockId == acq.Id)
            {
                acquistion.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }
    }

    private void TripulationUpgrade(TripulationInstance tripulation)
    {
        switch (tripulation.Type)
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
            if (acquistion.Value.UnlockType == tripulation.Type)
            {
                acquistion.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
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
