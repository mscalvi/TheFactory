using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingController : MonoBehaviour
{
    [SerializeField] LandingUi Ui;

    private void Awake()
    {
        var Game = GameController.Instance.GameState;
        if (Game == null)
        {
            Debug.LogError("Game NULL!");
            return;
        }

        var Data = GameController.Instance.GameState.DataState;
        if (Data == null)
        {
            Debug.LogError("Data NULL!");
            return;
        }

        var MissionsService = GameController.Instance.MissionsService;

        var Ship = GameController.Instance.GameState.ExpeditionState.Ship;
        if (Ship == null)
        {
            Debug.LogError("Ship NULL!");
            return;
        }

        // Landing
        Ui.Initialize(Game, MissionsService);
    }
}
