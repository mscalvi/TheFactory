using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanyCurrencyDefinition : MonoBehaviour
{
    public TMP_Text CurrencyName;
    public TMP_Text CurrencyAmount;

    public void Setup(CurrencyInstance currency, DataState db)
    {
        CurrencyName.text = currency.Id;
        CurrencyAmount.text = currency.Amount.ToString("N0");
    }
}
