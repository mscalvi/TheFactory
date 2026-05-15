using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeInstance
{
    public UpgradeModel Model;

    public string Id;

    public string NameEN;
    public string NamePT;
    public string DescriptionEN;
    public string DescriptionPT;

    public UpgradeHelper.UpgradeScope Scope;

    public UpgradeHelper.UpgradeType UpgradeType;
    public UpgradeHelper.EffectType EffectType;

    public UpgradeHelper.UpgradeMenu ExpeditionMenu;
    public UpgradeHelper.UpgradeBuilding Building;

    public double UpgradeValue;
    public double StartValue;
    public double ActualValue;

    public UpgradeHelper.TargetType TargetType;
    public string TargetId;
    public int MaxBuy;
    public int ActualBuy;

    public double Cost;
    public double ActualCost;
    public double CostGrowth;
    public CurrencyHelper.CurrencyType Currency;
    public bool CanBuy;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public UpgradeInstance(UpgradeModel model)
    {
        Id = model.Id;

        NameEN = model.NameEN;
        NamePT = model.NamePT;
        DescriptionEN = model.DescriptionEN;
        DescriptionPT = model.DescriptionPT;

        Scope = model.Scope;

        UpgradeType = model.UpgradeType;
        EffectType = model.EffectType;

        ExpeditionMenu = model.ExpeditionMenu;
        Building = model.Building;

        UpgradeValue = model.UpgradeValue;
        StartValue = model.StartValue;
        ActualValue = model.StartValue;

        TargetType = model.TargetType;
        TargetId = model.TargetId;

        ActualBuy = 0;
        MaxBuy = model.MaxBuy;

        Cost = model.Cost;
        ActualCost = model.Cost;
        CostGrowth = model.CostGrowth;
        Currency = model.Currency;
        CanBuy = false;

        UnlockId = model.UnlockId;
        UnlockStatus = model.UnlockStatus;
    }
}
