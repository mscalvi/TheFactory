using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;
    private UnlockService UnlockService;

    public void Initialize(GameState game, UnlockService unlock)
    {
        GameState = game;

        DataState = GameState.DataState;

        UnlockService = unlock;
    }

    public void AddUpgrade(UpgradeInstance upgrade)
    {
        var upgrades = DataState.upgrades;

        if (upgrade.ActualBuy >= upgrade.MaxBuy && upgrade.MaxBuy > 0)
        {
            upgrade.UnlockStatus = UnlockHelper.UnlockStatus.Finished;
        }

        if (upgrade.EffectType != UpgradeHelper.EffectType.Unlock)
        {
            Recalculate();
        }
        else
        {
            UnlockService.UnlockUpgrade(upgrade);
        }
    }

    void ResetExpeditionUpgrades()
    {
        GameState.ExpeditionState.Ship.CurrentLife = GameState.ExpeditionState.Ship.BaseLife;

        foreach (var upgrade in DataState.upgrades)
        {
            if(upgrade.Value.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                upgrade.Value.ActualBuy = 0;
                upgrade.Value.ActualCost = upgrade.Value.BaseCost;
                upgrade.Value.ActualUpgradeValue = upgrade.Value.BaseUpgradeValue;

                if (upgrade.Value.UnlockStatus == UnlockHelper.UnlockStatus.Finished)
                {
                    upgrade.Value.UnlockStatus = UnlockHelper.UnlockStatus.Available;
                }
            }
        }

        Recalculate();
    }

    private void Recalculate()
    {
        if (GameState.ExpeditionState.ExpeditionStatus == GameHelper.ExpeditionStatus.Running || GameState.ExpeditionState.ExpeditionStatus == GameHelper.ExpeditionStatus.Paused)
        {
            ApplyBaseStats();

            foreach (var upgrade in DataState.upgrades)
            {
                if (upgrade.Value.Scope == UpgradeHelper.UpgradeScope.Expedition)
                {
                    if (upgrade.Value.UpgradeType == UpgradeHelper.UpgradeType.Additive)
                    {
                        ApplyUpgrade(upgrade.Value);
                    }
                }
            }

            foreach (var upgrade in DataState.upgrades)
            {
                if (upgrade.Value.Scope == UpgradeHelper.UpgradeScope.Expedition)
                {
                    if (upgrade.Value.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
                    {
                        ApplyUpgrade(upgrade.Value);
                    }
                }
            }
        }
        else
        {
            ApplyStartStats();

            foreach (var upgrade in DataState.upgrades)
            {
                if (upgrade.Value.Scope == UpgradeHelper.UpgradeScope.Company)
                {
                    if (upgrade.Value.UpgradeType == UpgradeHelper.UpgradeType.Additive)
                    {
                        ApplyUpgrade(upgrade.Value);
                    }
                }
            }

            foreach (var upgrade in DataState.upgrades)
            {
                if (upgrade.Value.Scope == UpgradeHelper.UpgradeScope.Company)
                {
                    if (upgrade.Value.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
                    {
                        ApplyUpgrade(upgrade.Value);
                    }
                }
            }
        }

        ExpeditionEvents.OnShipAtributeChange?.Invoke();
    }

    private void ApplyBaseStats()
    {
        var Ship = GameState.ExpeditionState.Ship;

        Ship.ActualLife = Ship.BaseLife;
        Ship.ActualArmor = Ship.BaseArmor;
        Ship.ActualResistence = Ship.BaseResistence;
        Ship.ActualRepairPerTripulation = Ship.BaseRepairPerTripulation;

        foreach (var weapon in DataState.weapons)
        {
            weapon.Value.ActualDamage = weapon.Value.BaseDamage;
            weapon.Value.ActualRange = weapon.Value.BaseRange;
            weapon.Value.ActualAttackSpeed = weapon.Value.BaseAttackSpeed;
            weapon.Value.ActualPrecision = weapon.Value.BasePrecision;
            weapon.Value.ActualCriticalDamage = weapon.Value.BaseCriticalDamage;
        }

        foreach (var ammo in DataState.ammos)
        {
            ammo.Value.ActualDamage = ammo.Value.BaseDamage;
            ammo.Value.ActualRecharge = ammo.Value.BaseRecharge;
            ammo.Value.ActualAmmount = ammo.Value.BaseAmmount;
            ammo.Value.ActualProjectileSpeed = ammo.Value.BaseProjectileSpeed;
        }

        GameState.ExpeditionState.ActualExperienceKillBonus = GameState.ExpeditionState.BaseExperienceKillBonus;
        GameState.ExpeditionState.ActualDayReward = GameState.ExpeditionState.BaseDayReward;
        GameState.ExpeditionState.ActualNightReward = GameState.ExpeditionState.BaseNightReward;
    }

    private void ApplyStartStats()
    {
        var Ship = GameState.ExpeditionState.Ship;

        GameState.MissionsState.MaxCancelableMissions = 0;
        GameState.MissionsState.MaxOnGoingMissions = 0;
        GameState.MissionsState.MaxRewardItens = 1;
        GameState.MissionsState.RewardBonus = 1;
        GameState.MissionsState.MaxMissionsOptions = 1;

        Ship.BaseLife = Ship.StartLife;
        Ship.BaseArmor = Ship.StartArmor;
        Ship.BaseResistence = Ship.StartResistence;
        Ship.BaseRepairPerTripulation = Ship.StartRepairPerTripulation;

        foreach (var weapon in DataState.weapons)
        {
            weapon.Value.BaseDamage = weapon.Value.StartDamage;
            weapon.Value.BaseRange = weapon.Value.StartRange;
            weapon.Value.BaseAttackSpeed = weapon.Value.StartAttackSpeed;
            weapon.Value.BasePrecision = weapon.Value.StartPrecision;
            weapon.Value.BaseCriticalDamage = weapon.Value.StartCriticalDamage;
        }

        foreach (var ammo in DataState.ammos)
        {
            ammo.Value.BaseDamage = ammo.Value.StartDamage;
            ammo.Value.BaseRecharge = ammo.Value.StartRecharge;
            ammo.Value.BaseAmmount = ammo.Value.StartAmmount;
            ammo.Value.BaseProjectileSpeed = ammo.Value.StartProjectileSpeed;
        }

        GameState.ExpeditionState.BaseExperienceKillBonus = GameState.ExpeditionState.StartExperienceKillBonus;
        GameState.ExpeditionState.BaseDayReward = GameState.ExpeditionState.StartDayReward;
        GameState.ExpeditionState.BaseNightReward = GameState.ExpeditionState.StartNightReward;
    }

    private void ApplyUpgrade(UpgradeInstance upgrade)
    {
        upgrade.CurrentValue = upgrade.FirstValue;

        switch (upgrade.TargetType)
        {
            case UpgradeHelper.TargetType.Ship:
                switch (upgrade.EffectType)
                {
                    case UpgradeHelper.EffectType.ShipMaxLife:
                        ShipMaxLifeModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.ShipArmor:
                        ShipArmorModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.ShipResistence:
                        ShipResistenceModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.ShipRepair:
                        ShipRepairModifier(upgrade);
                        break;
                    default:
                        Debug.Log($"UPGRADE NÃO IMPLEMENTADO! {upgrade.NamePT}");
                        break;
                }
                break;
            case UpgradeHelper.TargetType.Weapon:
                switch (upgrade.EffectType)
                {
                    case UpgradeHelper.EffectType.WeaponDamage:
                        WeaponDamageModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.WeaponAtackSpeed:
                        WeaponAtkSpeedModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.WeaponRange:
                        WeaponRangeModifier(upgrade);
                        break;
                    default:
                        Debug.Log($"UPGRADE NÃO IMPLEMENTADO! {upgrade.NamePT}");
                        break;
                }
                break;
            case UpgradeHelper.TargetType.Ammo:
                switch (upgrade.EffectType)
                {
                    case UpgradeHelper.EffectType.AmmoDamage:
                        AmmoDamageModifier(upgrade);
                        break;
                    default:
                        Debug.Log($"UPGRADE NÃO IMPLEMENTADO! {upgrade.NamePT}");
                        break;
                }
                break;
            case UpgradeHelper.TargetType.Missions:
                switch (upgrade.EffectType)
                {
                    case UpgradeHelper.EffectType.MissionsMax:
                        MissionsMaxModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.MissionsReward:
                        MissionsRewardModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.MissionsOptions:
                        MissionsOptionsModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.MissionsCancel:
                        MissionsCancelModifier(upgrade);
                        break;
                    default:
                        Debug.Log($"UPGRADE NÃO IMPLEMENTADO! {upgrade.NamePT}");
                        break;
                }
                break;
            case UpgradeHelper.TargetType.Upgrade:
                switch (upgrade.EffectType)
                {
                    case UpgradeHelper.EffectType.UpgradeExperiencePerKillRate:
                        ExperiencePerKillRateModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.UpgradeShipRepairRate:
                        ShipRepairRateModifier(upgrade);
                        break;
                    default:
                        Debug.Log($"UPGRADE NÃO IMPLEMENTADO! {upgrade.NamePT}");
                        break;
                }
                break;
            case UpgradeHelper.TargetType.Meta:
                switch (upgrade.EffectType)
                {
                    case UpgradeHelper.EffectType.ExperiencePerKill:
                        ExperienceKillModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.ExperienceIncome:
                        ExperienceIncomeModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.ClickTarget:
                        ClickTargetModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.ClickMax:
                        ClickMaxModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.ClickRarity:
                        ClickRarityModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.GameSpeed:
                        GameSpeedModifier(upgrade);
                        break;
                    default:
                        Debug.Log($"UPGRADE NÃO IMPLEMENTADO! {upgrade.NamePT}");
                        break;
                }
                break;

        }
    }

    private void CalculateUpgradeValue(UpgradeInstance upgrade)
    {
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.CurrentValue *= (1 + upgrade.ActualUpgradeValue);
            }
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.CurrentValue += upgrade.ActualUpgradeValue;
            }
        }
    }

    // Modificadores Ship
    private void ShipMaxLifeModifier(UpgradeInstance upgrade)
    {
        var ship = GameState.ExpeditionState.Ship;
        double healthPercent = ship.CurrentLife / ship.ActualLife;

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            CalculateUpgradeValue(upgrade);

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                ship.ActualLife *= upgrade.CurrentValue;
            }

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
            {
                ship.BaseLife *= upgrade.CurrentValue;
            }
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            CalculateUpgradeValue(upgrade);

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                ship.ActualLife += upgrade.CurrentValue;
            }

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
            {
                ship.BaseLife += upgrade.CurrentValue;
            }
        }

        ship.CurrentLife = ship.ActualLife * healthPercent;
        if (ship.CurrentLife > ship.ActualLife)
        {
            ship.CurrentLife = ship.ActualLife;
        }
    }
    private void ShipArmorModifier(UpgradeInstance upgrade)
    {
        var ship = GameState.ExpeditionState.Ship;

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            CalculateUpgradeValue(upgrade);

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                ship.ActualArmor *= upgrade.CurrentValue;
            }

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
            {
                ship.BaseArmor *= upgrade.CurrentValue;
            }
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            CalculateUpgradeValue(upgrade);

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                ship.ActualArmor += upgrade.CurrentValue;
            }

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
            {
                ship.BaseArmor += upgrade.CurrentValue;
            }
        }
    }
    private void ShipResistenceModifier(UpgradeInstance upgrade)
    {
        var ship = GameState.ExpeditionState.Ship;

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            CalculateUpgradeValue(upgrade);

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                ship.ActualResistence *= upgrade.CurrentValue;
            }

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
            {
                ship.BaseResistence *= upgrade.CurrentValue;
            }
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            CalculateUpgradeValue(upgrade);

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                ship.ActualResistence += upgrade.CurrentValue;
            }

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
            {
                ship.BaseResistence += upgrade.CurrentValue;
            }
        }
    }
    private void ShipRepairModifier(UpgradeInstance upgrade)
    {
        var ship = GameState.ExpeditionState.Ship;

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            CalculateUpgradeValue(upgrade);

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                ship.ActualRepairPerTripulation *= upgrade.CurrentValue;
            }

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
            {
                ship.BaseRepairPerTripulation *= upgrade.CurrentValue;
            }
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            CalculateUpgradeValue(upgrade);

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                ship.ActualRepairPerTripulation += upgrade.CurrentValue;
            }

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
            {
                ship.BaseRepairPerTripulation += upgrade.CurrentValue;
            }
        }
    }

    // Modificadores Weapons
    private void WeaponDamageModifier(UpgradeInstance upgrade)
    {
        DataState.weapons.TryGetValue(upgrade.TargetId, out var weapon);

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            CalculateUpgradeValue(upgrade);

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                weapon.ActualDamage *= upgrade.CurrentValue;
            }

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
            {
                weapon.BaseDamage *= upgrade.CurrentValue;
            }
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            CalculateUpgradeValue(upgrade);

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                weapon.ActualDamage += upgrade.CurrentValue;
            }

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
            {
                weapon.BaseDamage += upgrade.CurrentValue;
            }
        }
    }
    private void WeaponAtkSpeedModifier(UpgradeInstance upgrade)
    {
        DataState.weapons.TryGetValue(upgrade.TargetId, out var weapon);

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            CalculateUpgradeValue(upgrade);

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                weapon.ActualAttackSpeed *= upgrade.CurrentValue;
            }

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
            {
                weapon.BaseAttackSpeed *= upgrade.CurrentValue;
            }
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            CalculateUpgradeValue(upgrade);

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                weapon.ActualAttackSpeed += upgrade.CurrentValue;
            }

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
            {
                weapon.BaseAttackSpeed += upgrade.CurrentValue;
            }
        }
    }
    private void WeaponRangeModifier(UpgradeInstance upgrade)
    {
        DataState.weapons.TryGetValue(upgrade.TargetId, out var weapon);

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            CalculateUpgradeValue(upgrade);

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                weapon.ActualRange *= upgrade.CurrentValue;
            }

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
            {
                weapon.BaseRange *= upgrade.CurrentValue;
            }
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            CalculateUpgradeValue(upgrade);

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                weapon.ActualRange += upgrade.CurrentValue;
            }

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
            {
                weapon.BaseRange += upgrade.CurrentValue;
            }
        }
    }

    // Modificadores Ammos
    private void AmmoDamageModifier(UpgradeInstance upgrade)
    {
        DataState.ammos.TryGetValue(upgrade.TargetId, out var ammo);

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            CalculateUpgradeValue(upgrade);

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                ammo.ActualDamage *= upgrade.CurrentValue;
            }

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
            {
                ammo.BaseDamage *= upgrade.CurrentValue;
            }
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            CalculateUpgradeValue(upgrade);

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                ammo.ActualDamage += upgrade.CurrentValue;
            }

            if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
            {
                ammo.BaseDamage += upgrade.CurrentValue;
            }
        }
    }

    // Modificadores de Upgrades
    private void ExperiencePerKillRateModifier(UpgradeInstance upgrade)
    {
        CalculateUpgradeValue(upgrade);

        foreach (var upgradeTarget in GameState.DataState.upgrades.Values)
        {
            if (upgradeTarget.Id == upgrade.TargetId)
            {
                upgradeTarget.BaseUpgradeValue += upgrade.CurrentValue;
            }
        }
    }
    private void ShipRepairRateModifier(UpgradeInstance upgrade)
    {
        CalculateUpgradeValue(upgrade);

        foreach (var upgradeTarget in GameState.DataState.upgrades.Values)
        {
            if (upgradeTarget.Id == upgrade.TargetId)
            {
                upgradeTarget.BaseUpgradeValue += upgrade.CurrentValue;
            }
        }
    }

    // Modificadores Meta
    private void GameSpeedModifier(UpgradeInstance upgrade)
    {
        CalculateUpgradeValue(upgrade);

        GameState.MaxGameSpeed += (int)upgrade.CurrentValue;
    }
    private void ExperienceKillModifier(UpgradeInstance upgrade)
    {
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            CalculateUpgradeValue(upgrade);

            GameState.ExpeditionState.ActualExperienceKillBonus *= upgrade.CurrentValue;
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            CalculateUpgradeValue(upgrade);

            GameState.ExpeditionState.ActualExperienceKillBonus += upgrade.CurrentValue;
        }
    }
    private void ExperienceIncomeModifier(UpgradeInstance upgrade)
    {
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            CalculateUpgradeValue(upgrade);

            GameState.ExpeditionState.ActualNightReward *= upgrade.CurrentValue;
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            CalculateUpgradeValue(upgrade);

            GameState.ExpeditionState.ActualNightReward += upgrade.CurrentValue;
        }
    }
    private void ClickTargetModifier(UpgradeInstance upgrade)
    {
        switch (upgrade.TargetId)
        {
            case "Fish":
                foreach (var ingredient in GameState.DataState.ingredients.Values)
                {
                    if (ingredient.UnlockId == "Fish")
                    {
                        if (ingredient.Rarity != GameHelper.ItemRarity.Common)
                            continue;

                        ingredient.UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
                    }
                }
                break;
        }
    }
    private void ClickMaxModifier(UpgradeInstance upgrade)
    {
        CalculateUpgradeValue(upgrade);

        GameState.ExpeditionState.BaseMaxMarkedEnemies += (int)upgrade.CurrentValue;
    }
    private void ClickRarityModifier(UpgradeInstance upgrade)
    {
        switch (upgrade.TargetId)
        {
            case "Fish":
                foreach (var ingredient in GameState.DataState.ingredients.Values)
                {
                    if (ingredient.UnlockId == "Fish")
                    {
                        if (upgrade.ActualBuy == 1)
                        {
                            if (ingredient.Rarity != GameHelper.ItemRarity.Uncommon)
                                continue;

                            ingredient.UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
                        }
                        if (upgrade.ActualBuy == 2)
                        {
                            if (ingredient.Rarity != GameHelper.ItemRarity.Rare)
                                continue;

                            ingredient.UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
                        }
                        if (upgrade.ActualBuy == 3)
                        {
                            if (ingredient.Rarity != GameHelper.ItemRarity.Legendary)
                                continue;

                            ingredient.UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
                        }
                    }
                }
                break;
        }
    }

    // Modificadores Missions
    private void MissionsMaxModifier(UpgradeInstance upgrade)
    {
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            CalculateUpgradeValue(upgrade);

            GameState.MissionsState.MaxOnGoingMissions += (int)upgrade.CurrentValue;
        }

        GameEvents.MissionSlotAtualize?.Invoke();
    }
    private void MissionsRewardModifier(UpgradeInstance upgrade)
    {
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            CalculateUpgradeValue(upgrade);

            GameState.MissionsState.RewardBonus *= upgrade.CurrentValue;
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            CalculateUpgradeValue(upgrade);

            GameState.MissionsState.RewardBonus += upgrade.CurrentValue;
        }
    }
    private void MissionsOptionsModifier(UpgradeInstance upgrade)
    {
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            CalculateUpgradeValue(upgrade);

            GameState.MissionsState.MaxMissionsOptions += (int)upgrade.CurrentValue;
        }
    }
    private void MissionsCancelModifier(UpgradeInstance upgrade)
    {
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            CalculateUpgradeValue(upgrade);

            GameState.MissionsState.MaxCancelableMissions += (int)upgrade.CurrentValue;
        }
    }

    // Events
    void OnEnable()
    {
        GameEvents.OnUpgradeBuy += AddUpgrade;
        ExpeditionEvents.OnExpeditionEnd += ResetExpeditionUpgrades;
    }

    void OnDisable()
    {
        GameEvents.OnUpgradeBuy -= AddUpgrade;
        ExpeditionEvents.OnExpeditionEnd -= ResetExpeditionUpgrades;
    }
}


