using TMPro;
using UnityEngine;

public class DaysCycleService : MonoBehaviour, ITickable
{
    public ExpeditionService expedition;

    int tickCounter = 0;

    [SerializeField] TickService tick;
    [SerializeField] TextMeshProUGUI DayCounterText;
    [SerializeField] TextMeshProUGUI DestinationArrivalText;
    [SerializeField] TextMeshProUGUI DayTimeText;

    void Start()
    {
        tick.Subscribe(this);

        // Mudar para UiService
        Debug.Log("DaysCicleService On");
        DayTimeText.text = "Dia";
        DayCounterText.text = expedition.DayCounter.ToString();
    }

    void OnDestroy()
    {
        tick?.Unsubscribe(this);
    }

    public void OnTick(float dt)
    {
        tickCounter++;

        if (tickCounter >= expedition.BasePhaseCycleTime)
        {
            Debug.Log("Período terminado.");

            tickCounter = 0;
            expedition.IsDay = !expedition.IsDay;

            if (expedition.IsDay) 
            {
                expedition.DayCounter++;
                // Mudar para UiService
                DayCounterText.text = expedition.DayCounter.ToString();
                DayTimeText.text = "Dia";
            } else
            {
                // Mudar para UiService
                DayTimeText.text = "Noite";
            }
        }
    }
}