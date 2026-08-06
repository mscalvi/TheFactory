using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static CurrencyHelper;

public class PurchaseService : MonoBehaviour
{
    private GameState GameState;

    private CurrencyService CurrencyService;

    public void Initialize(GameState gameState, CurrencyService currencyService)
    {
        GameState = gameState;

        CurrencyService = currencyService;

        foreach (var currency in GameState.DataState.currencies)
        {
            CanBuyCurrency(currency.Value.Type);
        }
    }

    public void BuyUpgrade(UpgradeInstance upgrade)
    {
        if (!GameState.DataState.currencies.TryGetValue(upgrade.Currency, out var Currency))
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
        if (!GameState.DataState.currencies.TryGetValue(type, out var Currency))
            return;

        bool needAtt = false;
        bool initialState = false;

        foreach (var upgrade in GameState.DataState.upgrades)
        {
            if (upgrade.Value.UnlockStatus != UnlockHelper.UnlockStatus.Available)
                continue;

            if (upgrade.Value.Currency == type)
            {
                initialState = upgrade.Value.CanBuy;

                if (Currency.Amount >= upgrade.Value.ActualCost)
                {
                    upgrade.Value.CanBuy = true;
                    if (initialState == upgrade.Value.CanBuy)
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

            if (upgrade.Value.MaxBuy <= upgrade.Value.ActualBuy && upgrade.Value.MaxBuy != 0)
            {
                needAtt = true;
            }
        }

        if (needAtt)
        {
            GameEvents.OnCanBuyChange?.Invoke(type, CurrencyHelper.CurrencyScope.Company);
        }
    }

    public bool CanBuyUpgrade(UpgradeInstance upgrade)
    {
        if (!GameState.DataState.currencies.TryGetValue(upgrade.Currency, out var currency))
            return false;

        if (upgrade.Scope == UpgradeHelper.UpgradeScope.Company)
        {
            var building = upgrade.Building;

            foreach(var build in GameState.DataState.buildings.Values)
            {
                if (build.Type != building)
                    continue;

                if (build.Level * 10 <= upgrade.ActualBuy)
                    return false;
            }
        }

        return currency.Amount >= upgrade.ActualCost;
    }

    private void AtualizePrice(UpgradeInstance upgrade)
    {
        double lastCost = upgrade.ActualCost;

        upgrade.ActualCost = upgrade.StartCost * System.Math.Pow(upgrade.CostGrowth, upgrade.ActualBuy + 1);

        upgrade.ActualCost = (int)Math.Ceiling(upgrade.ActualCost);

        if (upgrade.ActualCost <= lastCost)
        {
            upgrade.ActualCost = lastCost + 1;
        }

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
