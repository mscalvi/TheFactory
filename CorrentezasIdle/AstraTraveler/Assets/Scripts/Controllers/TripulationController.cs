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
        var RecruitmentService = GameController.Instance.RecruitmentService;
        var CurrencyService = GameController.Instance.CurrencyService;

        TripulationUi.Initialize(Game, UnlockService, RecruitmentService, PurchaseService, CurrencyService);
    }
}
