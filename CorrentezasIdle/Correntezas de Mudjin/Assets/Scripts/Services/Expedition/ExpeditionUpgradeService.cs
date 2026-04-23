using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionUpgradeService : MonoBehaviour
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
        var upgrades = ExpeditionState.ExpeditionUpgrades;

        if (!upgrades.TryGetValue(upgrade.Id, out var upgradeInstance))
        {
            upgrades[upgrade.Id] = upgrade;
        }

        upgrade.ActualBuy++;

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

        ShipEvents.OnAtributeChange?.Invoke();
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

        ShipEvents.AfterUpgradeBuy?.Invoke(upgrade);
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
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.ActualValue * upgrade.UpgradeValue;
            }

            ShipState.Ship.MaxLife *= upgrade.ActualValue;

            ShipState.Ship.CurrentLife = ShipState.Ship.MaxLife;
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            ShipState.Ship.MaxLife += upgrade.ActualValue;

            ShipState.Ship.CurrentLife = ShipState.Ship.MaxLife;
        }
    }

    // Events
    void OnEnable()
    {
        ShipEvents.OnUpgradeBuy += AddUpgrade;
        ExpeditionEvents.OnExpeditionEnd += ResetExpeditionUpgrades;
    }

    void OnDisable()
    {
        ShipEvents.OnUpgradeBuy -= AddUpgrade;
        ExpeditionEvents.OnExpeditionEnd -= ResetExpeditionUpgrades;
    }

    void ResetExpeditionUpgrades()
    {
        var upgrades = ExpeditionState.ExpeditionUpgrades;

        foreach (var upgrade in upgrades)
        {
            upgrade.Value.ActualBuy = 0;
            upgrade.Value.ActualCost = upgrade.Value.Cost;
        }

        Recalculate();

        ExpeditionState.ExpeditionUpgrades.Clear();
    }
}


