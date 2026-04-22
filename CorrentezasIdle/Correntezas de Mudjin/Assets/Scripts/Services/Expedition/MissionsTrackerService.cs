using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyHelper;
using static MissionHelper;

public class MissionsTrackerService : MonoBehaviour
{
    private MissionsState MissionsState;
    private MissionsService MissionsService;
    private ExpeditionState ExpeditionState;
    private List<MissionInstance> ActiveMissions;

    public void Initialize(MissionsState missionState, MissionsService missions, ExpeditionState expedition)
    {
        MissionsService = missions;

        MissionsState = missionState;

        ExpeditionState = expedition;

        ActiveMissions = MissionsState.ActiveMissions;
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
                    MissionsService.CompleteMission(mission);
            }
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
                    MissionsService.CompleteMission(mission);
            }

            if (mission.MissionType == MissionType.DayNoDamage)
            {
                if (!ExpeditionState.DamageTaken)
                {
                    mission.CurrentValue++;

                    if (mission.CurrentValue >= mission.TargetValue)
                        MissionsService.CompleteMission(mission);
                }
            }
        }
    }
}
