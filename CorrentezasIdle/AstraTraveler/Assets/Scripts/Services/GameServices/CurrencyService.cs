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

    void EnemyDeathReward(EnemyInstance enemy)
    {
        var total = enemy.Experience * GameState.ExpeditionState.ActualExperienceKillBonus;

        Add(CurrencyHelper.CurrencyType.Experience, total);
    }

    void DayFinishReward()
    {
        double reward = GameState.ExpeditionState.ActualDayReward;
        Add(CurrencyHelper.CurrencyType.Marcos, reward);
    }

    void NightFinishReward()
    {
        double reward = GameState.ExpeditionState.ActualNightReward;
        Add(CurrencyHelper.CurrencyType.Experience, reward);
    }

    private void MissionCompleteReward(MissionInstance mission)
    {
        if (mission.MissionStatus == MissionHelper.MissionStatus.Finished)
        {
            return;
        }
        else
        {
            mission.MissionStatus = MissionHelper.MissionStatus.Finished;
        }

        if (GameState.MissionsState.MaxRewardItens > 0)
        {
            Debug.Log($"CurrencyService - Reward da Mission {mission.Name}: {mission.Reward1Ammount} {mission.RewardType1}");
            Add(mission.RewardType1, mission.Reward1Ammount);
        }
        if (GameState.MissionsState.MaxRewardItens > 1)
        {
            Debug.Log($"CurrencyService - Reward da Mission {mission.Name}: {mission.Reward2Ammount} {mission.RewardType2}");
            Add(mission.RewardType2, mission.Reward2Ammount);
        }
        if (GameState.MissionsState.MaxRewardItens > 2)
        {
            Debug.Log($"CurrencyService - Reward da Mission {mission.Name}: {mission.Reward3Ammount} {mission.RewardType4}");
            Add(mission.RewardType3, mission.Reward3Ammount);
        }
        if (GameState.MissionsState.MaxRewardItens > 3)
        {
            Debug.Log($"CurrencyService - Reward da Mission {mission.Name}: {mission.Reward4Ammount} {mission.RewardType4}");
            Add(mission.RewardType4, mission.Reward4Ammount);
        }
    }

    private void EnemyAvailableReward(EnemyInstance enemy)
    {
        Add(CurrencyType.Knowledge, enemy.Rarity);
    }

    private void DestinationArrivalEvent()
    {
        var prestige = GameState.DataState.currencies[CurrencyHelper.CurrencyType.Prestige];

        if (prestige.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
        {
            prestige.UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
        }

        Add(CurrencyType.Prestige, GameState.ExpeditionState.ReachedDestinations);
    }


    // Event
    void OnEnable()
    {
        ExpeditionEvents.OnEnemyDeath += EnemyDeathReward;
        ExpeditionEvents.OnDayFinish += DayFinishReward;
        ExpeditionEvents.OnNightFinish += NightFinishReward;
        ExpeditionEvents.OnDestinationArrival += DestinationArrivalEvent;
        GameEvents.OnMissionComplete += MissionCompleteReward;
        GameEvents.NewEnemySeen += EnemyAvailableReward;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnEnemyDeath -= EnemyDeathReward;
        ExpeditionEvents.OnDayFinish -= DayFinishReward;
        ExpeditionEvents.OnNightFinish -= NightFinishReward;
        ExpeditionEvents.OnDestinationArrival -= DestinationArrivalEvent;
        GameEvents.OnMissionComplete -= MissionCompleteReward;
        GameEvents.NewEnemySeen -= EnemyAvailableReward;
    }
}
