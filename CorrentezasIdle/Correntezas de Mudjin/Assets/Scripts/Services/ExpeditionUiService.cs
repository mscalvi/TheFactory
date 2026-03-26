using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExpeditionUiService : MonoBehaviour
{
    private ShipState ShipState;
    private GameState GameState;
    private ExpeditionState ExpeditionState;

    [SerializeField] TextMeshProUGUI DaysPastText;
    [SerializeField] TextMeshProUGUI DaysToGoText;

    [SerializeField] TextMeshProUGUI CycleText;

    [SerializeField] TextMeshProUGUI DestinationText;
    [SerializeField] TextMeshProUGUI DestinationArrivalText;

    [SerializeField] TextMeshProUGUI CurrentLifeText;
    [SerializeField] TextMeshProUGUI TotalEnemiesText;

    public void Initialize(ExpeditionState expeditionState, ShipState shipState, GameState gameState)
    {
        ShipState = shipState;

        ExpeditionState = expeditionState;

        GameState = gameState;

        Debug.Log("ExpeditionUiService On");
    }

    public void DestinationTextSet()
    {
        DestinationText.text = ExpeditionState.NewDestination.Name;
        DaysToGoText.text = ExpeditionState.DestinationArrival.ToString();

        DayCycleTextSet();
        LifeTextSet();
        EnemiesTotalSet();
    }

    public void DayCycleTextSet()
    {
        if (ExpeditionState.IsDay)
        {
            CycleText.text = "Dia";
            DaysPastText.text = ExpeditionState.DestinationDayCounter.ToString();
        } else
        {
            CycleText.text = "Noite";
        }
    }

    public void LifeTextSet()
    {
        CurrentLifeText.text = ShipState.Ship.CurrentLife.ToString() + " / " + ShipState.Ship.MaxLife.ToString();
    }

    public void EnemiesTotalSet()
    {
        TotalEnemiesText.text = ExpeditionState.ActiveEnemies.Count.ToString();
    }
}
