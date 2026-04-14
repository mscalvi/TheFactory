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
    private ExpeditionPurchaseService ExpeditionPurchaseService;

    [SerializeField] TextMeshProUGUI DaysPastText;
    [SerializeField] TextMeshProUGUI DaysToGoText;

    [SerializeField] TextMeshProUGUI CycleText;

    [SerializeField] TextMeshProUGUI DestinationText;
    [SerializeField] TextMeshProUGUI DestinationArrivalText;

    [SerializeField] TextMeshProUGUI CurrentLifeText;
    [SerializeField] TextMeshProUGUI TotalEnemiesText;

    [SerializeField] Transform CompanyCurrencyPanel;
    [SerializeField] Transform ExpeditionCurrencyPanel;
    [SerializeField] ExpeditionCurrencyDefinition CurrencyPrefab;

    [SerializeField] Transform ExpeditionShipUpgradesPanel;
    [SerializeField] ExpeditionUpgradeDefinition UpgradePrefab;

    Dictionary<CurrencyType, ExpeditionCurrencyDefinition> companyUI = new();
    Dictionary<CurrencyType, ExpeditionCurrencyDefinition> expeditionUI = new();
    Dictionary<string, ExpeditionUpgradeDefinition> shipUpgradeUI = new();

    public void Initialize(ExpeditionState expeditionState, ShipState shipState, GameState gameState, DataState dataState, ExpeditionPurchaseService purchaseService)
    {
        ShipState = shipState;

        ExpeditionState = expeditionState;

        GameState = gameState;

        DataState = dataState;

        ExpeditionPurchaseService = purchaseService;
    }

    public void DestinationTextSet()
    {
        if(ExpeditionState.DestinationArrival > 0)
        {
            DaysToGoText.text = ExpeditionState.DestinationArrival.ToString();
            DestinationText.text = ExpeditionState.NewDestination.Name;
        } else
        {
            DaysToGoText.text = "";
            DestinationText.text = ExpeditionState.CurrentPath.Name;
        }

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
        CurrentLifeText.text = ShipState.Ship.CurrentLife.ToString("N0") + " / " + ShipState.Ship.MaxLife.ToString("N0");
    }

    public void EnemiesTotalSet()
    {
        TotalEnemiesText.text = ExpeditionState.ActiveEnemies.Count.ToString();
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

    public void UpgradesSet(CurrencyHelper.CurrencyType type)
    {
        foreach (var upgrade in DataState.upgrades)
        {
            if (upgrade.Value.Currency != type)
                continue;

            if (!shipUpgradeUI.TryGetValue(upgrade.Value.Id, out var ui))
                continue;

            ui.Setup(upgrade.Value, ExpeditionPurchaseService);            
        }
    }

    public void UpgradeSet(UpgradeInstance upgrade)
    {
        if (!shipUpgradeUI.TryGetValue(upgrade.Id, out var ui))
            return;

        ui.Setup(upgrade, ExpeditionPurchaseService);
    }

    // Starter Builders
    public void CurrenciesBuild()
    {
        BuildCurrencies(CurrencyScope.Company, CompanyCurrencyPanel);
        BuildCurrencies(CurrencyScope.Expedition, ExpeditionCurrencyPanel);
    }

    public void BuildCurrencies(CurrencyScope scope, Transform parent)
    {
        var currencies = ExpeditionState.ExpeditionCurrency;

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        if (scope == CurrencyScope.Company)
            companyUI.Clear();
        else
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

            var ui = obj.GetComponent<ExpeditionCurrencyDefinition>();
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

    public void BuildShipUpgrades(Transform parent)
    {
        var upgrades = DataState.upgrades;

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        foreach (var upgrade in upgrades)
        {
            if (upgrade.Value.UnlockStatus != UnlockHelper.UnlockStatus.Available)
                continue;

            if (upgrade.Value.Scope != UpgradeHelper.UpgradeScope.Expedition)
                continue;

            if (upgrade.Value.ExpeditionMenu != UpgradeHelper.UpgradeMenu.Ship)
                continue;

            var obj = Instantiate(UpgradePrefab, parent);
            var ui = obj.GetComponent<ExpeditionUpgradeDefinition>();

            ui.Setup(upgrade.Value, ExpeditionPurchaseService);

            shipUpgradeUI[upgrade.Value.Id] = ui;
        }
    }

    // Eventos
    void OnEnable()
    {
        CombatEvents.OnEnemySpawn += RefreshEnemiesUi;
        CombatEvents.OnEnemyDeath += RefreshEnemiesUi;

        ShipEvents.OnAtributeChange += RefreshShipUi;
        ShipEvents.OnUpgradeBuy += RefreshUpgradeUi;
        ShipEvents.AfterUpgradeBuy += RefreshUpgradeUi;
        ShipEvents.OnCanBuyChange += RefreshUpgradeUi;

        RunEvents.OnExpeditionStart += GameStart;
        RunEvents.OnCurrencyChange += RefreshCurrencyUi;
        RunEvents.OnDestinationChose += DestinationTextSet;
        RunEvents.OnDayFinish += DayCycleTextSet;
        RunEvents.OnNightFinish += DayCycleTextSet;
    }

    void OnDisable()
    {
        CombatEvents.OnEnemySpawn -= RefreshEnemiesUi;
        CombatEvents.OnEnemyDeath -= RefreshEnemiesUi;

        ShipEvents.OnAtributeChange -= RefreshShipUi;
        ShipEvents.OnUpgradeBuy -= RefreshUpgradeUi;
        ShipEvents.AfterUpgradeBuy -= RefreshUpgradeUi;
        ShipEvents.OnCanBuyChange -= RefreshUpgradeUi;

        RunEvents.OnExpeditionStart -= GameStart;
        RunEvents.OnCurrencyChange -= RefreshCurrencyUi;
        RunEvents.OnDestinationChose -= DestinationTextSet;
        RunEvents.OnDayFinish -= DayCycleTextSet;
        RunEvents.OnNightFinish -= DayCycleTextSet;
    }

    void GameStart()
    {
        CurrenciesBuild();
        BuildShipUpgrades(ExpeditionShipUpgradesPanel);

        foreach (var upgrade in GameState.DataState.upgrades)
        {
            if (upgrade.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                RefreshUpgradeUi(upgrade.Value);
            }
        }
    }

    void RefreshEnemiesUi(EnemyInstance enemy)
    {
        EnemiesTotalSet();
    }

    void RefreshCurrencyUi(CurrencyType type, CurrencyScope scope)
    {
        CurrencySet(type);
        UpgradesSet(type);
    }

    void RefreshShipUi()
    {
        LifeTextSet();
    }

    void RefreshUpgradeUi(UpgradeInstance upgrade)
    {
        UpgradeSet(upgrade);
    }
}
