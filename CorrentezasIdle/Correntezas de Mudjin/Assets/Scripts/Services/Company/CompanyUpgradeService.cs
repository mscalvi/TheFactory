using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyUpgradeService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;
    private ShipState ShipState;

    public void Initialize (GameState gameState, DataState dataState, ShipState shipState)
    {
        GameState = gameState;

        DataState = dataState;

        ShipState = shipState;
    }

    public void AddUpgrade(UpgradeInstance upgrade)
    {
        var upgrades = GameState.CompanyUpgrades;

        if (!upgrades.TryGetValue(upgrade.Id, out var instance))
        {
            upgrades[upgrade.Id] = upgrade;
            instance = upgrade;
        }

        instance.ActualBuy++;

        Recalculate();
    }

    private void Recalculate()
    {
        ApplyBaseStats();

        foreach (var upgrade in GameState.CompanyUpgrades)
        {
            if (upgrade.Value.UpgradeType == UpgradeHelper.UpgradeType.Additive)
            {
                ApplyUpgrade(upgrade.Value);
            }
        }

        foreach (var upgrade in GameState.CompanyUpgrades)
        {
            if (upgrade.Value.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
            {
                ApplyUpgrade(upgrade.Value);
            }
        }
    }

    private void ApplyBaseStats()
    {
        ShipState.Ship.BaseLife = ShipState.Ship.StartLife;
        ShipState.Ship.BaseArmor = ShipState.Ship.StartArmor;
    }

    private void ApplyUpgrade(UpgradeInstance upgrade)
    {
        upgrade.ActualValue = upgrade.StartValue;

        switch (upgrade.TargetType)
        {
            case UpgradeHelper.TargetType.Ship:
                switch (upgrade.EffectType)
                {
                    case UpgradeHelper.EffectType.ShipArmor:
                        ShipArmorModifier(upgrade);
                        break;
                    case UpgradeHelper.EffectType.ShipLife:
                        ShipLifeModifier(upgrade);
                        break;
                }
                break;
        }

        CompanyEvents.AfterUpgradeBuy?.Invoke(upgrade);
    }

    // Modificadores Ship
    private void ShipArmorModifier(UpgradeInstance upgrade)
    {
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.ActualValue * upgrade.UpgradeValue;
            }

            ShipState.Ship.BaseArmor *= upgrade.ActualValue;
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            ShipState.Ship.BaseArmor += upgrade.ActualValue;
        }
    }

    private void ShipLifeModifier(UpgradeInstance upgrade)
    {
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.ActualValue * upgrade.UpgradeValue;
            }

            ShipState.Ship.BaseLife *= upgrade.ActualValue;
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            ShipState.Ship.BaseLife += upgrade.ActualValue;
        }
    }

    // Events
    void OnEnable()
    {
        CompanyEvents.OnUpgradeBuy += UpgradeBought;
    }

    void OnDisable()
    {
        CompanyEvents.OnUpgradeBuy -= UpgradeBought;
    }

    void UpgradeBought(UpgradeInstance upgrade)
    {
        AddUpgrade(upgrade);
    }
}
