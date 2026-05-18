using System.Collections;
using System.Collections.Generic;
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

    public void BuyAcquisition(AcquisitionInstance upgrade)
    {
        if (!GameState.DataState.currencies.TryGetValue(upgrade.Currency, out var Currency))
            return;

        if (Currency.Amount >= upgrade.ActualCost)
        {
            CurrencyService.Spend(upgrade.Currency, upgrade.ActualCost);

            GameEvents.OnAcquisitionBuy?.Invoke(upgrade);
        }

        CanBuyCurrency(upgrade.Currency);
    }

    private void BuyTripulation(TripulationInstance tripulation)
    {
        CurrencyService.Spend(CurrencyType.Prestige, GameState.ExpeditionState.ActiveTripulation.Count);
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

        return currency.Amount >= upgrade.ActualCost;
    }

    public bool CanBuyRecruit()
    {
        if (GameState.DataState.currencies[CurrencyHelper.CurrencyType.Prestige].Amount >= GameState.ExpeditionState.ActiveTripulation.Count)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public bool CanBuyAcquisition(AcquisitionInstance upgrade)
    {
        if (!GameState.DataState.currencies.TryGetValue(upgrade.Currency, out var currency))
            return false;

        if (GameState.CompanyState.ActiveAcquisitons.Count >= GameState.CompanyState.MaxAcquisitionsSlots)
        {
            if (GameState.CompanyState.AcquisitionsQueue.Count >= GameState.CompanyState.MaxAcquisitonsQueue)
            {
                return false;
            }
        }

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
        GameEvents.OnTripulationPurchase += BuyTripulation;
    }

    void OnDisable()
    {
        GameEvents.OnCurrencyChange -= CurrencyCheck;
        GameEvents.OnTripulationPurchase -= BuyTripulation;
    }

    void CurrencyCheck(CurrencyHelper.CurrencyType type, CurrencyHelper.CurrencyScope scope)
    {
        CanBuyCurrency(type);
    }
}
