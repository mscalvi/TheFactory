using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInitializeService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState gameState)
    {
        GameState = gameState;

        if (GameState.ProgressState.m000)
        {
            LoadShip();
        }
        else
        {
            FirstShip();
        }
    }

    private void FirstShip()
    {
        GameState.ShipState.Ship = GameState.DataState.ships["s001"];
    }

    private void LoadShip()
    {

    }

}
