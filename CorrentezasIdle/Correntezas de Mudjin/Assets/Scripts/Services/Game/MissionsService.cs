using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnemyHelper;
using static MissionHelper;

public class MissionsService : MonoBehaviour
{
    private GameState GameState;
    private List<MissionInstance> ActiveMissions;

    public void Initialize(GameState gameState)
    {
        GameState = gameState;

        ActiveMissions = GameState.MissionsState.ActiveMissions;
    }


    public List<MissionInstance> GenerateMissionOptions(int MaxMissionOptions)
    {
        List<MissionInstance> missionOptions = new();

        for (int i = 0; i < MaxMissionOptions; i++)
        {
            MissionInstance mission;
            do
            {
                mission = CreateMissionsFromTemplate();
            }
            while (missionOptions.Any(m => MissionDuplicate(m, mission)));

            missionOptions.Add(mission);
        }

        return missionOptions;
    }

    public void CompleteMission(MissionInstance mission)
    {
        mission.MissionStatus = MissionStatus.Finished;

        GameEvents.OnMissionComplete?.Invoke(mission);

        RemoveMission(mission);
    }

    public void CancelMission(MissionInstance mission)
    {
        mission.MissionStatus = MissionStatus.Canceled;

        GameEvents.OnMissionCanceled?.Invoke(mission);

        RemoveMission(mission);
    }

    private void RemoveMission(MissionInstance mission)
    {
        if (mission.MissionStatus == MissionStatus.Finished)
        {
            GameState.MissionsState.CompletedMissions++;
        } else
        {
            GameState.MissionsState.CanceledMissions++;
        }

        GameState.MissionsState.ActiveMissions.Remove(mission);
    }

    // Helpers
    private MissionInstance CreateMissionsFromTemplate()
    {
        var Templates = new List<MissionInstance>();

        foreach (var missionTemplate in GameState.DataState.missions)
        {
            if (missionTemplate.Value.MissionType == MissionType.MainMission)
                continue;

            if (missionTemplate.Value.UnlockStatus == UnlockHelper.UnlockStatus.Available)
            {
                Templates.Add(missionTemplate.Value);
            }
        }

        // Trocar para ter pesos por raridade
        var template = Templates[Random.Range(0, Templates.Count)];

        var mission = new MissionInstance(template);

        switch (mission.MissionType)
        {
            case MissionType.EnemyKilling:
                EnemyKillingPrepare(mission);
                break;

            case MissionType.DaySurvival:
                DaySurvivalPrepare(mission);
                break;

            case MissionType.DayNoDamage:
                DayNoDamagePrepare(mission);
                break;

            case MissionType.IngredientFinding:
                IngredientFindingPrepare(mission);
                break;
        }

        switch (mission.MissionRarity)
        {
            case MissionRarity.Common:
                mission.Reward1Ammount = 25;
                mission.Reward2Ammount = 25;
                mission.Reward3Ammount = 25;
                mission.Reward4Ammount = 25;
                break;
            case MissionRarity.Uncommon:
                mission.Reward1Ammount = 45;
                mission.Reward2Ammount = 45;
                mission.Reward3Ammount = 45;
                mission.Reward4Ammount = 45;
                break;
        }

        return mission;
    }

    private bool MissionDuplicate(MissionInstance a, MissionInstance b)
    {
        if (a.MissionType != b.MissionType)
            return false;

        if (a.TargetsIds.Count != b.TargetsIds.Count)
            return false;

        return a.TargetsIds.SequenceEqual(b.TargetsIds);
    }

    // Missions Types
    private void EnemyKillingPrepare(MissionInstance mission)
    {
        var validTargets = new List<EnemyInstance>();

        string targets = "";

        int counter = 0;

        foreach (var enemy in GameState.DataState.enemies)
        {
            if (enemy.Value.UnlockStatus == UnlockHelper.UnlockStatus.Available || enemy.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                validTargets.Add(enemy.Value);
                counter++;
                if (counter > 1)
                {
                    targets += ", ";
                }
                targets += enemy.Value.Name;
            }
        }

        var chosenTarget = validTargets[Random.Range(0, validTargets.Count)];

        mission.TargetsIds.Add(chosenTarget.Id);

        double targetValue = 100 / chosenTarget.Cost;

        int realValue = (int)targetValue;

        if (realValue < 1)
        {
            realValue = 1;
        }

        mission.TargetValue = realValue;

        mission.Description = "Eliminar " + mission.TargetValue + " " + targets + ".";
    }


    private void DaySurvivalPrepare(MissionInstance mission)
    {
        int maxSurvival = GameState.RecordsState.MaxDaysTraveling;

        int nextSurvival = 0;

        if (maxSurvival * 0.1 < 1)
        {
            nextSurvival = maxSurvival + 1;
        } else
        {
            nextSurvival = maxSurvival + (int)(maxSurvival * 0.1);
        }

        mission.TargetValue = nextSurvival;
    }

    private void DayNoDamagePrepare(MissionInstance mission)
    {
        int maxSurvival = GameState.RecordsState.MaxDaysTraveling;

        mission.TargetValue = maxSurvival / 2;
    }

    private void IngredientFindingPrepare(MissionInstance mission)
    {
        var validTargets = new List<IngredientInstance>();

        foreach (var ingredient in GameState.DataState.ingredients)
        {
            if (ingredient.Value.UnlockStatus == UnlockHelper.UnlockStatus.Available || ingredient.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                validTargets.Add(ingredient.Value);
            }
        }

        var chosenTarget = validTargets[Random.Range(0, validTargets.Count)];

        mission.TargetsIds.Add(chosenTarget.Id);

        switch (chosenTarget.Rarity)
        {
            case IngredientHelper.IngredientRarity.Common:
                mission.TargetValue = 100;
                break;
            case IngredientHelper.IngredientRarity.Uncommon:
                mission.TargetValue = 50;
                break;
            case IngredientHelper.IngredientRarity.Rare:
                mission.TargetValue = 10;
                break;
            case IngredientHelper.IngredientRarity.Legendary:
                mission.TargetValue = 1;
                break;
        }
    }
}
