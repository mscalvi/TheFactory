using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeModel 
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public UpgradeHelper.UpgradeScope UpgradeScope { get; set; }
    public UpgradeHelper.UpgradeType UpgradeType { get; set; }
    public UpgradeHelper.EffectType EffectType { get; set; }
    public double UpgradeValue { get; set; }
    public string TargetId { get; set; }
    public string UnlockId { get; set; }
    public int MaxBuy { get; set; }


    public UnlockHelper.UnlockStatus UnlockStatus { get; set; }
}
