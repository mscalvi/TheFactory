 using TMPro;
using UnityEngine;
using static GameHelper;

public class DaysCycleService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private SaveService SaveService;

    private GameState GameState;
    private ExpeditionState ExpeditionState;

    public void Initialize(GameState gameState, TickService Tick, SaveService save)
    {
        GameState = gameState;

        SaveService = save;

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
        GameState.ExpeditionState.phaseTimer += dt;

        if (GameState.ExpeditionState.phaseTimer >= ExpeditionState.PhaseDuration)
        {
            GameState.ExpeditionState.phaseTimer -= ExpeditionState.PhaseDuration;

            ExpeditionState.IsDay = !ExpeditionState.IsDay;

            if (ExpeditionState.IsDay)
            {
                ExpeditionState.DayCounter++;
                ExpeditionEvents.OnNightFinish?.Invoke();
                SaveService.Save();

                if (ExpeditionState.DayCounter > ExpeditionState.NextDestination)
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