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

        var upgrades = ExpeditionState.ExpeditionUpgrades;

        if (!upgrades.TryGetValue(upgrade.Id, out var upgradeInstance))
        {
            upgrades[upgrade.Id] = upgrade;
        }

        Recalculate();
    }

    void ResetExpeditionUpgrades()
    {
        var upgrades = ExpeditionState.ExpeditionUpgrades;

        foreach (var upgrade in upgrades)
        {
            upgrade.Value.ActualBuy = 0;
            upgrade.Value.ActualCost = upgrade.Value.Cost;
            upgrade.Value.ActualValue = upgrade.Value.StartValue;
        }

        ExpeditionState.ExpeditionUpgrades.Clear();

        Recalculate();
    }

    private void Recalculate()
    {
        ApplyBaseStats();

        foreach (var upgrade in ExpeditionState.ExpeditionUpgrades)
        {
            if (upgrade.Value.UpgradeType == UpgradeHelper.UpgradeType.Additive)
            {
                ApplyUpgrade(upgrade.Value);
            }
        }

        foreach (var upgrade in ExpeditionState.ExpeditionUpgrades)
        {
            if (upgrade.Value.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
            {
                ApplyUpgrade(upgrade.Value);
            }
        }

        ExpeditionEvents.OnShipAtributeChange?.Invoke();
    }

    private void ApplyBaseStats()
    {
        ShipState.Ship.MaxLife = ShipState.Ship.BaseLife;
        ShipState.Ship.MaxArmor = ShipState.Ship.BaseArmor;
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

            ShipState.Ship.CurrentLife = currentLife + (currentLife * upgrade.UpgradeValue);
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            ShipState.Ship.MaxLife += upgrade.ActualValue;

            ShipState.Ship.CurrentLife = currentLife + upgrade.UpgradeValue;
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


