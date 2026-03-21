using TMPro;
using UnityEngine;
using static GameHelper;

public class DaysCycleService : MonoBehaviour, ITickable
{
    private TickService TickService;

    [SerializeField] TextMeshProUGUI DayCounterText;
    [SerializeField] TextMeshProUGUI DestinationArrivalText;
    [SerializeField] TextMeshProUGUI DayTimeText;

    private ExpeditionState Expedition;

    int tickCounter = 0;

    public void Initialize(ExpeditionState expeditionState, TickService Tick)
    {
        Expedition = expeditionState;

        TickService = Tick;

        TickService.Subscribe(this);

        Debug.Log("DaysCicleService On");

        // Mudar para UiService
        DayTimeText.text = "Dia";
        DayCounterText.text = Expedition.DayCounter.ToString();

        if (Expedition.DestinationArrival == 0)
        {
            DestinationArrivalText.text = "...";
        }
        else
        {
            DestinationArrivalText.text = Expedition.DestinationArrival.ToString();
        }
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
                // Mudar para UiService
                DayCounterText.text = Expedition.DayCounter.ToString();
                DayTimeText.text = "Dia";
                Debug.Log($"Início do Dia {Expedition.DayCounter}");
            } else
            {
                // Mudar para UiService
                DayTimeText.text = "Noite";
                Debug.Log($"Início da Noite {Expedition.DayCounter}");
            }
        }
    }
}