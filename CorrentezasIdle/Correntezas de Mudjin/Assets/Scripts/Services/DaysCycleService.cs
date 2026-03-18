using TMPro;
using UnityEngine;
using static GameHelper;

public class DaysCycleService : MonoBehaviour, ITickable
{
    [SerializeField] ExpeditionService expedition;
    [SerializeField] TickService tick;

    [SerializeField] TextMeshProUGUI DayCounterText;
    [SerializeField] TextMeshProUGUI DestinationArrivalText;
    [SerializeField] TextMeshProUGUI DayTimeText;

    int tickCounter = 0;

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
        if (expedition.State != GameState.Running)
            return;

        tickCounter++;

        if (tickCounter >= expedition.BasePhaseCycleTime)
        {
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