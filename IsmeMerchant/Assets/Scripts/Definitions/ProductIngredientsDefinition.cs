using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductIngredientsDefinition : MonoBehaviour
{
    public TMP_Text IngredientName;
    public TMP_Text IngredientAmount;
    public Image IngredientIcon;

    public void Setup(AlchemyHelper.IngredientType ingredient, double cost, DataState data)
    {
        IngredientAmount.text = Mathf.CeilToInt((float)cost).ToString();

        foreach (var ingredientdata in data.ingredients.Values)
        {
            if (ingredientdata.Type == ingredient)
            {
                Sprite icon = Resources.Load<Sprite>($"Sprites/Ingredients/{ingredientdata.Id}");

                if (icon != null)
                    IngredientIcon.sprite = icon;
            }
        }
    }
}
