using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquisitonsController : MonoBehaviour
{
    [SerializeField] AcquisitonsUi AcquisitonsUi;

    private void Awake()
    {
        var Game = GameController.Instance.GameState;

        if (Game == null)
        {
            Debug.LogError("Game NULL!");
            return;
        }

        var PurchaseService = GameController.Instance.PurchaseService;

        AcquisitonsUi.Initialize(Game, PurchaseService);
    }
}
