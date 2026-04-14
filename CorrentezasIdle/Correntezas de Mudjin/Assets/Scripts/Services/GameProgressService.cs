using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState gameState)
    {
        GameState = gameState;
    }

    // Compute de Progress
    private void FinishFirstExpedition()
    {
        GameState.ProgressState.m000 = true;
        GameState.UnlockState.Company = true;

        foreach (var upgrade in GameState.DataState.upgrades)
        {
            if (upgrade.Value.UnlockId == "m000")
            {
                upgrade.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }
    }

    // Events
    void OnEnable()
    {
        ProgressEvents.OnFirstExpeditionFinish += FinishFirstExpedition;
    }

    void OnDisable()
    {
        ProgressEvents.OnFirstExpeditionFinish -= FinishFirstExpedition;
    }
}
