using System;
using System.Collections.Generic;
using UnityEngine;
using static CurrencyHelper;

public class CurrencyService : MonoBehaviour
{
    private ExpeditionState ExpeditionState;
    private ExpeditionUiService UiService;

    public void Initialize(ExpeditionState expedition, ExpeditionUiService ui)
    {
        ExpeditionState = expedition;

        UiService = ui;

        UiService.CurrenciesSet();

        Debug.Log("CurrencyService On");
    }

    public double Get(CurrencyType type)
    {
        var currencies = ExpeditionState.ExpeditionCurrency;

        var currency = currencies.TryGetValue(type, out var value) ? value.Amount : 0;

        var currencyAmount = currencies[type].Amount;

        return currencyAmount;
    }

    public void Add(CurrencyType type, double amount)
    {
        var currencies = ExpeditionState.ExpeditionCurrency;

        currencies[type].Amount = Get(type) + amount;

        UiService.CurrencySet(type);

        Debug.Log($"{currencies[type].Id} : {currencies[type].Amount}");
    }

    public bool Spend(CurrencyType type, double amount)
    {
        var currencies = ExpeditionState.ExpeditionCurrency;
        double current = Get(type);

        if (current < amount)
            return false;

        currencies[type].Amount = current - amount;

        UiService.CurrencySet(type);

        Debug.Log($"{currencies[type].Id} : {currencies[type].Amount}");

        return true;
    }
}