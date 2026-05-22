using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MissionHelper;

public class MissionsTrackerService : MonoBehaviour
{
    private GameState GameState;
    private MissionsState MissionsState;
    private ExpeditionState ExpeditionState;
    private MissionsService MissionsService;

    private List<MissionSlotModel> Slots;

    private List<MissionInstance> toComplete = new();

    public void Initialize(GameState gameState, MissionsService missions)
    {
        GameState = gameState;
        MissionsState = GameState.MissionsState;
        ExpeditionState = GameState.ExpeditionState;

        MissionsService = missions;

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

            mission.ActualValue++;

            if (mission.ActualValue >= mission.TargetValue)
            {
                GameEvents.OnMissionUpdate?.Invoke(mission);
                toComplete.Add(mission);
            }
        }

        CompleteQueued();
    }

    private void OnDayCycleFinish()
    {
        toComplete.Clear();

        foreach (var slot in Slots)
        {
            var mission = slot.ActiveMission;

            if (mission == null)
                continue;

            if (mission.MissionType == MissionType.DaySurvival)
            {
                if (ExpeditionState.DayCounter >= mission.TargetValue)
                {
                    toComplete.Add(mission);
                    GameEvents.OnMissionUpdate?.Invoke(mission);
                }
            }

            if (mission.MissionType == MissionType.DayNoDamage)
            {
                if (!ExpeditionState.DamageTaken)
                {
                    if (ExpeditionState.DayCounter >= mission.TargetValue)
                    {
                        toComplete.Add(mission);
                        GameEvents.OnMissionUpdate?.Invoke(mission);
                    }
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
        ExpeditionEvents.OnNightFinish += OnDayCycleFinish;
        ExpeditionEvents.OnExpeditionEnd += OnExpeditionEnd;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnEnemyDeath -= OnEnemyKilled;
        ExpeditionEvents.OnNightFinish -= OnDayCycleFinish;
        ExpeditionEvents.OnExpeditionEnd -= OnExpeditionEnd;
    }
}
