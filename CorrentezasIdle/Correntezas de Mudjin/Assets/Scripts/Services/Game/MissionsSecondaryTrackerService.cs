using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyHelper;
using static MissionHelper;

public class MissionsSecondaryTrackerService : MonoBehaviour
{
    private MissionsState MissionsState;
    private MissionsService MissionsService;
    private ExpeditionState ExpeditionState;
    private List<MissionInstance> ActiveMissions;

    List<MissionInstance> toComplete = new();

    public void Initialize(MissionsState missionState, MissionsService missions, ExpeditionState expedition)
    {
        MissionsService = missions;

        MissionsState = missionState;

        ExpeditionState = expedition;

        ActiveMissions = MissionsState.ActiveMissions;

        toComplete.Clear();
    }

    private void OnExpeditionEnd()
    {
        toComplete.Clear();
    }

    // Missions Tracker
    private void OnEnemyKilled(EnemyInstance enemy)
    {
        foreach (var mission in ActiveMissions)
        {
            if (mission.MissionType == MissionType.EnemyKilling)
            {
                if (!mission.TargetsIds.Contains(enemy.Id))
                    continue;

                mission.CurrentValue++;

                if (mission.CurrentValue >= mission.TargetValue)
                    toComplete.Add(mission);
            }
        }

        foreach (var mission in toComplete)
        {
            MissionsService.CompleteMission(mission);
        }
    }

    private void OnDayFinish()
    {
        foreach ( var mission in ActiveMissions)
        {
            if (mission.MissionType == MissionType.DaySurvival)
            {
                mission.CurrentValue++;

                if (mission.CurrentValue >= mission.TargetValue)
                    toComplete.Add(mission);
            }

            if (mission.MissionType == MissionType.DayNoDamage)
            {
                if (!ExpeditionState.DamageTaken)
                {
                    mission.CurrentValue++;

                    if (mission.CurrentValue >= mission.TargetValue)
                        toComplete.Add(mission);
                }
            }
        }

        foreach (var mission in toComplete)
        {
            MissionsService.CompleteMission(mission);
        }
    }

    // Events
    void OnEnable()
    {
        ExpeditionEvents.OnEnemyDeath += OnEnemyKilled;
        ExpeditionEvents.OnNightFinish += OnDayFinish;
        ExpeditionEvents.OnExpeditionEnd += OnExpeditionEnd;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnEnemyDeath -= OnEnemyKilled;
        ExpeditionEvents.OnNightFinish -= OnDayFinish;
        ExpeditionEvents.OnExpeditionEnd -= OnExpeditionEnd;
    }
}
