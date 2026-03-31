using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentUpgradeService : MonoBehaviour
{
    private ShipState ShipState;

    private Dictionary<string, UpgradeInstance> ActiveUpgrades;

    public void Initialize (ShipState shipState)
    {
        ShipState = shipState;

        ActiveUpgrades = new Dictionary<string, UpgradeInstance>();
    }

    public void AddUpgrade(UpgradeInstance upgrade)
    {
        if (!ActiveUpgrades.TryGetValue(upgrade.Id, out var upgradeInstance))
        {
            ActiveUpgrades[upgrade.Id] = upgrade;
        }

        Recalculate();
    }

    private void Recalculate()
    {
        ApplyBaseStats();

        foreach (var upgrade in ActiveUpgrades)
        {
            if (upgrade.Value.UpgradeType == UpgradeHelper.UpgradeType.Additive)
            {
                ApplyUpgrade(upgrade.Value);
            }
        }

        foreach (var upgrade in ActiveUpgrades)
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

        ShipEvents.AfterUpgradeBuy?.Invoke(upgrade);
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
        BuildingEvents.OnUpgradeBuy += UpgradeBought;
    }

    void OnDisable()
    {
        BuildingEvents.OnUpgradeBuy -= UpgradeBought;
    }

    void UpgradeBought(UpgradeInstance upgrade)
    {
        AddUpgrade(upgrade);
    }
}
