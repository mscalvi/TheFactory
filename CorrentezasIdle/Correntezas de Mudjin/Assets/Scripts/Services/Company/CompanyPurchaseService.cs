using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyPurchaseService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    private CompanyCurrencyService CompanyCurrencyService;

    public void Initialize(GameState gameState, DataState dataState, CompanyCurrencyService currencyService)
    {
        GameState = gameState;

        DataState = dataState;

        CompanyCurrencyService = currencyService;
    }

    public void BuyUpgrade(UpgradeInstance upgrade)
    {
        if (!GameState.CompanyCurrency.TryGetValue(upgrade.Currency, out var Currency))
            return;

        if (Currency.Amount >= upgrade.ActualCost)
        {
            CompanyCurrencyService.Spend(upgrade.Currency, upgrade.ActualCost);

            CanBuyCheck(upgrade.Currency);

            CompanyEvents.OnUpgradeBuy?.Invoke(upgrade);
        }
    }

    public void CanBuyCheck(CurrencyHelper.CurrencyType type)
    {
        if (!GameState.CompanyCurrency.TryGetValue(type, out var Currency))
            return;

        foreach (var upgrade in GameState.CompanyUpgrades)
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

                CompanyEvents.OnCanBuyChange?.Invoke(upgrade.Value);
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
