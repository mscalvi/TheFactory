using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MissionHelper;

public class MissionsService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState gameState)
    {
        GameState = gameState;

        GenerateSlots();
    }

    private void GenerateSlots()
    {
        for (int i = 0; i < GameState.MissionsState.MaxOnGoingMissions; i++)
        {
            if (GameState.MissionsState.Slots.Count > i)
                continue;

            GameState.MissionsState.Slots.Add(new MissionSlotModel());
        }
    }

    public List<MissionInstance> GenerateMissionOptions(int MaxMissionOptions)
    {
        List<MissionInstance> missionOptions = new();

        for (int i = 0; i < MaxMissionOptions; i++)
        {
            MissionInstance mission;

            mission = CreateMissionsFromTemplate();

            missionOptions.Add(mission);
        }

        return missionOptions;
    }

    private MissionSlotModel GetSlot(MissionInstance mission)
    {
        return GameState.MissionsState.Slots
            .FirstOrDefault(s => s.ActiveMission == mission);
    }

    public void CompleteMission(MissionInstance mission)
    {
        var slot = GetSlot(mission);
        if (slot == null) return;

        GameEvents.OnMissionComplete?.Invoke(mission);

        GameState.MissionsState.CompletedMissions++;

        slot.ActiveMission = null;
    }

    public void CancelMission(MissionInstance mission)
    {
        var slot = GetSlot(mission);
        if (slot == null) return;

        mission.MissionStatus = MissionStatus.Canceled;

        GameEvents.OnMissionCanceled?.Invoke(mission);

        GameState.MissionsState.CanceledMissions++;

        ApplyCooldown(slot);
    }

    private void ApplyCooldown(MissionSlotModel slot)
    {
        slot.ActiveMission = null;
        slot.CooldownEnd = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 86400;
    }

    public void AssignMissionToSlot(MissionInstance mission, MissionSlotModel slot)
    {
        slot.ActiveMission = mission;
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


    // Missions Types
    private void EnemyKillingPrepare(MissionInstance mission)
    {
        var validTargets = new List<EnemyInstance>();

        foreach (var enemy in GameState.DataState.enemies)
        {
            if (enemy.Value.UnlockStatus == UnlockHelper.UnlockStatus.Available || enemy.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                validTargets.Add(enemy.Value);

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

        mission.Description = "Eliminar " + mission.TargetValue + " " + chosenTarget.Name;
    }


    private void DaySurvivalPrepare(MissionInstance mission)
    {
        int maxSurvival = GameState.ProgressState.MaxDaysTraveling;

        int nextSurvival = 0;

        if (maxSurvival < 10)
        {
            nextSurvival = maxSurvival + 1;
        }
        else
        {
            nextSurvival = maxSurvival + (int)(maxSurvival * 0.1);
        }

        mission.TargetValue = nextSurvival;
    }

    private void DayNoDamagePrepare(MissionInstance mission)
    {
        int maxSurvival = GameState.ProgressState.MaxDaysTraveling;

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

    // Events
    void OnEnable()
    {
        GameEvents.MissionSlotAtualize += GenerateSlots;
    }

    void OnDisable()
    {
        GameEvents.MissionSlotAtualize -= GenerateSlots;
    }
}
