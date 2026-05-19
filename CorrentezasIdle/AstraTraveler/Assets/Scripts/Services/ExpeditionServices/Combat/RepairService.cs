using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameHelper;

public class RepairService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState game)
    {
        GameState = game;
    }

    private void RepairShip()
    {
        var Ship = GameState.ExpeditionState.Ship;

        Ship.CurrentLife += Ship.ActualRepairPerTripulation * GameState.ExpeditionState.ActiveTripulation.Count;

        if (Ship.CurrentLife > Ship.ActualLife)
        {
            Ship.CurrentLife = Ship.ActualLife;
        }

        ExpeditionEvents.OnShipAtributeChange?.Invoke();
    }

    // Events
    private void OnEnable()
    {
        ExpeditionEvents.OnDayFinish += RepairShip;
    }

    private void OnDisable()
    {
        ExpeditionEvents.OnDayFinish -= RepairShip;
    }
}
