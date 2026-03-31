using System;
using System.Collections.Generic;
using UnityEngine;
using static CurrencyHelper;

public class CurrencyService : MonoBehaviour
{
    private ExpeditionState ExpeditionState;

    public void Initialize(ExpeditionState expedition)
    {
        ExpeditionState = expedition;
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
        Add(CurrencyHelper.CurrencyType.Experience, enemy.Experience);
    }

    void DayFinishReward()
    {
        double reward = ExpeditionState.BaseDayReward;
        Add(CurrencyHelper.CurrencyType.Marcos, reward);
    }

    void NightFinishReward()
    {
        double reward = ExpeditionState.BaseNightReward;
        Add(CurrencyHelper.CurrencyType.Experience, reward);
    }
}