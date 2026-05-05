using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CurrencyHelper;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class StudiesUi : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    private PurchaseService PurchaseService;

    [SerializeField] Transform CompanyCurrencyPanel;
    [SerializeField] CompanyCurrencyDefinition CurrencyPrefab;

    [SerializeField] GameObject UpgradesPanel;

    [SerializeField] StudyDefinition StudyDefinition;

    Dictionary<string, StudyDefinition> studyUi = new();
    Dictionary<CurrencyType, CompanyCurrencyDefinition> companyUI = new();

    public void Initialize(GameState gameState, PurchaseService purchaseService)
    {
        GameState = gameState;

        DataState = GameState.DataState;

        PurchaseService = purchaseService;

        BuildCurrencies(CurrencyScope.Company, CompanyCurrencyPanel);

        PopulateUpgrades();
    }

    void PopulateUpgrades()
    {
        ClearContainer();
        studyUi.Clear();

        foreach (var currency in GameState.DataState.currencies)
        {
            if (currency.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                PurchaseService.CanBuyCurrency(currency.Value.Type);
            }
        }

        foreach (var upgrade in GameState.DataState.upgrades)
        {
            if (upgrade.Value.UpgradeType != UpgradeHelper.UpgradeType.Study)
                continue;

            if (upgrade.Value.UnlockStatus == UnlockHelper.UnlockStatus.Available)
            {
                var go = Instantiate(StudyDefinition, UpgradesPanel.transform);
                var ui = go.GetComponent<StudyDefinition>();

                ui.Setup(upgrade.Value, PurchaseService);

                studyUi[upgrade.Value.Id] = ui;
            }
        }
    }

    public void CurrencySet(CurrencyType type)
    {
        var currencies = GameState.CompanyState.CompanyCurrency;

        if (!currencies.TryGetValue(type, out var currency))
            return;

        if (currency.Scope == CurrencyScope.Company)
        {
            if (!companyUI.TryGetValue(type, out var ui))
                return;

            ui.Setup(currency, DataState);
        }
    }

    // Helpers
    public void BuildCurrencies(CurrencyScope scope, Transform parent)
    {
        var currencies = GameState.CompanyState.CompanyCurrency;

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        if (scope == CurrencyScope.Company)
            companyUI.Clear();

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

            companyUI[currency.Type] = ui;
        }
    }

    void RefreshCurrencyUi(CurrencyType type, CurrencyScope scope)
    {
        CurrencySet(type);

        foreach (var upgrade in GameState.CompanyState.CompanyUpgrades)
        {
            if (upgrade.Value.Currency == type)
            {
                RefreshUpgradeUi(upgrade.Value);
            }
        }
    }

    void RefreshUpgradeUi(UpgradeInstance upgrade)
    {
        if (studyUi.TryGetValue(upgrade.Id, out var ui))
        {
            ui.Setup(upgrade, PurchaseService);
        }
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
    }

    void OnDisable()
    {
        GameEvents.OnCurrencyChange -= RefreshCurrencyUi;
        GameEvents.OnCanBuyChange -= RefreshCurrencyUi;
    }
}
