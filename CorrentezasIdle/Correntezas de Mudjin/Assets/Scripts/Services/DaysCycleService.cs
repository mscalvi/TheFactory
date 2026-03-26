using TMPro;
using UnityEngine;
using static GameHelper;

public class DaysCycleService : MonoBehaviour, ITickable
{
    private TickService TickService;

    private ExpeditionState ExpeditionState;

    private ExpeditionUiService UiService;

    int tickCounter = 0;

    public void Initialize(ExpeditionState expeditionState, TickService Tick, ExpeditionUiService ui)
    {
        ExpeditionState = expeditionState;

        TickService = Tick;

        UiService = ui;

        TickService.Subscribe(this);

        Debug.Log("DaysCicleService On");
    }

    void OnDestroy()
    {
        TickService?.Unsubscribe(this);
    }

    public void OnTick(float dt)
    {
        tickCounter++;

        if (tickCounter >= ExpeditionState.BaseTicksPerPhase)
        {
            tickCounter = 0;
            ExpeditionState.IsDay = !ExpeditionState.IsDay;

            if (ExpeditionState.IsDay) 
            {
                ExpeditionState.DayCounter++;
            }

            if (ExpeditionState.DayCounter > ExpeditionState.DestinationArrival)
            {
                ExpeditionState.ExpeditionStatus = ExpeditionStatus.Complete;
            } else
            {
                UiService.DayCycleTextSet();
            }
        }
    }
}