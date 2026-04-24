using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentUpgradeService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;
    private ShipState ShipState;
    private UnlockService UnlockService;

    public void Initialize (GameState gameState, DataState dataState, ShipState shipState, UnlockService unlock)
    {
        GameState = gameState;

        DataState = dataState;

        ShipState = shipState;

        UnlockService = unlock;
    }

    public void AddUpgrade(UpgradeInstance upgrade)
    {
        if(upgrade.Scope != UpgradeHelper.UpgradeScope.Company)
        {
            return;
        }

        Debug.Log($"Game PermanentUpgradeService - Adicionando Upgrade: {upgrade.Id}");

        var upgrades = GameState.CompanyState.CompanyUpgrades;

        if (!upgrades.TryGetValue(upgrade.Id, out var instance))
        {
            upgrades[upgrade.Id] = upgrade;
            instance = upgrade;
        }

        Debug.Log($"Game PermanentUpgradeService - Localizando Upgrade: {instance.Id}");

        if (instance.ActualBuy >= instance.MaxBuy && instance.MaxBuy > 0)
        {
            instance.UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
        }

        if(instance.EffectType != UpgradeHelper.EffectType.Unlock)
        {
            Debug.Log($"Game PermanentUpgradeService - Recualculando");
            Recalculate();
        } else
        {
            Debug.Log($"Game PermanentUpgradeService - Unlocking");
            UnlockService.UnlockUpgrade(instance);
        }
    }

    private void Recalculate()
    {
        ApplyBaseStats();

        foreach (var upgrade in GameState.CompanyState.CompanyUpgrades)
        {
            if (upgrade.Value.UpgradeType == UpgradeHelper.UpgradeType.Additive)
            {
                ApplyUpgrade(upgrade.Value);
            }
        }

        foreach (var upgrade in GameState.CompanyState.CompanyUpgrades)
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

        GameState.MissionsState.MaxCancelableMissions = 0;
        GameState.MissionsState.MaxOnGoingMissions = 0;
        GameState.MissionsState.MaxRewardItens = 1;
        GameState.MissionsState.RewardBonus = 1;
        GameState.MissionsState.MaxMissionsOptions = 1;
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
                        ShipLifeModifier(upgrade);
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

    // Modificadores Missions
    private void MissionsMaxModifier(UpgradeInstance upgrade)
    {
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            GameState.MissionsState.MaxOnGoingMissions += (int)upgrade.ActualValue;
        }
    }

    private void MissionsRewardModifier(UpgradeInstance upgrade)
    {
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.ActualValue * upgrade.UpgradeValue;
            }

            GameState.MissionsState.RewardBonus *= (int)upgrade.ActualValue;
        }

        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            GameState.MissionsState.MaxRewardItens += (int)upgrade.ActualValue;
        }
    }

    private void MissionsOptionsModifier(UpgradeInstance upgrade)
    {
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            GameState.MissionsState.MaxMissionsOptions += (int)upgrade.ActualValue;
        }
    }

    private void MissionsCancelModifier(UpgradeInstance upgrade)
    {
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            for (int i = 1; i <= upgrade.ActualBuy; i++)
            {
                upgrade.ActualValue += upgrade.UpgradeValue;
            }

            GameState.MissionsState.MaxCancelableMissions += (int)upgrade.ActualValue;
        }
    }

    // Events
    void OnEnable()
    {
        GameEvents.OnUpgradeBuy += AddUpgrade;
    }

    void OnDisable()
    {
        GameEvents.OnUpgradeBuy -= AddUpgrade;
    }
}
