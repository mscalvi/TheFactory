using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyHelper;
using static MissionHelper;

public class MainMissionsTrakerService : MonoBehaviour
{
    private DataState DataState;
    private GameState GameState;

    public void Initialize(GameState gameState, DataState dataState)
    {
        GameState = gameState;

        DataState = dataState;
    }

    private void DestinationMissions()
    {
        var currentMission = GameState.MainMission;

        switch (currentMission.Id)
        {
            case "m000":                
                if (GameState.ExpeditionState.CurrentDestination.Id == "d101")
                {
                    GameState.ProgressState.m000 = true;
                    GameState.UnlockState.Company = true;
                    MissionFinisher(currentMission);
                    MissionLoader("m001");
                }
                break;
            case "m001":
                // Trocar para ver o Status da Destination?
                if (GameState.ExpeditionState.CurrentDestination.Id == "d102")
                {
                    GameState.ProgressState.m001 = true;
                    MissionFinisher(currentMission);
                    MissionLoader("m002");
                }
                break;
        }
    }

    private void MissionFinisher(MissionInstance currentMission)
    {
        currentMission.MissionStatus = MissionStatus.Finished;
        GameState.MainMission.UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;

        foreach (var upgrade in GameState.DataState.upgrades)
        {
            if (upgrade.Value.UnlockId == "m000")
            {
                upgrade.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }
    }

    private void MissionLoader(string missionId)
    {
        foreach (var mission in DataState.missions)
        {
            if (mission.Value.Id == missionId)
            {
                GameState.MainMission.UnlockStatus = UnlockHelper.UnlockStatus.Available;
                GameState.MainMission = mission.Value;
                GameState.MainMission.MissionStatus = MissionStatus.OnGoing;
            }
        }
    }

    //Events
    void OnEnable()
    {
        ProgressEvents.OnDestinationArrival += DestinationMissions;
    }

    void OnDisable()
    {
        ProgressEvents.OnDestinationArrival -= DestinationMissions;
    }
}
