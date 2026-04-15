using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyHelper;
using static MissionHelper;

public class MissionsTrackerService : MonoBehaviour
{
    private MissionsState MissionsState;
    private MissionsService MissionsService;
    private List<MissionInstance> ActiveMissions;

    public void Initialize(MissionsState missionState, MissionsService missions)
    {
        MissionsService = missions;

        MissionsState = missionState;

        ActiveMissions = MissionsState.ActiveMissions;
    }

    // Missions Tracker
    private void OnEnemyKilled(EnemyType type)
    {
        foreach (var mission in ActiveMissions)
        {
            if (mission.MissionType == MissionType.EnemiesKilling)
            {
                mission.CurrentValue++;

                if (mission.CurrentValue >= mission.TargetValue)
                    MissionsService.CompleteMission(mission);
            }
        }
    }
}
