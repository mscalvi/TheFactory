using System.Collections.Generic;
using UnityEngine;
using static GameHelper;

public class ExpeditionService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private ShipState ShipState;
    private GameState GameState;
    private ExpeditionState ExpeditionState;

    public void Initialize(ExpeditionState expeditionState, ShipState shipState, GameState game, TickService Tick)
    {
        ShipState = shipState;

        ExpeditionState = expeditionState;

        GameState = game;

        TickService = Tick;

        TickService.Subscribe(this);
    }

    void OnDestroy()
    {
        TickService?.Unsubscribe(this);
    }

    public void OnTick(float dt)
    {
        if (ShipState.Ship.CurrentLife <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Debug.Log("Game Over!");

        ExpeditionState.ExpeditionStatus = ExpeditionStatus.GameOver;

        RunEvents.OnExpeditionEnd?.Invoke();

        TickService.Pause();
    }

    public void NewDestinationChose()
    {
        ExpeditionState.DestinationDayCounter = 1;
    }
}