using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlchemyProductDefinition : MonoBehaviour
{
    public TMP_Text BrewName;
    public TMP_Text BrewActualValue;
    public Button BrewButton;

    private bool CanBuyBrew;

    private ProductInstance ProductInstance;
    private IngredientService IngredientService;
    private AlchemyService AlchemyService;

    public void Setup(ProductInstance productInstance, IngredientService ingredientService, GameState GameState, AlchemyService Alchemy)
    {
        ProductInstance = productInstance;

        IngredientService = ingredientService;
        AlchemyService = Alchemy;

        if (GameState.ActualLanguage == GameState.Language.English)
        {
            BrewName.text = ProductInstance.NameEN;
        }

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            BrewName.text = ProductInstance.NamePT;
        }
    }

    void OnBuyClicked()
    {
        IngredientService.BuyProduct(ProductInstance);
    }
}
