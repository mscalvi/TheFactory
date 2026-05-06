using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CurrencyHelper;

public class ExpeditionUi : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;
    private ExpeditionState ExpeditionState;
    private PurchaseService PurchaseService;

    [SerializeField] GameObject ShipPanel;
    [SerializeField] GameObject CrewPanel;
    [SerializeField] GameObject ItensPanel;
    [SerializeField] GameObject SettingsPanel;

    [SerializeField] TextMeshProUGUI DaysPastText;

    [SerializeField] TextMeshProUGUI CycleText;

    [SerializeField] TextMeshProUGUI PathText;

    [SerializeField] TextMeshProUGUI CurrentLifeText;

    [SerializeField] Transform CurrencyPanel;
    [SerializeField] ExpeditionCurrencyDefinition CurrencyPrefab;
    Dictionary<CurrencyType, ExpeditionCurrencyDefinition> currencyUi = new();

    [SerializeField] Transform ExpeditionShipUpgradesPanel;
    [SerializeField] Transform ExpeditionCrewUpgradesPanel;
    [SerializeField] Transform ExpeditionItensUpgradesPanel;
    [SerializeField] ExpeditionUpgradeDefinition UpgradePrefab;
    Dictionary<string, ExpeditionUpgradeDefinition> shipUpgradeUI = new();

    public void Initialize(GameState gameState, PurchaseService purchaseService)
    {               
        GameState = gameState;

        ExpeditionState = GameState.ExpeditionState;

        DataState = GameState.DataState;

        PurchaseService = purchaseService;

        DaysPastText.text = ExpeditionState.DayCounter.ToString("N0");
        CycleText.text = "Dia";
    }

    void Start()
    {
        // Inicialização dos Paineis
        HideAllMenus();
        ShipPanel.SetActive(true);
    }

    // Troca de Menu de Upgrades
    public void OpenShipMenu()
    {
        HideAllMenus();
        ShipPanel.SetActive(true);
    }
    public void OpenCrewMenu()
    {
        HideAllMenus();
        CrewPanel.SetActive(true);
    }
    public void OpenRoomMenu()
    {
        HideAllMenus();
        ItensPanel.SetActive(true);
    }
    public void OpenSettingMenu()
    {
        HideAllMenus();
        SettingsPanel.SetActive(true);
    }
    void HideAllMenus()
    {
        ShipPanel.SetActive(false);
        CrewPanel.SetActive(false);
        ItensPanel.SetActive(false);
        SettingsPanel.SetActive(false);
    }

    // Texts
    private void DayCycleTextSet()
    {
        if (ExpeditionState.IsDay)
        {
            CycleText.text = "Dia";
            DaysPastText.text = ExpeditionState.DayCounter.ToString();
        } else
        {
            CycleText.text = "Noite";
        }
    }

    private void LifeTextSet()
    {
        CurrentLifeText.text = ExpeditionState.Ship.CurrentLife.ToString("N0") + " / " + ExpeditionState.Ship.MaxLife.ToString("N0");
    }

    private void CurrencySet(CurrencyType type)
    {
        var currencies = GameState.DataState.currencies;

        if (!currencies.TryGetValue(type, out var currency))
            return;

        if (currency.Scope == CurrencyScope.Expedition)
        {
            if (!currencyUi.TryGetValue(type, out var ui))
                return;

            ui.Setup(currency, DataState);
        }
    }

    private void UpgradesSet(CurrencyHelper.CurrencyType type)
    {
        foreach (var upgrade in DataState.upgrades)
        {
            if (upgrade.Value.Currency != type)
                continue;

            if (!shipUpgradeUI.TryGetValue(upgrade.Value.Id, out var ui))
                continue;

            ui.Setup(upgrade.Value, PurchaseService);            
        }
    }

    private void UpgradeSet(UpgradeInstance upgrade)
    {
        if (!shipUpgradeUI.TryGetValue(upgrade.Id, out var ui))
            return;

        ui.Setup(upgrade, PurchaseService);
    }

    // Starter Builders
    private void CurrenciesBuild()
    {
        BuildCurrencies(CurrencyPanel);
    }

    private void BuildCurrencies(Transform parent)
    {
        var currencies = GameState.DataState.currencies;

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        currencyUi.Clear();

        var ordered = new List<CurrencyInstance>();

        foreach (var pair in currencies)
        {
            var c = pair.Value;

            if (c.Scope != CurrencyScope.Expedition)
                continue;

            ordered.Add(c);
        }

        ordered.Sort((a, b) => string.Compare(a.Id, b.Id));

        foreach (var currency in ordered)
        {
            var obj = Instantiate(CurrencyPrefab, parent);

            var ui = obj.GetComponent<ExpeditionCurrencyDefinition>();
            ui.Setup(currency, DataState);
            
            currencyUi[currency.Type] = ui;
        }
    }

    private void BuildShipUpgrades(Transform parent)
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

            ui.Setup(upgrade.Value, PurchaseService);

            shipUpgradeUI[upgrade.Value.Id] = ui;
        }
    }
    private void BuildCrewUpgrades(Transform parent)
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

            if (upgrade.Value.ExpeditionMenu != UpgradeHelper.UpgradeMenu.Crew)
                continue;

            var obj = Instantiate(UpgradePrefab, parent);
            var ui = obj.GetComponent<ExpeditionUpgradeDefinition>();

            ui.Setup(upgrade.Value, PurchaseService);

            shipUpgradeUI[upgrade.Value.Id] = ui;
        }
    }

    private void BuildItensUpgrades(Transform parent)
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

            if (upgrade.Value.ExpeditionMenu != UpgradeHelper.UpgradeMenu.Itens)
                continue;

            var obj = Instantiate(UpgradePrefab, parent);
            var ui = obj.GetComponent<ExpeditionUpgradeDefinition>();

            ui.Setup(upgrade.Value, PurchaseService);

            shipUpgradeUI[upgrade.Value.Id] = ui;
        }
    }

    // Eventos
    void OnEnable()
    {
        ExpeditionEvents.OnShipAtributeChange += RefreshShipUi;
        GameEvents.OnUpgradeBuy += RefreshUpgradeUi;
        GameEvents.OnCurrencyChange += RefreshCurrencyUi;
        GameEvents.OnCanBuyChange += RefreshCurrencyUi;

        ExpeditionEvents.OnExpeditionStart += GameStart;
        ExpeditionEvents.OnDayFinish += DayCycleTextSet;
        ExpeditionEvents.OnNightFinish += DayCycleTextSet;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnShipAtributeChange -= RefreshShipUi;
        GameEvents.OnUpgradeBuy -= RefreshUpgradeUi;
        GameEvents.OnCurrencyChange -= RefreshCurrencyUi;
        GameEvents.OnCanBuyChange -= RefreshCurrencyUi;

        ExpeditionEvents.OnExpeditionStart -= GameStart;
        ExpeditionEvents.OnDayFinish -= DayCycleTextSet;
        ExpeditionEvents.OnNightFinish -= DayCycleTextSet;
    }

    void GameStart()
    {
        CurrenciesBuild();
        BuildShipUpgrades(ExpeditionShipUpgradesPanel);
        BuildCrewUpgrades(ExpeditionCrewUpgradesPanel);
        BuildItensUpgrades(ExpeditionItensUpgradesPanel);

        foreach (var upgrade in GameState.DataState.upgrades)
        {
            if (upgrade.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                RefreshUpgradeUi(upgrade.Value);
            }
        }
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
