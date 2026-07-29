using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static CurrencyHelper;

public class ConstructionsUi : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    private PurchaseService PurchaseService;

    public TMP_Text Slots;
    public TMP_Text Queue;

    [SerializeField] Transform CompanyCurrencyPanel;
    [SerializeField] CompanyCurrencyDefinition CurrencyPrefab;

    [SerializeField] GameObject UpgradesPanel;

    [SerializeField] ConstructionDefinition ConstructionDefinition;

    Dictionary<string, ConstructionDefinition> constructionUi = new();
    Dictionary<CurrencyType, CompanyCurrencyDefinition> currencyUI = new();

    public void Initialize(GameState gameState, PurchaseService purchaseService)
    {
        GameState = gameState;

        DataState = GameState.DataState;

        PurchaseService = purchaseService;
    
        BuildTexts();
        BuildCurrencies(CompanyCurrencyPanel);

        PopulateUpgrades();
    }

    void PopulateUpgrades()
    {
        ClearContainer();
        constructionUi.Clear();

        foreach (var currency in GameState.DataState.currencies)
        {
            if (currency.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                PurchaseService.CanBuyCurrency(currency.Value.Type);
            }
        }

        foreach (var upgrade in GameState.DataState.constructions)
        {
            if (upgrade.Value.UnlockStatus == UnlockHelper.UnlockStatus.Available)
            {
                if (upgrade.Value.UnlockType == TripulationHelper.Type.Coach && !GameState.ProgressState.Coach)
                    continue;
                if (upgrade.Value.UnlockType == TripulationHelper.Type.Shipbuilder && !GameState.ProgressState.Shipbuilder)
                    continue;
                if (upgrade.Value.UnlockType == TripulationHelper.Type.Merchant && !GameState.ProgressState.Merchant)
                    continue;
                if (upgrade.Value.UnlockType == TripulationHelper.Type.Fisherman && !GameState.ProgressState.Fisherman)
                    continue;
                if (upgrade.Value.UnlockType == TripulationHelper.Type.Weaponsmith && !GameState.ProgressState.Weaponsmith)
                    continue;
                if (upgrade.Value.UnlockType == TripulationHelper.Type.Hunter && !GameState.ProgressState.Hunter)
                    continue;
                if (upgrade.Value.UnlockType == TripulationHelper.Type.Alchemist && !GameState.ProgressState.Alchemist)
                    continue;

                var go = Instantiate(ConstructionDefinition, UpgradesPanel.transform);
                var ui = go.GetComponent<ConstructionDefinition>();

                ui.Setup(upgrade.Value, PurchaseService, GameState);

                constructionUi[upgrade.Value.Id] = ui;
            }
        }
    }

    public void CurrencySet(CurrencyType type)
    {
        var currencies = GameState.DataState.currencies;

        if (!currencies.TryGetValue(type, out var currency))
            return;

        if (currency.Scope == CurrencyScope.Company)
        {
            if (!currencyUI.TryGetValue(type, out var ui))
                return;

            ui.Setup(currency, DataState);
        }
    }


    // Helpers
    private void BuildTexts()
    {
        Queue.text = GameState.CompanyState.ConstructionsQueue.Count.ToString("N0") + " / " + GameState.CompanyState.MaxConstructionsQueue.ToString("N0");

        Slots.text = GameState.CompanyState.ActiveConstructions.Count.ToString("N0") + " / " + GameState.CompanyState.MaxConstructionsSlots.ToString("N0");
    }
    private void BuildCurrencies(Transform parent)
    {
        var currencies = GameState.DataState.currencies;

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        currencyUI.Clear();

        var ordered = new List<CurrencyInstance>();

        foreach (var pair in currencies)
        {
            var c = pair.Value;

            if (c.Scope != CurrencyScope.Company)
                continue;

            if (c.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
                continue;

            ordered.Add(c);
        }

        ordered.Sort((a, b) => string.Compare(a.Id, b.Id));

        foreach (var currency in ordered)
        {
            var obj = Instantiate(CurrencyPrefab, parent);

            var ui = obj.GetComponent<CompanyCurrencyDefinition>();
            ui.Setup(currency, DataState);

            currencyUI[currency.Type] = ui;
        }
    }

    void RefreshCurrencyUi(CurrencyType type, CurrencyScope scope)
    {
        CurrencySet(type);

        foreach (var upgrade in GameState.DataState.constructions)
        {
            if (upgrade.Value.UnlockStatus != UnlockHelper.UnlockStatus.Available)
                continue;

            if (upgrade.Value.Currency == type)
            {
                RefreshUpgradeUi(upgrade.Value);
            }
        }
    }

    void RefreshUpgradeUi(ConstructionInstance upgrade)
    {
        BuildTexts();

        if (constructionUi.TryGetValue(upgrade.Id, out var ui))
        {
            ui.Setup(upgrade, PurchaseService, GameState);
        }
    }

    private void ReloadScreen(ConstructionInstance acq)
    {
        PopulateUpgrades();
    }

    private void ClearContainer()
    {
        foreach (Transform child in UpgradesPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Return()
    {
        SceneManager.LoadScene("LandingScene");
    }

    // Eventos
    void OnEnable()
    {
        GameEvents.OnCurrencyChange += RefreshCurrencyUi;
        GameEvents.OnCanBuyChange += RefreshCurrencyUi;
        GameEvents.OnConstructionBuy += ReloadScreen;
        GameEvents.OnConstructionFinished += ReloadScreen;
        GameEvents.OnConstructionStarted += RefreshUpgradeUi;
        GameEvents.OnConstructionUnlocked += ReloadScreen;
    }

    void OnDisable()
    {
        GameEvents.OnCurrencyChange -= RefreshCurrencyUi;
        GameEvents.OnCanBuyChange -= RefreshCurrencyUi;
        GameEvents.OnConstructionBuy -= ReloadScreen;
        GameEvents.OnConstructionFinished -= ReloadScreen;
        GameEvents.OnConstructionStarted -= RefreshUpgradeUi;
        GameEvents.OnConstructionUnlocked -= ReloadScreen;
    }
}
