using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MissionInstance
{
    public MissionModel Model;

    public string Id;
    public string Name;

    public double RewardFactor;
    public double Reward1Ammount;
    public double Reward2Ammount;
    public double Reward3Ammount;
    public double Reward4Ammount;
    public CurrencyHelper.CurrencyType RewardType1;
    public CurrencyHelper.CurrencyType RewardType2;
    public CurrencyHelper.CurrencyType RewardType3;
    public CurrencyHelper.CurrencyType RewardType4;

    public GameHelper.ItemRarity MissionRarity;
    public MissionHelper.MissionType MissionType;
    public MissionHelper.MissionStatus MissionStatus;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public string UnlockId;

    public string Description;

    public double TargetValue;
    public double TargetMultiplier;
    public double CurrentValue;

    public List<string> TargetsIds;

    public MissionInstance(MissionModel model)
    {
        Id = model.Id;

        Reward1Ammount = 0;
        Reward2Ammount = 0;
        Reward3Ammount = 0;
        Reward4Ammount = 0;
        RewardType1 = model.RewardType1;
        RewardType2 = model.RewardType2;
        RewardType3 = model.RewardType3;
        RewardType4 = model.RewardType4;

        MissionRarity = model.MissionRarity;
        MissionType = model.MissionType;
        MissionStatus = model.MissionStatus;

        UnlockStatus = model.UnlockStatus;
        UnlockId = model.UnlockId;

        TargetValue = 1;
        TargetMultiplier = 1;
        CurrentValue = 0;
        TargetsIds = new List<string>();
    }

    public MissionInstance(MissionInstance model)
    {
        Id = model.Id;
        Name = model.Name;

        RewardFactor = model.RewardFactor;
        Reward1Ammount = 0;
        Reward2Ammount = 0;
        Reward3Ammount = 0;
        Reward4Ammount = 0;
        RewardType1 = model.RewardType1;
        RewardType2 = model.RewardType2;
        RewardType3 = model.RewardType3;
        RewardType4 = model.RewardType4;

        MissionRarity = model.MissionRarity;
        MissionType = model.MissionType;
        MissionStatus = model.MissionStatus;

        UnlockStatus = model.UnlockStatus;
        UnlockId = model.UnlockId;

        Description = model.Description;
        TargetValue = 1;
        TargetMultiplier = 1;
        CurrentValue = 0;
        TargetsIds = new List<string>();
    }

    public string GetMissionKey()
    {
        string targetId = TargetsIds.Count > 0 ? TargetsIds[0] : "none";

        return $"{Id}_{targetId}";
    }
}