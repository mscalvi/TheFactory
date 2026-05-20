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

    public List<MissionInstance> GenerateMissionOptions(int maxMissionOptions)
    {
        List<MissionInstance> missionOptions = new();

        int safety = 100;

        while (missionOptions.Count < maxMissionOptions && safety > 0)
        {
            safety--;

            var mission = CreateMissionsFromTemplate();

            ApplyHistoricScaling(mission);

            string key = mission.GetMissionKey();

            bool alreadyInOptions =
                missionOptions.Any(m => m.GetMissionKey() == key);

            bool alreadyActive =
                GameState.MissionsState.Slots
                    .Where(s => s.ActiveMission != null)
                    .Any(s => s.ActiveMission.GetMissionKey() == key);

            if (alreadyInOptions || alreadyActive)
                continue;

            missionOptions.Add(mission);
        }

        return missionOptions;
    }

    private void ApplyHistoricScaling(MissionInstance mission)
    {
        switch (mission.MissionType)
        {
            case MissionType.EnemyKilling:
                break;

            case MissionType.IngredientFinding:
                break;

            default:
                return;
        }

        string targetId =
            mission.TargetsIds.Count > 0
            ? mission.TargetsIds[0]
            : "none";

        var key = (mission.Id, targetId);

        if (!GameState.MissionsState.MissionHistoric.ContainsKey(key))
            return;

        int completedTimes =
            GameState.MissionsState.MissionHistoric[key];

        double multiplier = 1 + (completedTimes * 0.2);

        mission.TargetMultiplier = multiplier;

        switch (mission.MissionType)
        {
            case MissionType.EnemyKilling:
                EnemyKillingPrepare(mission);
                break;

            case MissionType.IngredientFinding:
                IngredientFindingPrepare(mission);
                break;
        }
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

        string targetId =
            mission.TargetsIds.Count > 0
            ? mission.TargetsIds[0]
            : "none";

        var key = (mission.Id, targetId);

        if (!GameState.MissionsState.MissionHistoric.ContainsKey(key))
        {
            GameState.MissionsState.MissionHistoric[key] = 0;
        }

        GameState.MissionsState.MissionHistoric[key]++;

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
            case GameHelper.ItemRarity.Common:
                mission.Reward1Ammount = 25;
                mission.Reward2Ammount = 25;
                mission.Reward3Ammount = 25;
                mission.Reward4Ammount = 25;
                break;
            case GameHelper.ItemRarity.Uncommon:
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
            if (enemy.Value.UnlockStatus == UnlockHelper.UnlockStatus.Available
                || enemy.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                validTargets.Add(enemy.Value);
            }
        }

        EnemyInstance chosenTarget;

        if (mission.TargetsIds.Count > 0)
        {
            chosenTarget = validTargets
                .First(e => e.Id == mission.TargetsIds[0]);
        }
        else
        {
            chosenTarget = validTargets[Random.Range(0, validTargets.Count)];

            mission.TargetsIds.Add(chosenTarget.Id);
        }

        double targetValue = chosenTarget.SpawnCost * 5;

        targetValue *= mission.TargetMultiplier;

        int realValue = (int)targetValue;

        if (realValue < 1)
        {
            realValue = 1;
        }

        mission.TargetValue = realValue;
               
        mission.DescriptionEN = "Eliminate " + mission.TargetValue + " " + chosenTarget.NameEN + ".";
        mission.DescriptionPT = "Eliminar " + mission.TargetValue + " " + chosenTarget.NamePT + ".";
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

        mission.DescriptionPT = "Sobreviver por " + mission.TargetValue + " dias.";
        mission.DescriptionEN = "Survive for " + mission.TargetValue + " days.";

        mission.Reward1Ammount += mission.TargetValue * 5;
    }

    private void DayNoDamagePrepare(MissionInstance mission)
    {
        int maxSurvival = GameState.ProgressState.MaxDaysTraveling;

        mission.TargetValue = maxSurvival / 2;

        mission.DescriptionPT = "Sobreviver por " + mission.TargetValue + " dias sem receber dano.";
        mission.DescriptionEN = "Survive for " + mission.TargetValue + " days without taking damage.";

        mission.Reward1Ammount += mission.TargetValue * 5;
    }

    private void IngredientFindingPrepare(MissionInstance mission)
    {
        var validTargets = new List<IngredientInstance>();

        foreach (var ingredient in GameState.DataState.ingredients)
        {
            if (ingredient.Value.UnlockStatus == UnlockHelper.UnlockStatus.Available
                || ingredient.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                validTargets.Add(ingredient.Value);
            }
        }

        IngredientInstance chosenTarget;

        if (mission.TargetsIds.Count > 0)
        {
            chosenTarget = validTargets
                .First(i => i.Id == mission.TargetsIds[0]);
        }
        else
        {
            chosenTarget = validTargets[Random.Range(0, validTargets.Count)];

            mission.TargetsIds.Add(chosenTarget.Id);
        }

        double realValue = 0;

        switch (chosenTarget.Rarity)
        {
            case GameHelper.ItemRarity.Common:
                realValue = 100 * mission.TargetMultiplier;
                break;

            case GameHelper.ItemRarity.Uncommon:
                realValue = 50 * mission.TargetMultiplier;
                break;

            case GameHelper.ItemRarity.Rare:
                realValue = 10 * mission.TargetMultiplier;
                break;

            case GameHelper.ItemRarity.Legendary:
                realValue = 1 * mission.TargetMultiplier;
                break;
        }

        mission.TargetValue = (int)realValue;
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
