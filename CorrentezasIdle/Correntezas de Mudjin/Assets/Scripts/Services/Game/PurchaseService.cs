using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    private CurrencyService CurrencyService;

    public void Initialize(GameState gameState, DataState dataState, CurrencyService currencyService)
    {
        GameState = gameState;

        DataState = dataState;

        CurrencyService = currencyService;

        foreach (var currency in GameState.DataState.currencies)
        {
            CanBuyCurrency(currency.Value.Type);
        }
    }

    public void BuyUpgrade(UpgradeInstance upgrade)
    {
        if (!GameState.CompanyState.CompanyCurrency.TryGetValue(upgrade.Currency, out var Currency))
            return;

        if (Currency.Amount >= upgrade.ActualCost)
        {
            CurrencyService.Spend(upgrade.Currency, upgrade.ActualCost);

            upgrade.ActualBuy++;

            AtualizePrice(upgrade);

            GameEvents.OnUpgradeBuy?.Invoke(upgrade);
        }

        CanBuyCurrency(upgrade.Currency);
    }

    public void CanBuyCurrency(CurrencyHelper.CurrencyType type)
    {
        if (!GameState.CompanyState.CompanyCurrency.TryGetValue(type, out var Currency))
            return;

        bool needAtt = false;
        bool initialState = false;

        foreach (var upgrade in GameState.CompanyState.CompanyUpgrades)
        {
            if (upgrade.Value.Currency == type)
            {
                initialState = upgrade.Value.CanBuy;

                if (Currency.Amount >= upgrade.Value.ActualCost)
                {
                    upgrade.Value.CanBuy = true;
                    if(initialState == upgrade.Value.CanBuy)
                    {
                        needAtt = true;
                    }
                }
                else
                {
                    upgrade.Value.CanBuy = false;
                    if (initialState == upgrade.Value.CanBuy)
                    {
                        needAtt = true;
                    }
                }
            }
        }

        if (needAtt)
        {
            GameEvents.OnCanBuyChange?.Invoke(type, CurrencyHelper.CurrencyScope.Company);
        }
    }

    public bool CanBuyUpgrade(UpgradeInstance upgrade)
    {
        if (!GameState.CompanyState.CompanyCurrency.TryGetValue(upgrade.Currency, out var currency))
            return false;

        return currency.Amount >= upgrade.ActualCost;
    }

    private void AtualizePrice(UpgradeInstance upgrade)
    {
        upgrade.ActualCost = upgrade.Cost * System.Math.Pow(upgrade.ActualBuy + 1, upgrade.CostGrowth);

        upgrade.ActualCost = (int)upgrade.ActualCost;

        CanBuyUpgrade(upgrade);
    }

    // Eventos
    void OnEnable()
    {
        GameEvents.OnCurrencyChange += CurrencyCheck;
    }

    void OnDisable()
    {
        GameEvents.OnCurrencyChange -= CurrencyCheck;
    }

    void CurrencyCheck(CurrencyHelper.CurrencyType type, CurrencyHelper.CurrencyScope scope)
    {
        CanBuyCurrency(type);
    }
}
