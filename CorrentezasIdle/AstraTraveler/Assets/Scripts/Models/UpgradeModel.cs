using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade")]
public class UpgradeModel : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;

    public UpgradeHelper.UpgradeType UpgradeType;
    public UpgradeHelper.EffectType EffectType;

    public UpgradeHelper.UpgradeScope Scope;
    public UpgradeHelper.UpgradeMenu ExpeditionMenu;
    public UpgradeHelper.UpgradeBuilding Building;

    public double UpgradeValue;
    public double StartValue;

    public UpgradeHelper.TargetType TargetType;
    public string TargetId;
    public string UnlockId;
    public int MaxBuy;

    public double Cost;
    public double CostGrowth;
    public CurrencyHelper.CurrencyType Currency;

    public UnlockHelper.UnlockStatus UnlockStatus;
}