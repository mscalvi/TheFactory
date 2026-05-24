using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CurrencyHelper;

public class RewardService : MonoBehaviour
{
    private GameState GameState;

    private CurrencyService CurrencyService;

    public void Initialize(GameState game, CurrencyService currency)
    {
        GameState = game;

        CurrencyService = currency;
    }

    void EnemyDeathReward(EnemyRuntime enemy)
    {
        var total = enemy.Experience * GameState.ExpeditionState.ActualExperienceKillBonus;

        CurrencyService.Add(CurrencyHelper.CurrencyType.Experience, total);
    }

    void DayFinishReward()
    {
        double reward = GameState.ExpeditionState.ActualDayReward;
        CurrencyService.Add(CurrencyHelper.CurrencyType.Marcos, reward);
    }

    void NightFinishReward()
    {
        double reward = GameState.ExpeditionState.ActualNightReward;
        CurrencyService.Add(CurrencyHelper.CurrencyType.Experience, reward);
    }

    private void MissionCompleteReward(MissionRuntime mission)
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
            Debug.Log($"CurrencyService - Reward da Mission {mission.NamePT}: {mission.Reward1Ammount * GameState.MissionsState.RewardBonus} {mission.RewardType1}");
            CurrencyService.Add(mission.RewardType1, mission.Reward1Ammount * GameState.MissionsState.RewardBonus);
        }
        if (GameState.MissionsState.MaxRewardItens > 1)
        {
            Debug.Log($"CurrencyService - Reward da Mission {mission.NamePT}: {mission.Reward2Ammount * GameState.MissionsState.RewardBonus} {mission.RewardType2}");
            CurrencyService.Add(mission.RewardType2, mission.Reward2Ammount * GameState.MissionsState.RewardBonus);
        }
        if (GameState.MissionsState.MaxRewardItens > 2)
        {
            Debug.Log($"CurrencyService - Reward da Mission {mission.NamePT}: {mission.Reward3Ammount * GameState.MissionsState.RewardBonus} {mission.RewardType4}");
            CurrencyService.Add(mission.RewardType3, mission.Reward3Ammount * GameState.MissionsState.RewardBonus);
        }
        if (GameState.MissionsState.MaxRewardItens > 3)
        {
            Debug.Log($"CurrencyService - Reward da Mission {mission.NamePT}: {mission.Reward4Ammount * GameState.MissionsState.RewardBonus} {mission.RewardType4}");
            CurrencyService.Add(mission.RewardType4, mission.Reward4Ammount * GameState.MissionsState.RewardBonus);
        }
    }

    private void EnemyAvailableReward(EnemyInstance enemy)
    {
        CurrencyService.Add(CurrencyType.Knowledge, enemy.SpawnCost);
    }

    private void DestinationArrivalEvent()
    {
        var prestige = GameState.DataState.currencies[CurrencyHelper.CurrencyType.Prestige];

        if (prestige.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
        {
            prestige.UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
        }

        CurrencyService.Add(CurrencyType.Prestige, GameState.ExpeditionState.ReachedDestinations);
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

        GameEvents.MoneyTest += MoneyTestEvent;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnEnemyDeath -= EnemyDeathReward;
        ExpeditionEvents.OnDayFinish -= DayFinishReward;
        ExpeditionEvents.OnNightFinish -= NightFinishReward;
        ExpeditionEvents.OnDestinationArrival -= DestinationArrivalEvent;

        GameEvents.OnMissionComplete -= MissionCompleteReward;
        GameEvents.NewEnemySeen -= EnemyAvailableReward;

        GameEvents.MoneyTest -= MoneyTestEvent;
    }

    private void MoneyTestEvent()
    {
        CurrencyService.Add(CurrencyType.Marcos, 100);
    }
}
