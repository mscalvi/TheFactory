using System.Collections.Generic;
using UnityEngine;
using static GameHelper;

public class ExpeditionService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private ExpeditionUiService UiService;
    private ShipState ShipState;
    private ExpeditionState ExpeditionState;

    public void Initialize(ExpeditionState expeditionState, ShipState shipState, TickService Tick, ExpeditionUiService ui)
    {
        ShipState = shipState;

        ExpeditionState = expeditionState;

        TickService = Tick;

        UiService = ui;

        TickService.Subscribe(this);

        Debug.Log("ExpeditionService On");
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

        if (ExpeditionState.ExpeditionStatus == ExpeditionStatus.Complete)
        {
            Debug.Log("Vitória!");
            TickService.Pause();
        }
    }

    public void EndExpedition()
    {
        Debug.Log("Game Over!");
        TickService.Pause();
    }

    public void NewDestinationChose()
    {
        ExpeditionState.DayCounter = 1;

        UiService.DestinationTextSet();
    }
}