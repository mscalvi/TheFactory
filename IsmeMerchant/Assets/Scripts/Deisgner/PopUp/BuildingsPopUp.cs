using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuildingsPopUp : MonoBehaviour
{
    private GameState GameState;

    private PurchaseService PurchaseService;

    [SerializeField] GameObject UpgBuildingsPanel;

    [SerializeField] Button CloseBtn;

    [SerializeField] TextMeshProUGUI Title;

    public TMP_Text BuildingName;
    List<BuildingDefinition> unlockedBuildings;
    Dictionary<string, CompanyUpgradeDefinition> upgradesUI = new();

    [SerializeField] Transform BuildingsPanel;
    [SerializeField] BuildingDefinition BuildingDefinition;

    [SerializeField] GameObject UpgradesPanel;
    [SerializeField] CompanyUpgradeDefinition CompanyUpgradeDefinition;

    public void Show(GameState gameState, PurchaseService purchaseService)
    {
        GameState = gameState;
        PurchaseService = purchaseService;

        ClearMainContainer();
        Hide();

        UpgBuildingsPanel.SetActive(true);

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            Title.text = "Construções e Melhorias da Companhia";
        }
        if (GameState.ActualLanguage == GameState.Language.English)
        {
            Title.text = "Company's Buildings and Upgrades";
        }

        unlockedBuildings = new List<BuildingDefinition>();

        foreach (var building in GameState.DataState.buildings)
        {
            if (building.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                var obj = Instantiate(BuildingDefinition, BuildingsPanel);

                var ui = obj.GetComponent<BuildingDefinition>();

                ui.Setup(building.Value, this, GameState);

                unlockedBuildings.Add(ui);
            }
        }

        ShowUpgrades(GameState.DataState.buildings["b00"]);
    }

    public void ShowUpgrades(BuildingInstance building)
    {
        ClearContainer();
        upgradesUI.Clear();

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            BuildingName.text = building.NamePT.ToString();
        }
        if (GameState.ActualLanguage == GameState.Language.English)
        {
            BuildingName.text = building.NameEN.ToString();
        }

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

                upgradesUI[upgrade.Id] = ui;
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
    private void ClearMainContainer()
    {
        foreach (Transform child in BuildingsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Hide()
    {
        UpgBuildingsPanel.SetActive(false);
    }

    // Eventos
    void OnEnable() 
    {
        GameEvents.OnUpgradeBought += RefreshBuildingUi;
    }

    void OnDisable()
    {
        GameEvents.OnUpgradeBought -= RefreshBuildingUi;
    }

    private void RefreshBuildingUi(UpgradeInstance upgrade)
    {
        foreach (var building in GameState.DataState.buildings.Values)
        {
            if (building.Type == upgrade.Building)
            {
                ShowUpgrades(building);
            }
        }
    }
}
