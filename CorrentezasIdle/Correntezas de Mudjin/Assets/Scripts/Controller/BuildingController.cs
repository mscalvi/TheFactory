using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameHelper;

public class BuildingController : MonoBehaviour
{
    [SerializeField] BuildingUiService BuildingUiService;

    private void Awake()
    {
        var Game = GameController.Instance.GameState;

        if (Game == null)
        {
            Debug.LogError("Game NULL!");
            return;
        }

        var PurchaseService = GameController.Instance.CompanyPurchaseService;

        BuildingUiService.Initialize(Game, PurchaseService);
    }
}
