 using TMPro;
using UnityEngine;
using static GameHelper;

public class DaysCycleService : MonoBehaviour, ITickable
{
    private TickService TickService;
    
    private GameState GameState;
    private ExpeditionState ExpeditionState;

    int tickCounter = 0;

    public void Initialize(GameState gameState, ExpeditionState expeditionState, TickService Tick)
    {
        GameState = gameState;

        ExpeditionState = expeditionState;

        TickService = Tick;

        TickService.Subscribe(this);
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
                ExpeditionState.DestinationDayCounter++;
                RunEvents.OnNightFinish?.Invoke();
            } else
            {
                RunEvents.OnDayFinish?.Invoke();
            }

            if (ExpeditionState.DestinationArrival > 0)
            {
                if (ExpeditionState.DayCounter > ExpeditionState.DestinationArrival)
                {
                    Debug.Log($"Rota Finalizada");
                    RunEvents.OnDestinationArrival?.Invoke();
                }
            }
        }
    }
}