using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CurrencyHelper;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class CompanyUiService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    private CompanyPurchaseService CompanyPurchaseService;

    [SerializeField] Transform CompanyCurrencyPanel;
    [SerializeField] CompanyCurrencyDefinition CurrencyPrefab;

    [SerializeField] GameObject UpgradesPanel;

    [SerializeField] BuildingDefinition BuildingDefinition;
    [SerializeField] CompanyUpgradeDefinition CompanyUpgradeDefinition;

    Dictionary<string, CompanyUpgradeDefinition> upgradeUI = new();
    Dictionary<CurrencyType, CompanyCurrencyDefinition> companyUI = new();

    public TMP_Text BuildingName;
    int currentIndex = 0;
    List<BuildingDefinition> unlockedBuildings;

    public void Initialize(GameState gameState, DataState dataState, CompanyPurchaseService purchaseService)
    {
        GameState = gameState;

        DataState = dataState;

        CompanyPurchaseService = purchaseService;

        unlockedBuildings = new List<BuildingDefinition>();

        foreach (var building in DataState.buildings)
        {
            if (building.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                var obj = Instantiate(BuildingDefinition);

                var ui = obj.GetComponent<BuildingDefinition>();

                ui.Setup(building.Value);

                unlockedBuildings.Add(ui);
            }
        }

        if (unlockedBuildings.Count > 0)
        {
            currentIndex = 0;
            ShowBuilding(currentIndex);
        }

        BuildCurrencies(CurrencyScope.Company, CompanyCurrencyPanel);
    }

    void ShowBuilding(int index)
    {
        var building = unlockedBuildings[index];

        BuildingName.text = building.building.Name;

        PopulateUpgrades(building);
    }

    void PopulateUpgrades(BuildingDefinition building)
    {
        ClearContainer();
        upgradeUI.Clear();

        foreach (var upgrade in building.Upgrades)
        {
            var go = Instantiate(CompanyUpgradeDefinition, UpgradesPanel.transform);
            var ui = go.GetComponent<CompanyUpgradeDefinition>();

            ui.Setup(upgrade, CompanyPurchaseService);

            upgradeUI[upgrade.Id] = ui;
        }
    }

    public void CurrencySet(CurrencyType type)
    {
        var currencies = GameState.CompanyCurrency;

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
        var currencies = GameState.CompanyCurrency;

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

    public void GoNext()
    {
        if (currentIndex < unlockedBuildings.Count - 1)
        {
            currentIndex++;
        }
        else
        {
            currentIndex = 0;
        }

        ShowBuilding(currentIndex);
    }

    public void GoPrevious()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
        }
        else
        {
            currentIndex = unlockedBuildings.Count - 1;
        }

        ShowBuilding(currentIndex);
    }

    private void ClearContainer()
    {
        foreach (Transform child in UpgradesPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }


    // Eventos
    void OnEnable()
    {
        CompanyEvents.OnCurrencyChange += RefreshCurrencyUi;
        CompanyEvents.AfterUpgradeBuy += RefreshUpgradeUi;
        CompanyEvents.OnCanBuyChange += RefreshUpgradeUi;
    }

    void OnDisable()
    {
        CompanyEvents.OnCurrencyChange -= RefreshCurrencyUi;
        CompanyEvents.AfterUpgradeBuy -= RefreshUpgradeUi;
        CompanyEvents.OnCanBuyChange += RefreshUpgradeUi;
    }

    void RefreshCurrencyUi(CurrencyType type, CurrencyScope scope)
    {
        CurrencySet(type);

        foreach (var upgrade in GameState.CompanyUpgrades)
        {
            if (upgrade.Value.Currency == type)
            {
                RefreshUpgradeUi(upgrade.Value);
            }
        }
    }

    void RefreshUpgradeUi(UpgradeInstance upgrade)
    {
        if (upgradeUI.TryGetValue(upgrade.Id, out var ui))
        {
            ui.Setup(upgrade, CompanyPurchaseService);
        }
    }
}
