using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyHelper;
using static MissionHelper;

public class MissionsService : MonoBehaviour
{
    private MissionsState MissionsState;
    private List<MissionInstance> ActiveMissions;

    public void Initialize(MissionsState missionState)
    {
        MissionsState = missionState;

        ActiveMissions = MissionsState.ActiveMissions;
    }

    public void CompleteMission(MissionInstance mission)
    {

    }

}
