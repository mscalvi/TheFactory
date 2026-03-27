using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CurrencyHelper;

public class ExpeditionUiService : MonoBehaviour
{
    private ShipState ShipState;
    private GameState GameState;
    private DataState DataState;
    private ExpeditionState ExpeditionState;

    [SerializeField] TextMeshProUGUI DaysPastText;
    [SerializeField] TextMeshProUGUI DaysToGoText;

    [SerializeField] TextMeshProUGUI CycleText;

    [SerializeField] TextMeshProUGUI DestinationText;
    [SerializeField] TextMeshProUGUI DestinationArrivalText;

    [SerializeField] TextMeshProUGUI CurrentLifeText;
    [SerializeField] TextMeshProUGUI TotalEnemiesText;

    [SerializeField] Transform CompanyCurrencyPanel;
    [SerializeField] Transform ExpeditionCurrencyPanel;
    [SerializeField] CurrencyDefinition CurrencyPrefab;

    Dictionary<CurrencyType, CurrencyDefinition> companyUI = new();
    Dictionary<CurrencyType, CurrencyDefinition> expeditionUI = new();

    public void Initialize(ExpeditionState expeditionState, ShipState shipState, GameState gameState, DataState db)
    {
        ShipState = shipState;

        ExpeditionState = expeditionState;

        GameState = gameState;

        DataState = db;

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

    public void CurrenciesSet()
    {
        BuildCurrencies(CurrencyScope.Company, CompanyCurrencyPanel);
        BuildCurrencies(CurrencyScope.Expedition, ExpeditionCurrencyPanel);
    }

    public void CurrencySet(CurrencyType type)
    {
        var currencies = ExpeditionState.ExpeditionCurrency;

        if (!currencies.TryGetValue(type, out var currency))
            return;

        if (currency.Scope == CurrencyScope.Company)
        {
            if (!companyUI.TryGetValue(type, out var ui))
                return;

            ui.Setup(currency, DataState);
        } else
        {
            if (!expeditionUI.TryGetValue(type, out var ui))
                return;

            ui.Setup(currency, DataState);
        }
    }

    public void BuildCurrencies(CurrencyScope scope, Transform parent)
    {
        var currencies = ExpeditionState.ExpeditionCurrency;

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        companyUI.Clear();
        expeditionUI.Clear();

        var ordered = new List<CurrencyInstance>();

        foreach (var pair in currencies)
        {
            var c = pair.Value;

            if (c.Scope != scope)
                continue;

            ordered.Add(c);
        }

        ordered.Sort((a, b) => string.Compare(a.Id, b.Id));

        foreach (var currency in ordered)
        {
            var obj = Instantiate(CurrencyPrefab, parent);

            var ui = obj.GetComponent<CurrencyDefinition>();
            ui.Setup(currency, DataState);

            if (parent == CompanyCurrencyPanel)
            {
                companyUI[currency.Type] = ui;
            }
            if (parent == ExpeditionCurrencyPanel)
            {
                expeditionUI[currency.Type] = ui;
            }
        }
    }
}
