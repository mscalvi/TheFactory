using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnnamedTripulationService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private ShipState ShipState;

    int tickCounter = 0;

    public void Initialize(ShipState shipState, TickService Tick)
    {
        ShipState = shipState;

        TickService = Tick;

        TickService.Subscribe(this);
    }

    public void OnTick(float dt)
    {
        tickCounter++;

        if (tickCounter >= ShipState.BaseTicksPerRepair)
        {
            tickCounter = 0;
            RepairShip();
        }
    }

    void RepairShip()
    {
        if (ShipState.Ship.CurrentLife < ShipState.Ship.MaxLife)
        {
            if (ShipState.Ship.RepairPerTripulation > 0)
            {
                double TotalRepair = ShipState.Ship.TotalTripulation * ShipState.Ship.RepairPerTripulation;
                Debug.Log($"{TotalRepair} Reparados");

                ShipState.Ship.CurrentLife += TotalRepair;
            }
        }
    }
}
