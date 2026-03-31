using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHelper
{
    public enum UpgradeScope
    {
        Expedition,
        Permanent,
    }

    public enum UpgradeType
    {
        Additive,
        Multiplicative,
    }

    public enum EffectType
    {
        Unlock,
        ShipLife,
        ShipArmor,
    }

    public enum TargetType 
    {
        None,
        Ship,
        Weapon,
    }

}
