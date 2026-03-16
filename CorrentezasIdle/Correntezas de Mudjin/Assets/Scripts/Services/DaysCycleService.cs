using TMPro;
using UnityEngine;

public class DaysCycleService : MonoBehaviour, ITickable
{
    TickService tick;

    const int TicksPerPhase = 75;   // Recebe de um GameService?

    int tickCounter = 0;

    public bool IsDay { get; private set; } = true; // Recebe de um GameService?

    int DayCounter = 1; // Recebe de um GameService?
    int DestinationArrival; // Recebe de um GameService?

    [SerializeField] TextMeshProUGUI DayCounterText;
    [SerializeField] TextMeshProUGUI DestinationArrivalText;
    [SerializeField] TextMeshProUGUI DayTimeText;

    void Start()
    {
        Debug.Log("DaysCicleService On");

        tick = FindObjectOfType<TickService>();
        tick.Subscribe(this);

        DayTimeText.text = "Dia";
        DayCounterText.text = DayCounter.ToString();
    }

    void OnDestroy()
    {
        tick?.Unsubscribe(this);
    }

    public void OnTick(float dt)
    {
        tickCounter++;

        if (tickCounter >= TicksPerPhase)
        {
            tickCounter = 0;
            IsDay = !IsDay;

            if (IsDay) 
            {
                DayCounter++;
                DayCounterText.text = DayCounter.ToString();
                DayTimeText.text = "Dia";
            } else
            {
                DayTimeText.text = "Noite";
            }
        }
    }
}