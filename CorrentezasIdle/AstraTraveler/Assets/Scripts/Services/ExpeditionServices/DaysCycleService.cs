 using TMPro;
using UnityEngine;
using static GameHelper;

public class DaysCycleService : MonoBehaviour, ITickable
{
    private TickService TickService;
    
    private GameState GameState;
    private ExpeditionState ExpeditionState;

    float phaseTimer = 0f;

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
        phaseTimer += dt;

        if (phaseTimer >= ExpeditionState.PhaseDuration)
        {
            phaseTimer -= ExpeditionState.PhaseDuration;

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