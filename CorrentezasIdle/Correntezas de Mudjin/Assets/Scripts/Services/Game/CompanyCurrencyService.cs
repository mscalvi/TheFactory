using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CurrencyHelper;

public class CompanyCurrencyService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState game)
    {
        GameState = game;
    }

    public double Get(CurrencyType type)
    {
        var currencies = GameState.CompanyState.CompanyCurrency;

        var currency = currencies.TryGetValue(type, out var value) ? value.Amount : 0;

        var currencyAmount = currencies[type].Amount;

        return currencyAmount;
    }

    public void Add(CurrencyType type, double amount)
    {
        var currencies = GameState.CompanyState.CompanyCurrency;

        foreach (var currency in currencies)
        {
            if (currency.Value.Type == type)
            {
                if (currency.Value.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
                    return;
            }
        }

        currencies[type].Amount = Get(type) + amount;

        currencies[type].Amount = (int)currencies[type].Amount;

        CompanyEvents.OnCurrencyChange?.Invoke(type, currencies[type].Scope);
    }

    public bool Spend(CurrencyType type, double amount)
    {
        var currencies = GameState.CompanyState.CompanyCurrency;
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
        GameEvents.OnMissionComplete += MissionComplete;
    }

    void OnDisable()
    {
        GameEvents.OnMissionComplete -= MissionComplete;
    }

    private void MissionComplete(MissionInstance mission)
    {
        if (GameState.MissionsState.MaxRewardItens > 0)
        {
            Add(mission.RewardType1, mission.Reward1Ammount);
        }
        if (GameState.MissionsState.MaxRewardItens > 1)
        {
            Add(mission.RewardType2, mission.Reward2Ammount);
        }
        if (GameState.MissionsState.MaxRewardItens > 2)
        {
            Add(mission.RewardType3, mission.Reward3Ammount);
        }
        if (GameState.MissionsState.MaxRewardItens > 3)
        {
            Add(mission.RewardType4, mission.Reward4Ammount);
        }
    }
}
