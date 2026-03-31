using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameHelper;

public class LandingController : MonoBehaviour
{
    [SerializeField] LandingService LandingService;
    [SerializeField] PermanentUpgradeService PermanentUpgradeService;

    private void Awake()
    {
        var Game = GameController.Instance.GameState;

        var Ship = GameController.Instance.GameState.ShipState;

        if (Game == null)
        {
            Debug.LogError("Game NULL!");
            return;
        }

        var Data = GameController.Instance.GameState.DataState;

        // Landing
        LandingService.Initialize(Game, Data);

        // Buildings
        PermanentUpgradeService.Initialize(Ship);
    }
}
