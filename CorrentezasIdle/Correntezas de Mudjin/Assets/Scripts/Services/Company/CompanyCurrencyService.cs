using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CurrencyHelper;

public class CompanyCurrencyService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    public void Initialize(GameState game, DataState data)
    {
        GameState = game;

        DataState = data;
    }

    public double Get(CurrencyType type)
    {
        var currencies = GameState.CompanyCurrency;

        var currency = currencies.TryGetValue(type, out var value) ? value.Amount : 0;

        var currencyAmount = currencies[type].Amount;

        return currencyAmount;
    }

    public void Add(CurrencyType type, double amount)
    {
        var currencies = GameState.CompanyCurrency;

        currencies[type].Amount = Get(type) + amount;

        currencies[type].Amount = (int)currencies[type].Amount;

        CompanyEvents.OnCurrencyChange?.Invoke(type, currencies[type].Scope);
    }

    public bool Spend(CurrencyType type, double amount)
    {
        var currencies = GameState.CompanyCurrency;
        double current = Get(type);

        if (current < amount)
            return false;

        currencies[type].Amount = current - amount;

        currencies[type].Amount = (int)currencies[type].Amount;

        CompanyEvents.OnCurrencyChange?.Invoke(type, currencies[type].Scope);

        return true;
    }

    // Event

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }
}
