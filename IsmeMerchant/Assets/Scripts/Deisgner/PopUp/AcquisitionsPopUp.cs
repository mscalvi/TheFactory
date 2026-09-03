using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static EnemyHelper;

public class AcquisitionsPopUp : MonoBehaviour
{
    private GameState GameState;
    private PurchaseService PurchaseService;

    [SerializeField] GameObject ShopPanel;

    [SerializeField] Button CloseBtn;

    [SerializeField] TextMeshProUGUI Title;

    public TMP_Text AreaName;
    List<BuildingDefinition> unlockedBuildings;

    [SerializeField] Transform ShopsPanel;
    [SerializeField] BuildingDefinition ShopDefinition;

    Dictionary<string, CompanyUpgradeDefinition> itemsUI = new();

    [SerializeField] GameObject ItemsPanel;
    [SerializeField] CompanyUpgradeDefinition ShopItemDefinition;

    public void Show(GameState gameState, PurchaseService purchase)
    {
        GameState = gameState;
        PurchaseService = purchase;

        Hide();

        ShopPanel.SetActive(true);

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            Title.text = "Compras";
        }
        if (GameState.ActualLanguage == GameState.Language.English)
        {
            Title.text = "Shopping";
        }

        unlockedBuildings = new List<BuildingDefinition>();

        ClearMainContainer();

        foreach (var building in GameState.DataState.buildings)
        {
            Debug.Log($"Testando {building.Value.NamePT}");

            if (building.Value.Scope != UpgradeHelper.BuildingScope.Shop) continue;

            if (building.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                var obj = Instantiate(ShopDefinition, ShopsPanel);

                var ui = obj.GetComponent<BuildingDefinition>();

                ui.SetupShop(building.Value, this, GameState);

                unlockedBuildings.Add(ui);
            }
        }

        ShowItems(UpgradeHelper.UpgradeBuilding.ContractsShop);
    }

    public void ShowItems(UpgradeHelper.UpgradeBuilding type)
    {
        ClearContainer();

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            AreaName.text = type.ToString();
        }
        if (GameState.ActualLanguage == GameState.Language.English)
        {
            AreaName.text = type.ToString();
        }

        foreach (var upgrade in GameState.DataState.upgrades.Values)
        {
            if (upgrade.UnlockStatus == UnlockHelper.UnlockStatus.Available)
            {
                if (upgrade.Building != type)
                    continue;

                var go = Instantiate(ShopItemDefinition, ItemsPanel.transform);
                var ui = go.GetComponent<CompanyUpgradeDefinition>();

                ui.Setup(upgrade, PurchaseService, GameState);

                itemsUI[upgrade.Id] = ui;
            }
        }
    }

    private void ClearContainer()
    {
        foreach (Transform child in ItemsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private void ClearMainContainer()
    {
        foreach (Transform child in ShopsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Hide()
    {
        ShopPanel.SetActive(false);
    }
}
