using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CurrencyHelper;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class BuildingUi : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    private PurchaseService PurchaseService;

    [SerializeField] Button NextButton;
    [SerializeField] Button PreviousButton;

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

    public void Initialize(GameState gameState, PurchaseService purchaseService)
    {
        GameState = gameState;

        DataState = GameState.DataState;

        PurchaseService = purchaseService;

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

        if (unlockedBuildings.Count < 2)
        {
            NextButton.enabled = false;
            PreviousButton.enabled = false;
        }

        BuildCurrencies(CurrencyScope.Company, CompanyCurrencyPanel);
    }

    void ShowBuilding(int index)
    {
        var building = unlockedBuildings[index];

        BuildingName.text = building.building.Name;

        PopulateUpgrades(building);

        if (unlockedBuildings.Count > 1)
        {
            NextButton.enabled = true;
            PreviousButton.enabled = true;
        }
    }

    void PopulateUpgrades(BuildingDefinition building)
    {
        ClearContainer();
        upgradeUI.Clear();

        foreach (var currency in GameState.DataState.currencies)
        {
            if (currency.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                PurchaseService.CanBuyCurrency(currency.Value.Type);
            }
        }

        foreach (var upgrade in building.Upgrades)
        {
            if (upgrade.UnlockStatus == UnlockHelper.UnlockStatus.Available)
            {
                var go = Instantiate(CompanyUpgradeDefinition, UpgradesPanel.transform);
                var ui = go.GetComponent<CompanyUpgradeDefinition>();

                ui.Setup(upgrade, PurchaseService);

                upgradeUI[upgrade.Id] = ui;
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
            if (!companyUI.TryGetValue(type, out var ui))
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

        foreach (var upgrade in GameState.DataState.upgrades)
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
            if (upgrade.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                ShowBuilding(currentIndex);
                return;
            }

            ui.Setup(upgrade, PurchaseService);
        }
    }

    void RefreshBuildings()
    {
        unlockedBuildings.Clear();

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
    }

    public void GoNext()
    {
        if (unlockedBuildings.Count == 1)
        {
            return;
        }

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
        if (unlockedBuildings.Count == 1)
        {
            return;
        }

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

    public void Return()
    {
        SceneManager.LoadScene("LandingScene");
    }


    // Eventos
    void OnEnable()
    {
        GameEvents.OnCurrencyChange += RefreshCurrencyUi;
        GameEvents.OnCanBuyChange += RefreshCurrencyUi;
        GameEvents.OnBuildingUnlock += RefreshBuildings;
    }

    void OnDisable()
    {
        GameEvents.OnCurrencyChange -= RefreshCurrencyUi;
        GameEvents.OnCanBuyChange -= RefreshCurrencyUi;
        GameEvents.OnBuildingUnlock -= RefreshBuildings;
    }
}
