using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static CurrencyHelper;

public class AcquisitonsUi : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    private PurchaseService PurchaseService;

    public TMP_Text Slots;
    public TMP_Text Queue;

    [SerializeField] Transform CompanyCurrencyPanel;
    [SerializeField] CompanyCurrencyDefinition CurrencyPrefab;

    [SerializeField] GameObject UpgradesPanel;

    [SerializeField] AcquisitonDefinition AcquisitonDefinition;

    Dictionary<string, AcquisitonDefinition> acquisitonUi = new();
    Dictionary<CurrencyType, CompanyCurrencyDefinition> currencyUI = new();

    public void Initialize(GameState gameState, PurchaseService purchaseService)
    {
        GameState = gameState;

        DataState = GameState.DataState;

        PurchaseService = purchaseService;

        Queue.text = GameState.CompanyState.AcquisitionsQueue.Count.ToString("N0") + " / " + GameState.CompanyState.MaxAcquisitonsQueue.ToString("N0");

        Slots.text = GameState.CompanyState.ActiveAcquisitons.Count.ToString("N0") + " / " + GameState.CompanyState.MaxAcquisitionsSlots.ToString("N0");

        BuildCurrencies(CurrencyScope.Company, CompanyCurrencyPanel);

        PopulateUpgrades();
    }

    void PopulateUpgrades()
    {
        ClearContainer();
        acquisitonUi.Clear();

        foreach (var currency in GameState.DataState.currencies)
        {
            if (currency.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                PurchaseService.CanBuyCurrency(currency.Value.Type);
            }
        }

        foreach (var upgrade in GameState.DataState.acquisitions)
        {
            if (upgrade.Value.UnlockStatus == UnlockHelper.UnlockStatus.Available)
            {
                var go = Instantiate(AcquisitonDefinition, UpgradesPanel.transform);
                var ui = go.GetComponent<AcquisitonDefinition>();

                ui.Setup(upgrade.Value, PurchaseService);

                acquisitonUi[upgrade.Value.Id] = ui;
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
    public void BuildCurrencies(CurrencyScope scope, Transform parent)
    {
        var currencies = GameState.DataState.currencies;

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        if (scope == CurrencyScope.Company)
            currencyUI.Clear();

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

            var ui = obj.GetComponent<CompanyCurrencyDefinition>();
            ui.Setup(currency, DataState);

            currencyUI[currency.Type] = ui;
        }
    }

    void RefreshCurrencyUi(CurrencyType type, CurrencyScope scope)
    {
        CurrencySet(type);

        foreach (var upgrade in GameState.DataState.acquisitions)
        {
            if (upgrade.Value.UnlockStatus != UnlockHelper.UnlockStatus.Available)
                continue;

            if (upgrade.Value.Currency == type)
            {
                RefreshUpgradeUi(upgrade.Value);
            }
        }
    }

    void RefreshUpgradeUi(AcquisitionInstance upgrade)
    {
        if (acquisitonUi.TryGetValue(upgrade.Id, out var ui))
        {
            ui.Setup(upgrade, PurchaseService);
        }
    }

    private void ReloadScreen(AcquisitionInstance acq)
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
        GameEvents.OnAcquisitionBuy += ReloadScreen;
        GameEvents.OnAcquisitionFinished += ReloadScreen;
        GameEvents.OnAcquisitionStarted += RefreshUpgradeUi;
    }

    void OnDisable()
    {
        GameEvents.OnCurrencyChange -= RefreshCurrencyUi;
        GameEvents.OnCanBuyChange -= RefreshCurrencyUi;
        GameEvents.OnAcquisitionBuy -= ReloadScreen;
        GameEvents.OnAcquisitionFinished -= ReloadScreen;
        GameEvents.OnAcquisitionStarted -= RefreshUpgradeUi;
    }
}
