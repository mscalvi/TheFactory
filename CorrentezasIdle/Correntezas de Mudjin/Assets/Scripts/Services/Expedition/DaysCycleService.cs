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
                ExpeditionEvents.OnNightFinish?.Invoke();
            } else
            {
                ExpeditionEvents.OnDayFinish?.Invoke();
            }

            if (ExpeditionState.DestinationArrival > 0)
            {
                if (ExpeditionState.DayCounter > ExpeditionState.DestinationArrival)
                {
                    if (GameState.ProgressState.m000)
                    {
                        Debug.Log($"Expedition DaysCycleService - Rota Finalizada");
                        ExpeditionEvents.OnDestinationArrival?.Invoke(GameState.ExpeditionState.NewDestination);
                    } else
                    {
                        Debug.Log($"Expedition DaysCycleService - Rota Finalizada");
                        ExpeditionEvents.OnDestinationArrival?.Invoke(GameState.ExpeditionState.NewDestination);
                        ForcedEndExpedition();
                    }
                }
            }
        }
    }

    public void ForcedEndExpedition()
    {
        Debug.Log("Expedition ExpeditionService - Finalizado!");

        ExpeditionState.ExpeditionStatus = ExpeditionStatus.Finished;

        Debug.Log($"Expedition EndExpeditionService - Zerando Experience");

        GameState.CompanyState.CompanyCurrency[CurrencyHelper.CurrencyType.Experience].Amount = 0;

        ExpeditionEvents.OnShipDeath?.Invoke(true);
    }
}