using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeTemporaryService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    public void Initialize(GameState game)
    {
        GameState = game;

        DataState = GameState.DataState;
    }

    public void AddUpgrade(UpgradeInstance upgrade)
    {
        if (upgrade.Scope != UpgradeHelper.UpgradeScope.Expedition)
        {
            return;
        }

        var upgrades = DataState.upgrades;

        if (!upgrades.TryGetValue(upgrade.Id, out var upgradeInstance))
        {
            upgrades[upgrade.Id] = upgrade;
        }

        Recalculate();
    }

    void ResetExpeditionUpgrades()
    {
        var upgrades = DataState.upgrades;

        foreach(var upgrade in upgrades)
        {
            if(upgrade.Value.Scope == UpgradeHelper.UpgradeScope.Expedition)
            {
                upgrade.Value.ActualBuy = 0;
                upgrade.Value.ActualCost = upgrade.Value.Cost;
                upgrade.Value.ActualValue = upgrade.Value.StartValue;
            }
        }

        Recalculate();
    }

    private void Recalculate()
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

        ExpeditionEvents.OnShipAtributeChange?.Invoke();
    }

    private void ApplyBaseStats()
    {
        var Ship = GameState.ExpeditionState.Ship;

        Ship.MaxLife = Ship.BaseLife;
        Ship.MaxArmor = Ship.BaseArmor;
        Ship.MaxResistence = Ship.BaseResistence;
        Ship.MaxRepairPerTripulation = Ship.BaseRepairPerTripulation;

        foreach (var weapon in DataState.weapons)
        {
            weapon.Value.ActualDamage = weapon.Value.BaseDamage;
            weapon.Value.ActualRange = weapon.Value.BaseRange;
            weapon.Value.ActualAttackSpeed = weapon.Value.BaseAttackSpeed;
            weapon.Value.ActualPrecision = weapon.Value.BasePrecision;
            weapon.Value.ActualCriticalDamage = weapon.Value.BaseCriticalDamage;

        }

        GameState.ExpeditionState.ActualExperienceKillBonus = GameState.ExpeditionState.BaseExperienceKillBonus;
    }

    private void ApplyUpgrade(UpgradeInstance upgrade)
    {
        upgrade.ActualValue = upgrade.StartValue;

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
                }
                break;
            case UpgradeHelper.TargetType.Meta:
                switch (upgrade.EffectType)
                {
                    case UpgradeHelper.EffectType.ExperienceGain:
                        ExperienceKillModifier(upgrade);
                        break;
                }
                break;

        }
    }

    // Modificadores Ship
    private void ShipMaxLifeModifier(UpgradeInstance upgrade)
    {
        var ship = GameState.ExpeditionState.Ship;
        double healthPercent = ship.ActualLife / ship.MaxLife;

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.ActualValue * upgrade.UpgradeValue;
            }

            ship.MaxLife *= upgrade.ActualValue;
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            ship.MaxLife += upgrade.ActualValue;
        }

        ship.ActualLife = ship.MaxLife * healthPercent;
        if (ship.ActualLife > ship.MaxLife)
        {
            ship.ActualLife = ship.MaxLife;
        }
    }
    private void ShipArmorModifier(UpgradeInstance upgrade)
    {
        var ship = GameState.ExpeditionState.Ship;

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.ActualValue * upgrade.UpgradeValue;
            }

            ship.MaxArmor *= upgrade.ActualValue;
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            ship.MaxArmor += upgrade.ActualValue;
        }

        ship.ActualArmor = ship.MaxArmor;
    }
    private void ShipResistenceModifier(UpgradeInstance upgrade)
    {
        var ship = GameState.ExpeditionState.Ship;

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.ActualValue * upgrade.UpgradeValue;
            }

            ship.MaxResistence *= upgrade.ActualValue;
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            ship.MaxResistence += upgrade.ActualValue;
        }

        ship.ActualResistence = ship.MaxResistence;
    }
    private void ShipRepairModifier(UpgradeInstance upgrade)
    {
        var ship = GameState.ExpeditionState.Ship;

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.ActualValue * upgrade.UpgradeValue;
            }

            ship.MaxRepairPerTripulation *= upgrade.ActualValue;
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            ship.MaxRepairPerTripulation += upgrade.ActualValue;
        }

        ship.ActualRepairPerTripulation = ship.MaxRepairPerTripulation;
    }

    // Modificadores Weapons
    private void WeaponDamageModifier(UpgradeInstance upgrade)
    {
        DataState.weapons.TryGetValue(upgrade.TargetId, out var weapon);

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.ActualValue * upgrade.UpgradeValue;
            }

            weapon.ActualDamage *= upgrade.ActualValue;
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            weapon.ActualDamage += upgrade.ActualValue;
        }
    }
    private void WeaponAtkSpeedModifier(UpgradeInstance upgrade)
    {
        DataState.weapons.TryGetValue(upgrade.TargetId, out var weapon);

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.ActualValue * upgrade.UpgradeValue;
            }

            weapon.ActualAttackSpeed *= upgrade.ActualValue;
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            weapon.ActualAttackSpeed += upgrade.ActualValue;
        }
    }

    // Modificadores Meta
    private void ExperienceKillModifier(UpgradeInstance upgrade)
    {
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.ActualValue * upgrade.UpgradeValue;
            }

            GameState.ExpeditionState.ActualExperienceKillBonus *= upgrade.ActualValue;
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            GameState.ExpeditionState.ActualExperienceKillBonus += upgrade.ActualValue;
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


