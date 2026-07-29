using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeModel
{
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

    public UpgradeHelper.TargetType TargetType;
    public string TargetId;
    public int MaxBuy;

    public double Cost;
    public double CostGrowth;
    public CurrencyHelper.CurrencyType Currency;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;
    public UnlockHelper.UnlockTrigger UnlockTrigger;
}