using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeService : MonoBehaviour
{
    private GameState GameState;
    private UnlockService UnlockService;
    private ModifierService ModifierService;

    public void Initialize(GameState game, UnlockService unlock, ModifierService modifier)
    {
        GameState = game;

        UnlockService = unlock;

        ModifierService = modifier;
    }

    public void AddUpgrade(UpgradeInstance upgrade)
    {
        if (upgrade.ActualBuy >= upgrade.MaxBuy && upgrade.MaxBuy > 0)
        {
            upgrade.UnlockStatus = UnlockHelper.UnlockStatus.Finished;
        }

        if (upgrade.EffectType != UpgradeHelper.EffectType.Unlock)
        {
            ModifierService.ApplyUpgrade(upgrade);
            CalculateCurrentValue(upgrade);
        }
        else
        {
            UnlockService.UnlockUpgrade(upgrade);
        }

        GameEvents.OnUpgradeBought?.Invoke(upgrade);
    }

    private void CalculateCurrentValue(UpgradeInstance upgrade)
    {
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
        {
            upgrade.CurrentValue = Mathf.Pow((float)upgrade.ActualUpgradeValue, (float)upgrade.ActualBuy);
        }
        if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
        {
            upgrade.CurrentValue = (float)upgrade.ActualUpgradeValue * (float)upgrade.ActualBuy;
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


