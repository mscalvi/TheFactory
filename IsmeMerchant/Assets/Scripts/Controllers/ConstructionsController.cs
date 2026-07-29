using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionController : MonoBehaviour
{
    [SerializeField] ConstructionsUi ConstructionsUi;

    private void Awake()
    {
        var Game = GameController.Instance.GameState;

        if (Game == null)
        {
            Debug.LogError("Game NULL!");
            return;
        }

        var PurchaseService = GameController.Instance.PurchaseService;

        ConstructionsUi.Initialize(Game, PurchaseService);
    }
}
