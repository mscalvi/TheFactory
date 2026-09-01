using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierService : MonoBehaviour
{
    private GameState GameState;

    class Modifier
    {
        public double AdCompMod = 0;
        public double AdExpeMod = 0;
        public double MtCompMod = 1;
        public double MtExpeMod = 1;
    }

    public void Initialize(GameState game)
    {
        GameState = game;
    }

    public void ApplyUpgrade(UpgradeInstance upgrade)
    {
        CreateModifier(upgrade);
     
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
                    case UpgradeHelper.EffectType.ShipSpikes:
                        ShipSpikesModifier(upgrade);
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
                    case UpgradeHelper.EffectType.WeaponCritical:
                        WeaponCriticalModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.WeaponPrecision:
                        WeaponPrecisionModifier(upgrade);
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
                    case UpgradeHelper.EffectType.AmmoAmmount:
                        AmmoAmmountModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.AmmoRecharge:
                        AmmoRechargeModifier(upgrade);
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
            case UpgradeHelper.TargetType.Product:
                switch (upgrade.EffectType)
                {
                    case UpgradeHelper.EffectType.ProductGeneration:
                        ProductGenerationModifier(upgrade);
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
                    case UpgradeHelper.EffectType.GameSpeed:
                        GameSpeedModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.ExperiencePerKill:
                        ExperienceKillModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.ExperienceIncome:
                        ExperienceIncomeModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.MarcosIncome:
                        MarcosIncomeModifier(upgrade);
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
                    case UpgradeHelper.EffectType.ClickChance:
                        ClickChanceModifier(upgrade);
                        break;
                    default:
                        Debug.Log($"UPGRADE NÃO IMPLEMENTADO! {upgrade.NamePT}");
                        break;
                }
                break;
        }

        if (upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
        {
            ExpeditionEvents.OnShipAtributeChange?.Invoke();
        }
    }

    private void CreateModifier(UpgradeInstance upgrade)
    {
        var mod = new ModifierModel();

        mod.Opp = upgrade.UpgradeType;
        mod.Type = upgrade.EffectType;
        mod.Scope = upgrade.Scope;
        mod.Value = upgrade.ActualUpgradeValue;

        GameState.UpgradesState.Modifiers.Add(mod);
    }

    private Modifier ApplyModifiers(UpgradeInstance upgrade)
    {
        var mod = new Modifier();

        foreach(var modifier in GameState.UpgradesState.Modifiers)
        {
            if (modifier.Type != upgrade.EffectType)
                continue;

            if (modifier.Opp == UpgradeHelper.UpgradeType.Additive)
            {
                if (modifier.Scope == UpgradeHelper.UpgradeScope.Expedition)
                {
                    mod.AdExpeMod += modifier.Value;
                }
                if (modifier.Scope == UpgradeHelper.UpgradeScope.Company)
                {
                    mod.AdCompMod += modifier.Value;
                }
            }

            if (modifier.Opp == UpgradeHelper.UpgradeType.Multiplicative)
            {
                if (modifier.Scope == UpgradeHelper.UpgradeScope.Expedition)
                {
                    mod.MtExpeMod *= modifier.Value;
                }
                if (modifier.Scope == UpgradeHelper.UpgradeScope.Company)
                {
                    mod.MtCompMod *= modifier.Value;
                }
            }
        }

        return mod;
    }


    // Modificadores Ship
    private void ShipMaxLifeModifier(UpgradeInstance upgrade)
    {
        var ship = GameState.ExpeditionState.Ship;
        double healthPercent = ship.CurrentLife / ship.ActualLife;

        var Modifier = ApplyModifiers(upgrade);

        ship.BaseLife = (int)((ship.StartLife + Modifier.AdCompMod) * Modifier.MtCompMod);
        ship.ActualLife = (int)((ship.BaseLife + Modifier.AdExpeMod) * Modifier.MtExpeMod);

        ship.CurrentLife = (int)(ship.ActualLife * healthPercent);

        if (ship.CurrentLife > ship.ActualLife)
        {
            ship.CurrentLife = (int)ship.ActualLife;
        }
    }
    private void ShipArmorModifier(UpgradeInstance upgrade)
    {
        var ship = GameState.ExpeditionState.Ship;

        var Modifier = ApplyModifiers(upgrade);

        ship.BaseArmor = (ship.StartArmor + Modifier.AdCompMod) * Modifier.MtCompMod;
        ship.ActualArmor = (ship.BaseArmor + Modifier.AdExpeMod) * Modifier.MtExpeMod;
    }
    private void ShipResistenceModifier(UpgradeInstance upgrade)
    {
        var ship = GameState.ExpeditionState.Ship;

        var Modifier = ApplyModifiers(upgrade);

        ship.BaseResistence = (ship.StartResistence + Modifier.AdCompMod) * Modifier.MtCompMod;
        ship.ActualResistence = (ship.BaseResistence + Modifier.AdExpeMod) * Modifier.MtExpeMod;
    }
    private void ShipRepairModifier(UpgradeInstance upgrade)
    {
        var ship = GameState.ExpeditionState.Ship;

        var Modifier = ApplyModifiers(upgrade);

        ship.BaseRepair = (ship.StartRepair + Modifier.AdCompMod) * Modifier.MtCompMod;
        ship.ActualRepair = (ship.BaseRepair + Modifier.AdExpeMod) * Modifier.MtExpeMod;
    }
    private void ShipSpikesModifier(UpgradeInstance upgrade)
    {
        var ship = GameState.ExpeditionState.Ship;

        var Modifier = ApplyModifiers(upgrade);

        if (ship.StartSpikes == 0)
        {
            ship.StartSpikes = 1;
        }

        ship.BaseSpikes = (ship.StartSpikes + Modifier.AdCompMod) * Modifier.MtCompMod;
        ship.ActualSpikes = (ship.BaseSpikes + Modifier.AdExpeMod) * Modifier.MtExpeMod;
    }

    // Modificadores Weapons
    private void WeaponDamageModifier(UpgradeInstance upgrade)
    {
        GameState.DataState.weapons.TryGetValue(upgrade.TargetId, out var weapon);

        var Modifier = ApplyModifiers(upgrade);

        weapon.BaseDamage = (weapon.StartDamage + Modifier.AdCompMod) * Modifier.MtCompMod;
        weapon.ActualDamage = (weapon.BaseDamage + Modifier.AdExpeMod) * Modifier.MtExpeMod;
    }
    private void WeaponAtkSpeedModifier(UpgradeInstance upgrade)
    {
        GameState.DataState.weapons.TryGetValue(upgrade.TargetId, out var weapon);

        var Modifier = ApplyModifiers(upgrade);

        weapon.BaseAttackSpeed = (weapon.StartAttackSpeed + Modifier.AdCompMod) * Modifier.MtCompMod;
        weapon.ActualAttackSpeed = (weapon.BaseAttackSpeed + Modifier.AdExpeMod) * Modifier.MtExpeMod;
    }
    private void WeaponRangeModifier(UpgradeInstance upgrade)
    {
        GameState.DataState.weapons.TryGetValue(upgrade.TargetId, out var weapon);

        var Modifier = ApplyModifiers(upgrade);

        weapon.BaseRange = (weapon.StartRange + Modifier.AdCompMod) * Modifier.MtCompMod;
        weapon.ActualRange = (weapon.BaseRange + Modifier.AdExpeMod) * Modifier.MtExpeMod;
    }
    private void WeaponCriticalModifier(UpgradeInstance upgrade)
    {
        GameState.DataState.weapons.TryGetValue(upgrade.TargetId, out var weapon);

        var Modifier = ApplyModifiers(upgrade);

        weapon.BaseCriticalDamage = (weapon.StartCriticalDamage + Modifier.AdCompMod) * Modifier.MtCompMod;
        weapon.ActualCriticalDamage = (weapon.BaseCriticalDamage + Modifier.AdExpeMod) * Modifier.MtExpeMod;
    }
    private void WeaponPrecisionModifier(UpgradeInstance upgrade)
    {
        GameState.DataState.weapons.TryGetValue(upgrade.TargetId, out var weapon);

        var Modifier = ApplyModifiers(upgrade);

        weapon.BasePrecision = (weapon.StartPrecision + Modifier.AdCompMod) * Modifier.MtCompMod;
        weapon.ActualPrecision = (weapon.BasePrecision + Modifier.AdExpeMod) * Modifier.MtExpeMod;
    }

    // Modificadores Ammos
    private void AmmoDamageModifier(UpgradeInstance upgrade)
    {
        GameState.DataState.ammos.TryGetValue(upgrade.TargetId, out var ammo);

        var Modifier = ApplyModifiers(upgrade);

        ammo.BaseDamage = (ammo.StartDamage + Modifier.AdCompMod) * Modifier.MtCompMod;
        ammo.ActualDamage = (ammo.BaseDamage + Modifier.AdExpeMod) * Modifier.MtExpeMod;
    }
    private void AmmoAmmountModifier(UpgradeInstance upgrade)
    {
        GameState.DataState.ammos.TryGetValue(upgrade.TargetId, out var ammo);

        var Modifier = ApplyModifiers(upgrade);

        ammo.BaseAmmount = (int)((ammo.StartAmmount + Modifier.AdCompMod) * Modifier.MtCompMod);
        ammo.ActualAmmount = (int)((ammo.BaseAmmount + Modifier.AdExpeMod) * Modifier.MtExpeMod);
    }
    private void AmmoRechargeModifier(UpgradeInstance upgrade)
    {
        GameState.DataState.ammos.TryGetValue(upgrade.TargetId, out var ammo);

        var Modifier = ApplyModifiers(upgrade);

        ammo.BaseRecharge = (int)((ammo.StartRecharge + Modifier.AdCompMod) * Modifier.MtCompMod);
        ammo.ActualRecharge = (int)((ammo.BaseRecharge + Modifier.AdExpeMod) * Modifier.MtExpeMod);
    }

    // Modificadores de Products
    private void ProductGenerationModifier(UpgradeInstance upgrade)
    {
        GameState.DataState.products.TryGetValue(upgrade.TargetId, out var product);

        var Modifier = ApplyModifiers(upgrade);

        product.BaseIncomeAmmount = (int)((product.StartIncomeAmmount + Modifier.AdCompMod) * Modifier.MtCompMod);
        product.ActualIncomeAmmount = (int)((product.BaseIncomeAmmount + Modifier.AdExpeMod) * Modifier.MtExpeMod);
    }

    // Modificadores de Upgrades
    private void ExperiencePerKillRateModifier(UpgradeInstance upgrade)
    {
        var Modifier = ApplyModifiers(upgrade);

        upgrade.ActualUpgradeValue = (upgrade.StartUpgradeValue + Modifier.AdCompMod + Modifier.AdExpeMod) * Modifier.MtCompMod * Modifier.MtExpeMod;
    }
    private void ShipRepairRateModifier(UpgradeInstance upgrade)
    {
        var Modifier = ApplyModifiers(upgrade);

        upgrade.ActualUpgradeValue = (upgrade.StartUpgradeValue + Modifier.AdCompMod + Modifier.AdExpeMod) * Modifier.MtCompMod * Modifier.MtExpeMod;
    }

    // Modificadores Meta
    private void GameSpeedModifier(UpgradeInstance upgrade)
    {
        var Modifier = ApplyModifiers(upgrade);

        GameState.MaxGameSpeed = (float)((1 + Modifier.AdCompMod + Modifier.AdExpeMod) * Modifier.MtCompMod * Modifier.MtExpeMod);
    }
    private void ExperienceKillModifier(UpgradeInstance upgrade)
    {
        var Modifier = ApplyModifiers(upgrade);

        GameState.ExpeditionState.BaseExperienceKillBonus = (GameState.ExpeditionState.StartExperienceKillBonus + Modifier.AdCompMod) * Modifier.MtCompMod;
        GameState.ExpeditionState.ActualExperienceKillBonus = (GameState.ExpeditionState.BaseExperienceKillBonus + Modifier.AdExpeMod) * Modifier.MtExpeMod;
    }
    private void ExperienceIncomeModifier(UpgradeInstance upgrade)
    {
        var Modifier = ApplyModifiers(upgrade);

        GameState.ExpeditionState.ActualNightReward = (GameState.ExpeditionState.StartNightReward + Modifier.AdCompMod) * Modifier.MtCompMod;
        GameState.ExpeditionState.ActualNightReward = (GameState.ExpeditionState.StartNightReward + Modifier.AdExpeMod) * Modifier.MtExpeMod;
    }
    private void MarcosIncomeModifier(UpgradeInstance upgrade)
    {
        var Modifier = ApplyModifiers(upgrade);

        GameState.ExpeditionState.ActualDayReward = (GameState.ExpeditionState.StartDayReward + Modifier.AdCompMod) * Modifier.MtCompMod;
        GameState.ExpeditionState.ActualDayReward = (GameState.ExpeditionState.StartDayReward + Modifier.AdExpeMod) * Modifier.MtExpeMod;
    }
    private void ClickTargetModifier(UpgradeInstance upgrade)
    {
        var Modifier = ApplyModifiers(upgrade);

        switch (upgrade.TargetId)
        {
            case "Fish":
                Debug.Log("Isso ainda não faz nada");
                break;
        }

    }
    private void ClickMaxModifier(UpgradeInstance upgrade)
    {
        var Modifier = ApplyModifiers(upgrade);

        GameState.ExpeditionState.ActualMaxMarkedEnemies = (int)((GameState.ExpeditionState.StartMaxMarkedEnemies + Modifier.AdCompMod + Modifier.AdExpeMod) * Modifier.MtCompMod * Modifier.MtExpeMod);
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
    private void ClickChanceModifier(UpgradeInstance upgrade)
    {
        var weight = GameState.ExpeditionState.ActualIngredientRarityWeights;

        var Modifier = ApplyModifiers(upgrade);

        weight[GameHelper.ItemRarity.Common] = weight[GameHelper.ItemRarity.Common] + (float)Modifier.AdCompMod;
        weight[GameHelper.ItemRarity.Uncommon] = weight[GameHelper.ItemRarity.Uncommon] + (float)Modifier.AdCompMod * 0.250f;
        weight[GameHelper.ItemRarity.Rare] = weight[GameHelper.ItemRarity.Rare] + (float)Modifier.AdCompMod * 0.12f;          
        weight[GameHelper.ItemRarity.Legendary] = weight[GameHelper.ItemRarity.Legendary] + (float)Modifier.AdCompMod * 0.011f; 
    }

    // Modificadores Missions
    private void MissionsMaxModifier(UpgradeInstance upgrade)
    {
        var Modifier = ApplyModifiers(upgrade);

        GameState.MissionsState.MaxOnGoingMissions = (int)((0 + Modifier.AdCompMod + Modifier.AdExpeMod) * Modifier.MtCompMod * Modifier.MtExpeMod);

        GameEvents.MissionSlotAtualize?.Invoke();
    }
    private void MissionsRewardModifier(UpgradeInstance upgrade)
    {
        var Modifier = ApplyModifiers(upgrade);

        GameState.MissionsState.MaxRewardItens = (int)((1 + Modifier.AdCompMod + Modifier.AdExpeMod) * Modifier.MtCompMod * Modifier.MtExpeMod);
    }
    private void MissionsOptionsModifier(UpgradeInstance upgrade)
    {
        var Modifier = ApplyModifiers(upgrade);

        GameState.MissionsState.MaxMissionsOptions = (int)((1 + Modifier.AdCompMod + Modifier.AdExpeMod) * Modifier.MtCompMod * Modifier.MtExpeMod);
    }
    private void MissionsCancelModifier(UpgradeInstance upgrade)
    {
        var Modifier = ApplyModifiers(upgrade);

        GameState.MissionsState.MaxCancelableMissions = (int)((0 + Modifier.AdCompMod + Modifier.AdExpeMod) * Modifier.MtCompMod * Modifier.MtExpeMod);
    }
}
