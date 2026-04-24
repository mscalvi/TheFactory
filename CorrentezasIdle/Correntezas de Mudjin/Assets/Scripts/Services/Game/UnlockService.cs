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

        Unlock(upgrade);
    }

    private void Unlock(UpgradeInstance upgrade)
    {
        if (upgrade.Id == "uuu01")
        {
            GameState.UnlockState.Missions = true;
        }

        if (upgrade.Id.StartsWith("uub"))
        {
            foreach (var building in DataState.buildings)
            {
                if (building.Value.Id == upgrade.TargetId)
                {
                    building.Value.UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
                    GameEvents.OnBuildingUnlock?.Invoke();
                }
            }
        }
    }
}
