using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField] ShipUi ShipUi;

    private void Awake()
    {
        var Game = GameController.Instance.GameState;

        if (Game == null)
        {
            Debug.LogError("Game NULL!");
            return;
        }

        ShipUi.Initialize(Game);
    }
}
