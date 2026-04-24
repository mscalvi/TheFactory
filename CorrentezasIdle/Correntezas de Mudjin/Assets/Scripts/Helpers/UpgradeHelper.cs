using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHelper
{
    public enum UpgradeScope
    {
        Expedition,
        Company,
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
        Change,
    }

    public enum EffectType
    {
        None,
        Unlock,
        MissionsMax,
        MissionsReward,
        MissionsOptions,
        MissionsCancel,
        ShipMaxLife,
        ShipAbsoluteArmor,
    }

    public enum TargetType 
    {
        None,
        Meta,
        Ship,
        Weapon,
        Building,
        Missions,
    }

}
