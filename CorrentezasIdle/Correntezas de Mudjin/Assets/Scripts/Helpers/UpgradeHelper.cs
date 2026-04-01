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

    public enum UpgradeMenu
    {
        None,
        Ship,
        Crew,
        Room,
    }

    public enum UpgradeBuilding
    {
        None,
        CompanyRoom,
        PlanningRoom,
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
