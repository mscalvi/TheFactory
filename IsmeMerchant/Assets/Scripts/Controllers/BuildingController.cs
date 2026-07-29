using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [SerializeField] BuildingUi BuildingUi;

    private void Awake()
    {
        var Game = GameController.Instance.GameState;

        if (Game == null)
        {
            Debug.LogError("Game NULL!");
            return;
        }

        var PurchaseService = GameController.Instance.PurchaseService;

        BuildingUi.Initialize(Game, PurchaseService);
    }
}
