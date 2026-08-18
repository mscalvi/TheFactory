using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlchemyPopUp : MonoBehaviour
{
    private GameState GameState;

    private AlchemyService AlchemyService;
    private IngredientService IngredientService;

    [SerializeField] GameObject AlchemyPanel;

    [SerializeField] Button CloseBtn;

    [SerializeField] TextMeshProUGUI Title;

    public TMP_Text LabName;
    List<LabDefinition> unlockedLabs;

    [SerializeField] Transform LabsPanel;
    [SerializeField] LabDefinition LabDefinition;

    Dictionary<string, AlchemyProductDefinition> productsUI = new();

    [SerializeField] GameObject ProductsPanel;
    [SerializeField] AlchemyProductDefinition AlchemyProductDefinition;

    public void Show(GameState gameState, AlchemyService alchemyService, IngredientService ingredients)
    {
        GameState = gameState;
        AlchemyService = alchemyService;
        IngredientService = ingredients;

        ClearMainContainer();
        Hide();

        AlchemyPanel.SetActive(true);

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            Title.text = "Alquimia";
        }
        if (GameState.ActualLanguage == GameState.Language.English)
        {
            Title.text = "Alchemy";
        }

        unlockedLabs = new List<LabDefinition>();

        foreach (var building in GameState.DataState.labs)
        {
            if (building.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                var obj = Instantiate(LabDefinition, LabsPanel);

                var ui = obj.GetComponent<LabDefinition>();

                ui.Setup(building.Value, this, GameState);

                unlockedLabs.Add(ui);
            }
        }

        ShowProducts(GameState.DataState.labs["l00"]);
    }

    public void ShowProducts(LabInstance lab)
    {
        ClearContainer();
        productsUI.Clear();

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            LabName.text = lab.NamePT.ToString();
        }
        if (GameState.ActualLanguage == GameState.Language.English)
        {
            LabName.text = lab.NameEN.ToString();
        }

        foreach (var product in GameState.DataState.products.Values)
        {
            if (product.UnlockStatus == UnlockHelper.UnlockStatus.Available || product.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                if (product.LabType != lab.Type)
                    continue;

                var go = Instantiate(AlchemyProductDefinition, ProductsPanel.transform);
                var ui = go.GetComponent<AlchemyProductDefinition>();

                ui.Setup(product, IngredientService, GameState, AlchemyService);

                productsUI[product.Id] = ui;
            }
        }

        foreach (var currency in GameState.DataState.ingredients)
        {
            if (currency.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked || currency.Value.UnlockStatus == UnlockHelper.UnlockStatus.Available)
            {
                IngredientService.CanBuyIngredient(currency.Value.Type);
            }
        }
    }

    private void ClearContainer()
    {
        foreach (Transform child in ProductsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void ClearMainContainer()
    {
        foreach (Transform child in LabsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Hide()
    {
        AlchemyPanel.SetActive(false);
    }

    // Eventos
    void OnEnable()
    {
        //GameEvents.OnProductBought += RefreshLabsUi;
    }

    void OnDisable()
    {
        //GameEvents.OnProductBought -= RefreshLabsUi;
    }

    private void RefreshLabsUi(ProductInstance product)
    {
        foreach (var lab in GameState.DataState.labs.Values)
        {
            if (lab.Type == product.LabType)
            {
                ShowProducts(lab);
            }
        }
    }
}
