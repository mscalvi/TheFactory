using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyHelper;
using static MissionHelper;

public class MissionsPrimaryTrackerService : MonoBehaviour
{
    private DataState DataState;
    private GameState GameState;

    public void Initialize(GameState gameState, DataState dataState)
    {
        GameState = gameState;

        DataState = dataState;
    }

    private void DestinationMissions(DestinationInstance actualDestination)
    {
        var currentMission = GameState.MainMission;

        switch (currentMission.Id)
        {
            case "m000":
                if (actualDestination.Id == "d101")
                {
                    GameState.ProgressState.m000 = true;
                    GameState.UnlockState.Company = true;
                    MissionFinisher(currentMission);
                    MissionLoader("m001");
                    Debug.Log($"Game MainMissionTrackerService - Missão 0 Fechada");
                }
                break;
            case "m002":
                if (actualDestination.Id == "d102")
                {
                    GameState.ProgressState.m002 = true;
                    MissionFinisher(currentMission);
                    MissionLoader("m001");
                    Debug.Log($"Game MainMissionTrackerService - Missão 2 Fechada");
                }
                break;
        }
    }

    private void TripulationMission(TripulationInstance Tripulation)
    {
        int tripulation = GameState.TripulationState.ActiveTripulation.Count + GameState.TripulationState.InactiveTripulation.Count;
        Debug.Log($"Game MainMissionTrackerService - Tripulation Mission Chamado: {tripulation}");

        var currentMission = GameState.MainMission;

        switch (currentMission.Id)
        {
            case "m001":
                if (tripulation > 2)
                {
                    GameState.ProgressState.m001 = true;
                    GameState.UnlockState.Bestiary = true;
                    MissionFinisher(currentMission);
                    MissionLoader("m002");
                    Debug.Log($"Game MainMissionTrackerService - Missão 1 Fechada");
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
            if (upgrade.Value.UnlockId == currentMission.Id)
            {
                upgrade.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }

        foreach (var mission in GameState.DataState.missions)
        {
            if (mission.Value.UnlockId == currentMission.Id)
            {
                mission.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }

        foreach (var tripulation in GameState.DataState.tripulations)
        {
            if (tripulation.Value.UnlockId == currentMission.Id)
            {
                tripulation.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
            }
        }

        GameEvents.MainMissionFinished?.Invoke(currentMission);
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
        ExpeditionEvents.OnDestinationArrival += DestinationMissions;
        GameEvents.OnTripulationChange += TripulationMission;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnDestinationArrival -= DestinationMissions;
        GameEvents.OnTripulationChange -= TripulationMission;
    }
}
