using TMPro;
using UnityEngine;
using static GameHelper;

public class DaysCycleService : MonoBehaviour, ITickable
{
    private TickService TickService;

    private ExpeditionState Expedition;

    int tickCounter = 0;

    public void Initialize(ExpeditionState expeditionState, TickService Tick)
    {
        Expedition = expeditionState;

        TickService = Tick;

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

        if (tickCounter >= Expedition.BaseTicksPerPhase)
        {
            tickCounter = 0;
            Expedition.IsDay = !Expedition.IsDay;

            if (Expedition.IsDay) 
            {
                Expedition.DayCounter++;
                Debug.Log($"Início do Dia {Expedition.DayCounter}");
            } else
            {
                // Mudar para UiService
                Debug.Log($"Início da Noite {Expedition.DayCounter}");
            }
        }
    }
}