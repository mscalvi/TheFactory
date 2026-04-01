using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyPurchaseService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    private Dictionary<string, UpgradeInstance> ActiveUpgrades;

    public void Initialize(DataState dataState)
    {
        DataState = dataState;

        ActiveUpgrades = new Dictionary<string, UpgradeInstance>();

        foreach (var upgrade in DataState.upgrades)
        {
            if (upgrade.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                ActiveUpgrades.Add(upgrade.Key, upgrade.Value);
            }
        }
    }

    public void BuyUpgrade(UpgradeInstance upgrade)
    {

    }

    public void CanBuyCheck(CurrencyHelper.CurrencyType type)
    {
        if (!GameState.CompanyCurrency.TryGetValue(type, out var Currency))
            return;

        foreach (var upgrade in ActiveUpgrades)
        {
            if (upgrade.Value.Currency == type)
            {
                if (Currency.Amount >= upgrade.Value.ActualCost)
                {
                    upgrade.Value.CanBuy = true;
                }
                else
                {
                    upgrade.Value.CanBuy = false;
                }

                BuildingEvents.OnCanBuyChange?.Invoke(upgrade.Value);
            }
        }
    }


    // Eventos
    void OnEnable()
    {
        CompanyEvents.OnCurrencyChange += CurrencyCheck;
    }

    void OnDisable()
    {
        CompanyEvents.OnCurrencyChange -= CurrencyCheck;
    }

    void CurrencyCheck(CurrencyHelper.CurrencyType type, CurrencyHelper.CurrencyScope scope)
    {
        CanBuyCheck(type);
    }
}
