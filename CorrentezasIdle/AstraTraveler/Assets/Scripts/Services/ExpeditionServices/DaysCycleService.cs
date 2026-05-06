 using TMPro;
using UnityEngine;
using static GameHelper;

public class DaysCycleService : MonoBehaviour, ITickable
{
    private TickService TickService;
    
    private GameState GameState;
    private ExpeditionState ExpeditionState;

    int tickCounter = 0;

    public void Initialize(GameState gameState, TickService Tick)
    {
        GameState = gameState;

        ExpeditionState = GameState.ExpeditionState;

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
                ExpeditionEvents.OnNightFinish?.Invoke();

                if (ExpeditionState.DayCounter >= ExpeditionState.NextDestination)
                {
                    ExpeditionState.ReachedDestinations++;
                    ExpeditionEvents.OnDestinationArrival?.Invoke();
                }
            } else
            {
                ExpeditionEvents.OnDayFinish?.Invoke();
            }
        }
    }
}