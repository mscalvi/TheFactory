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

    private List<MissionSlotModel> Slots;

    private List<MissionInstance> toComplete = new();

    public void Initialize(MissionsState missionState, MissionsService missions, ExpeditionState expedition)
    {
        MissionsService = missions;
        MissionsState = missionState;
        ExpeditionState = expedition;

        Slots = MissionsState.Slots;

        toComplete.Clear();
    }

    private void OnExpeditionEnd()
    {
        toComplete.Clear();
    }

    private void OnEnemyKilled(EnemyInstance enemy)
    {
        toComplete.Clear();

        foreach (var slot in Slots)
        {
            var mission = slot.ActiveMission;

            if (mission == null)
                continue;

            if (mission.MissionType != MissionType.EnemyKilling)
                continue;

            if (!mission.TargetsIds.Contains(enemy.Id))
                continue;

            mission.CurrentValue++;

            if (mission.CurrentValue >= mission.TargetValue)
                toComplete.Add(mission);
        }

        CompleteQueued();
    }

    private void OnDayFinish()
    {
        toComplete.Clear();

        foreach (var slot in Slots)
        {
            var mission = slot.ActiveMission;

            if (mission == null)
                continue;

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

        CompleteQueued();
    }


    private void CompleteQueued()
    {
        foreach (var mission in toComplete)
        {
            MissionsService.CompleteMission(mission);
        }

        toComplete.Clear();
    }

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