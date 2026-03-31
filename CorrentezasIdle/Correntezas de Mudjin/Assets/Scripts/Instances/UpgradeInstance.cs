using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeInstance
{ 
    public UpgradeModel Model;

    public string Id;
    public string Name;
    public string Description;

    public UpgradeHelper.UpgradeScope UpgradeScope;
    public UpgradeHelper.UpgradeType UpgradeType;
    public UpgradeHelper.EffectType EffectType;

    public double UpgradeValue;
    public double StartValue;
    public double ActualValue;

    public UpgradeHelper.TargetType TargetType;
    public string TargetId;
    public string UnlockId;
    public int MaxBuy;

    public double Cost;
    public double ActualCost;
    public double CostGrowth;
    public bool CanBuy;
    public CurrencyHelper.CurrencyType Currency;

    public UnlockHelper.UnlockStatus UnlockStatus;

    public int ActualBuy;

    public UpgradeInstance(UpgradeModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Description = model.Description;

        UpgradeScope = model.UpgradeScope;
        UpgradeType = model.UpgradeType;
        EffectType = model.EffectType;

        UpgradeValue = model.UpgradeValue;
        StartValue = model.StartValue;
        ActualValue = model.StartValue;

        TargetType = model.TargetType;
        TargetId = model.TargetId;
        UnlockId = model.UnlockId;

        ActualBuy = 0;
        MaxBuy = model.MaxBuy;

        Cost = model.Cost;
        ActualCost = model.Cost;
        CostGrowth = model.CostGrowth;

        Currency = model.Currency;

        CanBuy = false;

        UnlockStatus = model.UnlockStatus;
    }
}
