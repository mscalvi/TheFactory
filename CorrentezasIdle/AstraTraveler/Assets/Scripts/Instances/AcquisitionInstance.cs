using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionInstance
{
    public ConstructionModel Model;

    public string Id;
    public string NamePT;
    public string NameEN;
    public string DescriptionPT;
    public string DescriptionEN;

    public UpgradeHelper.TargetType TargetType;

    public double StartCost;
    public double BaseCost;
    public double ActualCost;

    public bool CanBuy;
    public CurrencyHelper.CurrencyType Currency;

    public float StartTime;
    public float BaseTime;
    public float ActualTime;

    public double ElapsedTime;
    public bool IsRunning;

    public long StartTimestamp;
    public long FinishTimestamp;

    public int Level;

    public TripulationHelper.Type UnlockType;
    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public ConstructionInstance(ConstructionModel model)
    {
        Id = model.Id;
        NamePT = model.NamePT;
        NameEN = model.NameEN;
        DescriptionPT = model.DescriptionPT;
        DescriptionEN = model.DescriptionEN;

        TargetType = model.TargetType;

        StartCost = model.Cost;
        BaseCost = model.Cost;
        ActualCost = model.Cost;
        CanBuy = false;
        Currency = model.Currency;

        StartTime = model.Time;
        BaseTime = model.Time;
        ActualTime = model.Time;

        ElapsedTime = 0;
        IsRunning = false;

        StartTimestamp = 0;
        FinishTimestamp = 0;

        Level = model.Level;

        UnlockType = model.Type;
        UnlockId = model.UnlockId;
        UnlockStatus = model.UnlockStatus;
    }

    public ConstructionInstance()
    {

    }
}
