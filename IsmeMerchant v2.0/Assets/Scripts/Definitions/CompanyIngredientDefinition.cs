using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanyIngredientDefinition : MonoBehaviour
{
    public TMP_Text IngredientName;
    public TMP_Text IngredientAmount;
    public Image IngredientIcon;

    public void Setup(IngredientInstance currency, DataState db)
    {
        IngredientAmount.text = currency.Amount.ToString("N0");

        Sprite icon = Resources.Load<Sprite>($"Sprites/Ingredients/{currency.Id}");

        if (icon != null)
            IngredientIcon.sprite = icon;
    }
}
