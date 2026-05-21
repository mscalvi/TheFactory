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
        StudyRoom,
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
        GameSpeed,
        MissionsMax,
        MissionsReward,
        MissionsOptions,
        MissionsCancel,
        ShipMaxLife,
        ShipArmor,
        ShipResistence,
        ShipRepair,
        ShipRepairCost,
        ClickTarget,
        ClickRarity,
        ClickMax,
        WeaponDamage,
        WeaponAtackSpeed,
        WeaponRange,
        AmmoDamage,
        ExperiencePerKill,
        ExperienceChance,
        ExperienceIncome,
        MarcosGain,
        MarcosChance,
        MarcosIncome,
        UpgradeShipRepairRate,
        UpgradeExperiencePerKillRate,
        TripulationMax,
    }

    public enum TargetType 
    {
        None,
        Meta,
        Ship,
        Weapon,
        Building,
        Missions,
        Upgrade,
        Ammo,
    }

}
