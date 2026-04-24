using System.Collections;
using System.Collections.Generic;
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

        GameEvents.OnCurrencyChange?.Invoke(type, currencies[type].Scope);        
    }

    public bool Spend(CurrencyType type, double amount)
    {
        var currencies = GameState.CompanyState.CompanyCurrency;
        double current = Get(type);

        if (current < amount)
            return false;

        currencies[type].Amount = current - amount;

        currencies[type].Amount = (int)currencies[type].Amount;

        GameEvents.OnCurrencyChange?.Invoke(type, currencies[type].Scope);

        return true;
    }

    // Event
    void OnEnable()
    {
        ExpeditionEvents.OnEnemyDeath += EnemyDeathReward;
        ExpeditionEvents.OnDayFinish += DayFinishReward;
        ExpeditionEvents.OnNightFinish += NightFinishReward;
        GameEvents.OnMissionComplete += MissionComplete;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnEnemyDeath -= EnemyDeathReward;
        ExpeditionEvents.OnDayFinish -= DayFinishReward;
        ExpeditionEvents.OnNightFinish -= NightFinishReward;
        GameEvents.OnMissionComplete -= MissionComplete;
    }

    void EnemyDeathReward(EnemyInstance enemy)
    {
        Add(CurrencyHelper.CurrencyType.Experience, enemy.Experience);
    }

    void DayFinishReward()
    {
        double reward = GameState.ExpeditionState.BaseDayReward;
        Add(CurrencyHelper.CurrencyType.Marcos, reward);
    }

    void NightFinishReward()
    {
        double reward = GameState.ExpeditionState.BaseNightReward;
        Add(CurrencyHelper.CurrencyType.Experience, reward);
    }

    private void MissionComplete(MissionInstance mission)
    {
        if (mission.MissionStatus == MissionHelper.MissionStatus.Finished)
        {
            return;
        } else
        {
            mission.MissionStatus = MissionHelper.MissionStatus.Finished;
        }

        if (GameState.MissionsState.MaxRewardItens > 0)
        {
            Debug.Log($"Game CurrencyService - Reward da Mission {mission.Name}: {mission.Reward1Ammount} {mission.RewardType1}");
            Add(mission.RewardType1, mission.Reward1Ammount);
        }
        if (GameState.MissionsState.MaxRewardItens > 1)
        {
            Debug.Log($"Game CurrencyService - Reward da Mission {mission.Name}: {mission.Reward2Ammount} {mission.RewardType2}");
            Add(mission.RewardType2, mission.Reward2Ammount);
        }
        if (GameState.MissionsState.MaxRewardItens > 2)
        {
            Debug.Log($"Game CurrencyService - Reward da Mission {mission.Name}: {mission.Reward3Ammount} {mission.RewardType4}");
            Add(mission.RewardType3, mission.Reward3Ammount);
        }
        if (GameState.MissionsState.MaxRewardItens > 3)
        {
            Debug.Log($"Game CurrencyService - Reward da Mission {mission.Name}: {mission.Reward4Ammount} {mission.RewardType4}");
            Add(mission.RewardType4, mission.Reward4Ammount);
        }
    }
}
