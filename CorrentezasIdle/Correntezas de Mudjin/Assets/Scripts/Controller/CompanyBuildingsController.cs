using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyBuildingsController : MonoBehaviour
{
    [SerializeField] CompanyUiService CompanyUiService;
    [SerializeField] CompanyPurchaseService CompanyPurchaseService;
    [SerializeField] CompanyUpgradeService CompanyUpgradeService;
    [SerializeField] CompanyCurrencyService CompanyCurrencyService;
    [SerializeField] CompanyPricingService CompanyPricingService;
    [SerializeField] UnlockService UnlockService;

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

        var Ship = GameController.Instance.GameState.ShipState;

        if (Ship == null)
        {
            Debug.LogError("Ship NULL!");
            return;
        }

        CompanyCurrencyService.Initialize(Game);

        CompanyPurchaseService.Initialize(Game, Data, CompanyCurrencyService);

        CompanyUiService.Initialize(Game, Data, CompanyPurchaseService);

        UnlockService.Initialize(Game, Data);

        CompanyUpgradeService.Initialize(Game, Data, Ship, UnlockService);

        CompanyPricingService.Initialize(Game, Data);
    }
}
