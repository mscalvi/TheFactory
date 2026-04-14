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
        foreach (var upgradeData in DataState.upgrades)
        {
            if (upgradeData.Value.UnlockId == upgrade.Id)
            {
                upgradeData.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }

        switch (upgrade.Scope)
        {
            case UpgradeHelper.UpgradeScope.Meta:
                MetaUnlock(upgrade);
                break;
            case UpgradeHelper.UpgradeScope.Company:
                CompanyUnlock(upgrade);
                break;
            case UpgradeHelper.UpgradeScope.Expedition:
                ExpeditionUnlock(upgrade);
                break;
        }
    }

    private void MetaUnlock(UpgradeInstance upgrade)
    {
        switch (upgrade.Id) 
        {
            case "uuu01":
                Debug.Log("Desbloquear Missões!");
                GameState.UnlockState.Missions = true;
                break;
        }
    }

    private void CompanyUnlock(UpgradeInstance upgrade)
    {
        if (upgrade.Id.StartsWith("uub"))
        {
            foreach (var building in DataState.buildings)
            {
                if (building.Value.Id == upgrade.TargetId)
                {
                    building.Value.UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
                    CompanyEvents.OnBuildingUnlock?.Invoke();
                }
            }
        }
    }

    private void ExpeditionUnlock(UpgradeInstance upgrade)
    {

    }
}
