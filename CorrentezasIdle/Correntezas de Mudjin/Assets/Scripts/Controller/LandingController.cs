using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameHelper;

public class LandingController : MonoBehaviour
{
    [SerializeField] LandingService LandingService;

    private void Awake()
    {
        var Game = GameController.Instance.GameState;

        if (Game == null)
        {
            Debug.LogError("Game NULL!");
            return;
        }

        var db = GameController.Instance.GameState.DataState;

        // Landing
        LandingService.Initialize(Game, db);
    }
}
