using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripulationController : MonoBehaviour
{
    [SerializeField] TripulationUi TripulationUi;

    private void Awake()
    {
        var Game = GameController.Instance.GameState;

        if (Game == null)
        {
            Debug.LogError("Game NULL!");
            return;
        }

        var PurchaseService = GameController.Instance.PurchaseService;
        var UnlockService = GameController.Instance.UnlockService;

        TripulationUi.Initialize(Game, UnlockService);
    }
}
