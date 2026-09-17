using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionCurrencyDefinition : MonoBehaviour
{
    public TMP_Text CurrencyName;
    public TMP_Text CurrencyAmount;
    public Image CurrencyIcon;

    public void Setup(CurrencyInstance currency, DataState db)
    {
        CurrencyAmount.text = Math.Floor(currency.Amount).ToString("N0");

        Sprite icon = Resources.Load<Sprite>($"Sprites/Currencies/{currency.Id}");

        if (icon != null)
            CurrencyIcon.sprite = icon;
    }
}