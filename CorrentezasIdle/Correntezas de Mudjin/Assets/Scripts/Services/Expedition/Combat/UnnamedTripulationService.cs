using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnnamedTripulationService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private ShipState ShipState;
    private GameState GameState;

    int tickCounter = 0;

    public void Initialize(GameState gameState, ShipState shipState, TickService Tick)
    {
        GameState = gameState;

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
            if (ShipState.Ship.CurrentRepairPerTripulation > 0)
            {
                double TotalRepair = GameState.ShipState.Ship.ActualUnnamedTripulation * ShipState.Ship.CurrentRepairPerTripulation;

                ShipState.Ship.CurrentLife += (int)TotalRepair;

                if (ShipState.Ship.CurrentLife > ShipState.Ship.MaxLife)
                {
                    ShipState.Ship.CurrentLife = (int)ShipState.Ship.MaxLife;
                }

                ExpeditionEvents.OnShipAtributeChange?.Invoke();

                Debug.Log($"Expedition UnnamedTripulationService - {TotalRepair} Reparados");
            }
        }
    }
}
