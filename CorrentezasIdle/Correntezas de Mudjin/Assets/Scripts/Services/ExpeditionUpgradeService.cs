using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionUpgradeService : MonoBehaviour
{
    private ExpeditionState Expedition;
    private DataState DataState;
    private ShipState ShipState;

    private Dictionary<string, UpgradeInstance> ActiveUpgrades;

    public void Initialize(ExpeditionState expeditionState, DataState dataState, ShipState shipState)
    {
        Expedition = expeditionState;

        DataState = dataState;

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

    private void ShipLifeModifier(UpgradeInstance upgrade)
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
        ShipEvents.OnUpgradeBuy += UpgradeBought;
        RunEvents.OnExpeditionEnd += ResetExpeditionUpgrades;
    }

    void OnDisable()
    {
        ShipEvents.OnUpgradeBuy -= UpgradeBought;
        RunEvents.OnExpeditionEnd -= ResetExpeditionUpgrades;
    }

    void ResetExpeditionUpgrades()
    {
        foreach(var upgrade in ActiveUpgrades)
        {
            upgrade.Value.ActualBuy = 0;
        }

        Recalculate();
        ActiveUpgrades.Clear();
    }

    void UpgradeBought(UpgradeInstance upgrade)
    {
        AddUpgrade(upgrade);
    }
}


