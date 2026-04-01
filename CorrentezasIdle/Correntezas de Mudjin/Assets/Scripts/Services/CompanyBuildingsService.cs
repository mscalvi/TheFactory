using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanyBuildingsService : MonoBehaviour
{
    private DataState DataState;

    private CompanyPurchaseService CompanyPurchaseService;

    [SerializeField] GameObject HeaderPanel;
    [SerializeField] GameObject InfoPanel;
    [SerializeField] GameObject BuildingPanel;

    [SerializeField] BuildingDefinition BuildingDefinition;
    [SerializeField] CompanyUpgradeDefinition CompanyUpgradeDefinition;

    public TMP_Text BuildingName;
    int currentIndex = 0;
    List<BuildingDefinition> unlockedBuildings;

    public void Initialize(DataState dataState, CompanyPurchaseService purchaseService)
    {
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

        Debug.Log($"Buildings Desbloquedas: {unlockedBuildings.Count}");

        if (unlockedBuildings.Count > 0)
        {
            currentIndex = 0;
            ShowBuilding(currentIndex);
        }
    }

    void ShowBuilding(int index)
    {
        var building = unlockedBuildings[index];

        Debug.Log($"Buildings: {building.building.Name}");

        BuildingName.text = building.building.Name;

        PopulateUpgrades(building);
    }

    void PopulateUpgrades(BuildingDefinition building)
    {
        ClearContainer();

        Debug.Log($"Upgrades na Building {building.building.Name}: {building.Upgrades.Count}");

        foreach (var upgrade in building.Upgrades)
        {
            Debug.Log($"Upgrade {upgrade.Name} Disponível");

            var go = Instantiate(CompanyUpgradeDefinition, BuildingPanel.transform);
            var ui = go.GetComponent<CompanyUpgradeDefinition>();

            ui.Setup(upgrade, CompanyPurchaseService);
        }
    }

    public void GoNext()
    {
        Debug.Log($"{currentIndex}");

        if(currentIndex < unlockedBuildings.Count - 1)
        {
            currentIndex++;
        } else
        {
            currentIndex = 0;
        }

        Debug.Log($"{currentIndex}");
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
        foreach (Transform child in BuildingPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
