using System;
using System.Collections.Generic;
using UnityEngine;
using static CurrencyHelper;

public class ExpeditionCurrencyService : MonoBehaviour
{
    private ExpeditionState ExpeditionState;
    private GameState GameState;

    public void Initialize(ExpeditionState expedition, GameState game)
    {
        ExpeditionState = expedition;
        GameState = game;
    }

    public double Get(CurrencyType type)
    {
        var currencies = ExpeditionState.ExpeditionCurrency;

        var currency = currencies.TryGetValue(type, out var value) ? value.Amount : 0;

        var currencyAmount = currencies[type].Amount;

        return currencyAmount;
    }

    public void AddCurrency(CurrencyType type, double amount)
    {
        var dataCurrencies = GameState.DataState.currencies;

        foreach(var currency in dataCurrencies)
        {
            if (currency.Value.Type == type)
            {
                if (currency.Value.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
                    return;
            }
        }
        
        var currencies = ExpeditionState.ExpeditionCurrency;

        currencies[type].Amount = Get(type) + amount;

        RunEvents.OnCurrencyChange?.Invoke(type, currencies[type].Scope);
    }

    public bool Spend(CurrencyType type, double amount)
    {
        var currencies = ExpeditionState.ExpeditionCurrency;
        double current = Get(type);

        if (current < amount)
            return false;

        currencies[type].Amount = current - amount;

        RunEvents.OnCurrencyChange?.Invoke(type, currencies[type].Scope);

        return true;
    }

    // Event

    void OnEnable()
    {
        CombatEvents.OnEnemyDeath += EnemyDeathReward;
        RunEvents.OnDayFinish += DayFinishReward;
        RunEvents.OnNightFinish += NightFinishReward;
    }

    void OnDisable()
    {
        CombatEvents.OnEnemyDeath -= EnemyDeathReward;
        RunEvents.OnDayFinish -= DayFinishReward;
        RunEvents.OnNightFinish -= NightFinishReward;
    }

    void EnemyDeathReward(EnemyInstance enemy)
    {
        AddCurrency(CurrencyHelper.CurrencyType.Experience, enemy.Experience);
    }

    void DayFinishReward()
    {
        double reward = ExpeditionState.BaseDayReward;
        AddCurrency(CurrencyHelper.CurrencyType.Marcos, reward);
    }

    void NightFinishReward()
    {
        double reward = ExpeditionState.BaseNightReward;
        AddCurrency(CurrencyHelper.CurrencyType.Experience, reward);
    }
}