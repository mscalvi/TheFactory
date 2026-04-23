using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionPurchaseService : MonoBehaviour
{
    private ExpeditionState ExpeditionState;
    private DataState DataState;
    private ExpeditionCurrencyService CurrencyService;

    private Dictionary<string, UpgradeInstance> ActiveUpgrades;

    public void Initialize(ExpeditionState expeditionState, DataState dataState, ExpeditionCurrencyService currencyService)
    {
        ExpeditionState = expeditionState;

        DataState = dataState;

        CurrencyService = currencyService;

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
        if (!ExpeditionState.ExpeditionCurrency.TryGetValue(upgrade.Currency, out var Currency))
            return;

        if (Currency.Amount >= upgrade.ActualCost)
        {
            CurrencyService.Spend(upgrade.Currency, upgrade.ActualCost);

            ShipEvents.OnUpgradeBuy?.Invoke(upgrade);
        }
    }

    public void CanBuyCheck(CurrencyHelper.CurrencyType type)
    {
        if (!ExpeditionState.ExpeditionCurrency.TryGetValue(type, out var Currency))
            return;

        foreach (var upgrade in ActiveUpgrades)
        {
            if (upgrade.Value.Currency == type)
            {
                if (Currency.Amount >= upgrade.Value.ActualCost)
                {
                    upgrade.Value.CanBuy = true;
                } else
                {
                    upgrade.Value.CanBuy = false;
                }

                ShipEvents.OnCanBuyChange?.Invoke(upgrade.Value);
            }
        }
    }

    public bool CanBuyUpgrade(UpgradeInstance upgrade)
    {
        if (!ExpeditionState.ExpeditionCurrency.TryGetValue(upgrade.Currency, out var currency))
            return false;

        return currency.Amount >= upgrade.ActualCost;
    }


    // Eventos
    void OnEnable()
    {
        ExpeditionEvents.OnCurrencyChange += CurrencyCheck;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnCurrencyChange -= CurrencyCheck;
    }

    void CurrencyCheck(CurrencyHelper.CurrencyType type, CurrencyHelper.CurrencyScope scope)
    {
        CanBuyCheck(type);
    }
}
