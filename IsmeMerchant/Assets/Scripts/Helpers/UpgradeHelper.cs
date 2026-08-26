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
    public enum BuildingScope
    {
        Room,
        Shop,
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
        HuntersRoom,
        StockRoom,
        ContractsShop,
        ShipsShop,
        WeaponsShop,
        AmmosShop
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
        ConstructionTime,
        ConstructionCost,
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
        WeaponCritical,
        WeaponPrecision,
        AmmoDamage,
        AmmoAmmount,
        AmmoRecharge,
        ExperiencePerKill,
        ExperienceChance,
        ExperienceIncome,
        MarcosGain,
        MarcosChance,
        MarcosIncome,
        UpgradeShipRepairRate,
        UpgradeExperiencePerKillRate,
        ProductGeneration,
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
        Construction,
        Product,
    }

}
