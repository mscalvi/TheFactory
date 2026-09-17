using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlchemyProductDefinition : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text BrewName;
    [SerializeField] private TMP_Text BrewActualValue;

    [Header("Button")]
    [SerializeField] private Button BrewButton;

    [Header("Costs")]
    [SerializeField] private Transform CostsContainer;
    [SerializeField] private ProductIngredientsDefinition IngredientCostDefinition;

    [Header("Production")]
    [SerializeField] private Slider ProgressBar;

    private ProductInstance ProductInstance;
    private IngredientService IngredientService;
    private AlchemyService AlchemyService;
    private GameState GameState;

    public void Setup(
        ProductInstance productInstance,
        IngredientService ingredientService,
        GameState gameState,
        AlchemyService alchemyService)
    {
        ProductInstance = productInstance;
        IngredientService = ingredientService;
        GameState = gameState;
        AlchemyService = alchemyService;

        BrewButton.onClick.RemoveAllListeners();
        BrewButton.onClick.AddListener(OnBuyClicked);

        SetupName();
        SetupCosts();
        Refresh();
    }

    private void SetupName()
    {
        if (GameState.ActualLanguage == GameState.Language.English)
        {
            BrewName.text = ProductInstance.NameEN;
        }
        else if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            BrewName.text = ProductInstance.NamePT;
        }
    }

    private void SetupCosts()
    {
        foreach (Transform child in CostsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var cost in ProductInstance.ActualCosts)
        {
            var obj = Instantiate(
                IngredientCostDefinition,
                CostsContainer
            );

            obj.Setup(
                cost.Key,
                cost.Value,
                GameState.DataState
            );
        }
    }

    private void OnBuyClicked()
    {
        bool bought = IngredientService.BuyProduct(ProductInstance);

        if (!bought)
            return;

        AlchemyService.StartProduction(ProductInstance);

        SetupCosts();
        Refresh();

    }

    public void Refresh()
    {
        RefreshButton();
        RefreshValue();
        RefreshProgress();
    }

    private void RefreshButton()
    {
        BrewButton.gameObject.SetActive(
            ProductInstance.UnlockStatus == UnlockHelper.UnlockStatus.Available ||
            ProductInstance.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked
        );

        BrewButton.interactable = ProductInstance.CanBuy;
    }

    private void RefreshValue()
    {
        BrewActualValue.text = ProductInstance.BuyCount.ToString();
    }

    private void RefreshProgress()
    {
        if (ProductInstance.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
        {
            ProgressBar.value = 0f;
            return;
        }

        if (ProductInstance.NextProduction == default)
        {
            ProgressBar.value = 0f;
            return;
        }

        DateTime now = DateTime.UtcNow;

        DateTime start =
            ProductInstance.NextProduction
            .Subtract(TimeSpan.FromSeconds(ProductInstance.ActualTime));

        double total =
            (ProductInstance.NextProduction - start).TotalSeconds;

        double remaining =
            (ProductInstance.NextProduction - now).TotalSeconds;

        float progress =
            1f - Mathf.Clamp01((float)(remaining / total));

        ProgressBar.value = progress;
    }

    private void Update()
    {
        if (ProductInstance == null)
            return;

        Refresh();
    }
}