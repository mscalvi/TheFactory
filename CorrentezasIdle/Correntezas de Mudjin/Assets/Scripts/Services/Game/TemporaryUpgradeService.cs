using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TemporaryUpgradeService : MonoBehaviour
{
    private ExpeditionState ExpeditionState;
    private DataState DataState;
    private ShipState ShipState;

    public void Initialize(ExpeditionState expeditionState, DataState dataState, ShipState shipState)
    {
        ExpeditionState = expeditionState;

        DataState = dataState;

        ShipState = shipState;
    }

    public void AddUpgrade(UpgradeInstance upgrade)
    {
        if (upgrade.Scope != UpgradeHelper.UpgradeScope.Expedition)
        {
            return;
        }

        Debug.Log($"Adicionando {upgrade.Id}");

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
        ShipState.Ship.MaxLife = ShipState.Ship.BaseLife;
        ShipState.Ship.MaxArmor = ShipState.Ship.BaseArmor;

        foreach (var weapon in DataState.weapons)
        {
            weapon.Value.ActualDamage = weapon.Value.BaseDamage;
            weapon.Value.ActualRange = weapon.Value.BaseRange;
            weapon.Value.ActualAttackSpeed = weapon.Value.BaseAttackSpeed;
            weapon.Value.ActualPrecision = weapon.Value.BasePrecision;
            weapon.Value.ActualCriticalDamage = weapon.Value.BaseCriticalDamage;
        }
    }

    private void ApplyUpgrade(UpgradeInstance upgrade)
    {
        upgrade.ActualValue = upgrade.StartValue;

        switch (upgrade.TargetType) 
        { 
            case UpgradeHelper.TargetType.Ship:
                switch (upgrade.EffectType)
                {
                    case UpgradeHelper.EffectType.ShipAbsoluteArmor:
                        ShipAbsoluteArmorModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.ShipMaxLife:
                        ShipMaxLifeModifier(upgrade);
                        break;
                }
                break;
            case UpgradeHelper.TargetType.Weapon:
                switch (upgrade.EffectType)
                {
                    case UpgradeHelper.EffectType.WeaponDamage:
                        WeaponDamageModifier(upgrade);
                        break;
                }
                break;
        }
    }

    // Modificadores Ship
    private void ShipAbsoluteArmorModifier(UpgradeInstance upgrade)
    {
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.ActualValue * upgrade.UpgradeValue;
            }

            ShipState.Ship.MaxArmor *= upgrade.ActualValue;

            ShipState.Ship.CurrentArmor = ShipState.Ship.MaxArmor;
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            ShipState.Ship.MaxArmor += upgrade.ActualValue;

            ShipState.Ship.CurrentArmor = ShipState.Ship.MaxArmor;
        }
    }

    private void ShipMaxLifeModifier(UpgradeInstance upgrade)
    {
        double currentLife = ShipState.Ship.CurrentLife;

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.ActualValue * upgrade.UpgradeValue;
            }

            ShipState.Ship.MaxLife *= upgrade.ActualValue;

            ShipState.Ship.CurrentLife = (int)(currentLife + (ShipState.Ship.MaxLife * upgrade.UpgradeValue));

            if (ShipState.Ship.CurrentLife > ShipState.Ship.MaxLife)
            {
                ShipState.Ship.CurrentLife = (int)ShipState.Ship.MaxLife;
            }
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            ShipState.Ship.MaxLife += upgrade.ActualValue;

            ShipState.Ship.CurrentLife = (int)(ShipState.Ship.MaxLife + upgrade.UpgradeValue);

            if (ShipState.Ship.CurrentLife > ShipState.Ship.MaxLife)
            {
                ShipState.Ship.CurrentLife = (int)ShipState.Ship.MaxLife;
            }
        }
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

    // Events
    void OnEnable()
    {
        GameEvents.OnUpgradeBuy += AddUpgrade;
        ExpeditionEvents.OnExpeditionStart += ResetExpeditionUpgrades;
    }

    void OnDisable()
    {
        GameEvents.OnUpgradeBuy -= AddUpgrade;
        ExpeditionEvents.OnExpeditionStart -= ResetExpeditionUpgrades;
    }
}


