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

    [SerializeField] Transform CompanyCurrencyPanel;
    [SerializeField] CompanyCurrencyDefinition CurrencyPrefab;

    [SerializeField] GameObject UpgradesPanel;

    [SerializeField] BuildingDefinition BuildingDefinition;
    [SerializeField] CompanyUpgradeDefinition CompanyUpgradeDefinition;

    Dictionary<string, CompanyUpgradeDefinition> upgradeUI = new();
    Dictionary<CurrencyType, CompanyCurrencyDefinition> companyUI = new();

    public TMP_Text BuildingName;
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

        BuildCurrencies(CurrencyScope.Company, CompanyCurrencyPanel);
    }

    void ShowBuilding(int index, BuildingInstance building)
    {
        var buildingDef = unlockedBuildings[index];

        if (GameState.ActualLanguage == GameState.Language.English)
        {
            BuildingName.text = buildingDef.building.NameEN;
        }

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            BuildingName.text = buildingDef.building.NamePT;
        }

        PopulateUpgrades(building);
    }

    void PopulateUpgrades(BuildingInstance building)
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

        foreach (var upgrade in GameState.DataState.upgrades.Values)
        {
            if (upgrade.UnlockStatus == UnlockHelper.UnlockStatus.Available)
            {
                if (upgrade.Building != building.Type)                
                    continue;                

                var go = Instantiate(CompanyUpgradeDefinition, UpgradesPanel.transform);
                var ui = go.GetComponent<CompanyUpgradeDefinition>();

                ui.Setup(upgrade, PurchaseService, GameState);

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
                Destroy(ui.gameObject);

                upgradeUI.Remove(upgrade.Id);

                return;
            }

            ui.Setup(upgrade, PurchaseService, GameState);
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
