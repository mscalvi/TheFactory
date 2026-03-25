using System.Collections.Generic;
using UnityEngine;
using static GameHelper;

public class ExpeditionService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private ShipState ShipState;
    private ExpeditionState Expedition;

    public void Initialize(ExpeditionState expeditionState, ShipState shipState, TickService Tick)
    {
        ShipState = shipState;
        Debug.Log($"Vida Inicial: {ShipState.Ship.CurrentLife}");

        Expedition = expeditionState;

        TickService = Tick;

        TickService.Subscribe(this);

        Debug.Log("ShipControlService On");
    }

    void OnDestroy()
    {
        TickService?.Unsubscribe(this);
    }

    public void OnTick(float dt)
    {
        if (ShipState.Ship.CurrentLife <= 0)
        {
            EndExpedition();
        }
    }

    public void EndExpedition()
    {
        Debug.Log("Game Over!");
    }


}