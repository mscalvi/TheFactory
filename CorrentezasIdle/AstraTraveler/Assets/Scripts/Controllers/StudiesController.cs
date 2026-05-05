using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudiesController : MonoBehaviour
{
    [SerializeField] StudiesUi StudiesUi;

    private void Awake()
    {
        var Game = GameController.Instance.GameState;

        if (Game == null)
        {
            Debug.LogError("Game NULL!");
            return;
        }

        var PurchaseService = GameController.Instance.PurchaseService;

        StudiesUi.Initialize(Game, PurchaseService);
    }
}
