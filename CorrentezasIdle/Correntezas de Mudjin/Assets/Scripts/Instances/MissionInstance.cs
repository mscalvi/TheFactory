using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MissionInstance
{
    public MissionModel Model;

    public string Id;
    public string Name;
    public double RewardFactor;

    public MissionHelper.MissionRarity MissionRarity;
    public MissionHelper.MissionType MissionType;
    public MissionHelper.MissionStatus MissionStatus;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public string UnlockId;

    public string Description;

    public double TargetValue;
    public double CurrentValue;
    public double Reward;

    public List<string> TargetsIds;

    public MissionInstance(MissionModel model)
    {
        Id = model.Id;
        Name = model.Name;
        RewardFactor = model.RewardFactor;

        MissionRarity = model.MissionRarity;
        MissionType = model.MissionType;
        MissionStatus = model.MissionStatus;

        UnlockStatus = UnlockHelper.UnlockStatus.Blocked;
        UnlockId = model.UnlockId;

        Description = "";
        TargetValue = 1;
        CurrentValue = 0;
        Reward = 0;
        TargetsIds = new List<string>();
    }

    public MissionInstance(MissionInstance model)
    {
        Id = model.Id;
        Name = model.Name;
        RewardFactor = model.RewardFactor;

        MissionRarity = model.MissionRarity;
        MissionType = model.MissionType;
        MissionStatus = model.MissionStatus;

        UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
        UnlockId = model.UnlockId;

        Description = "";
        TargetValue = 1;
        CurrentValue = 0;
        Reward = 0;
        TargetsIds = new List<string>();
    }
}