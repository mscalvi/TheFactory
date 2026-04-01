using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyBuildingsController : MonoBehaviour
{
    [SerializeField] CompanyPurchaseService CompanyPurchaseService;

    [SerializeField] CompanyBuildingsService CompanyBuildingsService;

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

        // Buildings
        CompanyPurchaseService.Initialize(Data);

        CompanyBuildingsService.Initialize(Data, CompanyPurchaseService);
    }
}
