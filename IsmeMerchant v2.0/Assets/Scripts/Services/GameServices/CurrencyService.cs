using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static CurrencyHelper;

public class CurrencyService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState game)
    {
        GameState = game;
    }

    public double Get(CurrencyType type)
    {
        var currencies = GameState.DataState.currencies;

        var currency = currencies.TryGetValue(type, out var value) ? value.Amount : 0;

        var currencyAmount = currencies[type].Amount;

        return currencyAmount;
    }

    public void Add(CurrencyType type, double amount)
    {
        var currencies = GameState.DataState.currencies;

        foreach (var currency in currencies)
        {
            if (currency.Value.Type == type)
            {
                if (currency.Value.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
                    return;
            }
        }

        currencies[type].Amount = Get(type) + amount;

        GameEvents.OnCurrencyChange?.Invoke(type, currencies[type].Scope);
    }

    public bool Spend(CurrencyType type, double amount)
    {
        var currencies = GameState.DataState.currencies;
        double current = Get(type);

        if (current < amount)
            return false;

        currencies[type].Amount = current - amount;

        GameEvents.OnCurrencyChange?.Invoke(type, currencies[type].Scope);

        return true;
    }
}
