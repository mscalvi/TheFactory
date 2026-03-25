using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControlService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private ShipState ShipState;
    private GameState GameState;

    int tickCounter = 0;

    public void Initialize(ShipState shipState, GameState gameState, TickService Tick)
    {
        GameState = gameState;

        ShipState = shipState;

        TickService = Tick;

        TickService.Subscribe(this);

        Debug.Log("ShipControlService On");
    }

    public void OnTick(float dt)
    {
        tickCounter++;

        if (tickCounter >= 1)
        {
            tickCounter = 0;
        }
    }
}
