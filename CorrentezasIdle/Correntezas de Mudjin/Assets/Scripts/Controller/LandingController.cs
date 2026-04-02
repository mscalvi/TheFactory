using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameHelper;

public class LandingController : MonoBehaviour
{
    [SerializeField] LandingService LandingService;
    [SerializeField] LandingUiService UiService;

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

        // Landing
        UiService.Initialize(Game, Data);

        LandingService.Initialize(Game, Data);

        var Ship = GameController.Instance.GameState.ShipState;

        if (Ship == null)
        {
            Debug.LogError("Ship NULL!");
            return;
        }

    }
}
