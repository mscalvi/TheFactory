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

        Ship.ActualLife += Ship.ActualRepairPerTripulation * GameState.ExpeditionState.ActiveTripulation.Count;

        if (Ship.ActualLife > Ship.MaxLife)
        {
            Ship.ActualLife = Ship.MaxLife;
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
