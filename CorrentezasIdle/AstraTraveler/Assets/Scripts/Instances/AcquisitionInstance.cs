using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquisitionInstance : MonoBehaviour
{
    public AcquisitionModel Model;

    public string Id;
    public string Name;
    public string Description;

    public UpgradeHelper.TargetType TargetType;
    public string TargetId;
    public string UnlockId;

    public double Cost;
    public bool CanBuy;
    public CurrencyHelper.CurrencyType Currency;

    public float Time;
    public double TotalTime;
    public double ElapsedTime;
    public bool IsRunning;

    public long StartTimestamp;
    public long FinishTimestamp;

    public TripulationHelper.Type UnlockType;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public AcquisitionInstance(AcquisitionModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Description = model.Description;

        TargetType = model.TargetType;
        TargetId = model.TargetId;
        UnlockId = model.UnlockId;

        Cost = model.Cost;

        Currency = model.Currency;

        Time = model.Time;
        TotalTime = model.Time;
        ElapsedTime = 0;
        IsRunning = false;

        StartTimestamp = 0;
        FinishTimestamp = 0;

        CanBuy = false;

        UnlockType = model.Type;
        UnlockStatus = model.UnlockStatus;
    }
}
