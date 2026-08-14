using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

    void StartCurrency()
    {
        CurrencyService.Add(CurrencyHelper.CurrencyType.Experience, GameState.ExpeditionState.ActualStartExperience);

        if (GameState.ExpeditionState.ActualStartExperience <= 0) return;
        ExpeditionEvents.CurrencyIncome?.Invoke(GameState.DataState.currencies[CurrencyHelper.CurrencyType.Experience], GameState.ExpeditionState.ActualStartExperience);
    }

    void EnemyDeathReward(EnemyRuntime enemy, Vector3 position)
    {
        var total = enemy.Experience * GameState.ExpeditionState.ActualExperienceKillBonus;

        CurrencyService.Add(CurrencyHelper.CurrencyType.Experience, total);
    }

    void DayFinishReward()
    {
        double reward = GameState.ExpeditionState.ActualDayReward;
        CurrencyService.Add(CurrencyHelper.CurrencyType.Marcos, reward);

        if (reward <= 0) return;
        ExpeditionEvents.CurrencyIncome?.Invoke(GameState.DataState.currencies[CurrencyHelper.CurrencyType.Marcos], reward);
    }

    void NightFinishReward()
    {
        double reward = GameState.ExpeditionState.ActualNightReward;
        CurrencyService.Add(CurrencyHelper.CurrencyType.Experience, reward);

        if (reward <= 0) return;
        ExpeditionEvents.CurrencyIncome?.Invoke(GameState.DataState.currencies[CurrencyHelper.CurrencyType.Experience], reward);
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
            ExpeditionEvents.CurrencyIncome?.Invoke(GameState.DataState.currencies[mission.RewardType1], mission.Reward1Ammount * GameState.MissionsState.RewardBonus);
            CurrencyService.Add(mission.RewardType1, mission.Reward1Ammount * GameState.MissionsState.RewardBonus);
        }
        if (GameState.MissionsState.MaxRewardItens > 1)
        {
            Debug.Log($"CurrencyService - Reward da Mission {mission.NamePT}: {mission.Reward2Ammount * GameState.MissionsState.RewardBonus} {mission.RewardType2}");
            ExpeditionEvents.CurrencyIncome?.Invoke(GameState.DataState.currencies[mission.RewardType2], mission.Reward2Ammount * GameState.MissionsState.RewardBonus);
            CurrencyService.Add(mission.RewardType2, mission.Reward2Ammount * GameState.MissionsState.RewardBonus);
        }
        if (GameState.MissionsState.MaxRewardItens > 2)
        {
            Debug.Log($"CurrencyService - Reward da Mission {mission.NamePT}: {mission.Reward3Ammount * GameState.MissionsState.RewardBonus} {mission.RewardType4}");
            ExpeditionEvents.CurrencyIncome?.Invoke(GameState.DataState.currencies[mission.RewardType3], mission.Reward3Ammount * GameState.MissionsState.RewardBonus);
            CurrencyService.Add(mission.RewardType3, mission.Reward3Ammount * GameState.MissionsState.RewardBonus);
        }
        if (GameState.MissionsState.MaxRewardItens > 3)
        {
            Debug.Log($"CurrencyService - Reward da Mission {mission.NamePT}: {mission.Reward4Ammount * GameState.MissionsState.RewardBonus} {mission.RewardType4}");
            ExpeditionEvents.CurrencyIncome?.Invoke(GameState.DataState.currencies[mission.RewardType4], mission.Reward4Ammount * GameState.MissionsState.RewardBonus);
            CurrencyService.Add(mission.RewardType4, mission.Reward4Ammount * GameState.MissionsState.RewardBonus);
        }
    }

    private void EnemyAvailableReward(EnemyInstance enemy)
    {
        if (GameState.DataState.currencies[CurrencyType.Knowledge].UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
            return;

        CurrencyService.Add(CurrencyType.Knowledge, enemy.SpawnCost);
        ExpeditionEvents.CurrencyIncome?.Invoke(GameState.DataState.currencies[CurrencyHelper.CurrencyType.Knowledge], enemy.SpawnCost);
    }

    private void DestinationArrivalReward()
    {
        // if (GameState.DataState.currencies[CurrencyType.Prestige].UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
        //    return;

        // CurrencyService.Add(CurrencyType.Prestige, 1);
    }

    private void MechanicUnlockReward(string mechanic)
    {
        Debug.Log("Mecânica Desbloqueada: " + mechanic);

        switch(mechanic)
        {
            case "Constructions":
                CurrencyService.Add(CurrencyType.Marcos, 10);
                break;
            case "Recruiting":
                CurrencyService.Add(CurrencyType.Prestige, 10);
                break;
            default:
                Debug.Log("Mecânica sem Reward: " + mechanic);
                break;
        }
    }

    // Event
    void OnEnable()
    {
        ExpeditionEvents.OnExpeditionStart += StartCurrency;
        ExpeditionEvents.OnEnemyDeath += EnemyDeathReward;
        ExpeditionEvents.OnDayFinish += DayFinishReward;
        ExpeditionEvents.OnNightFinish += NightFinishReward;
        ExpeditionEvents.OnDestinationArrival += DestinationArrivalReward;

        GameEvents.OnMissionComplete += MissionCompleteReward;
        GameEvents.NewEnemySeen += EnemyAvailableReward;
        GameEvents.OnMechanicUnlock += MechanicUnlockReward;

        GameEvents.MoneyTest += MoneyTestEvent;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnExpeditionStart -= StartCurrency;
        ExpeditionEvents.OnEnemyDeath -= EnemyDeathReward;
        ExpeditionEvents.OnDayFinish -= DayFinishReward;
        ExpeditionEvents.OnNightFinish -= NightFinishReward;
        ExpeditionEvents.OnDestinationArrival -= DestinationArrivalReward;

        GameEvents.OnMissionComplete -= MissionCompleteReward;
        GameEvents.NewEnemySeen -= EnemyAvailableReward;
        GameEvents.OnMechanicUnlock -= MechanicUnlockReward;

        GameEvents.MoneyTest -= MoneyTestEvent;
    }

    private void MoneyTestEvent()
    {
        CurrencyService.Add(CurrencyType.Marcos, 100);
    }
}
