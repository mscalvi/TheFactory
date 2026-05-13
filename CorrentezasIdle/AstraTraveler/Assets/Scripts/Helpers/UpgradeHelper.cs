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
        Itens,
    }

    public enum UpgradeBuilding
    {
        None,
        CompanyRoom,
        PlanningRoom,
        FishingRoom,
        ArmoryRoom,
        LivingRoom,
        TreasureRoom,
        TrainingRoom,
        LaboratoryRoom,
        ShipImproveRoom,
    }

    public enum UpgradeType
    {
        Additive,
        Multiplicative,
        Change,
        Study,
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
        ShipArmor,
        ShipResistence,
        ShipRepair,
        ClickTarget,
        ClickRarity,
        ClickMax,
        WeaponDamage,
        WeaponAtackSpeed,
        ExperienceGain,
        ExperienceChance,
        ExperienceIncome,
        MarcosGain,
        MarcosChance,
        MarcosIncome,
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
