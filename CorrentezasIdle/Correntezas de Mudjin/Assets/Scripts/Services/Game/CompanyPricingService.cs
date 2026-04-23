using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyPricingService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    public void Initialize(GameState gameState, DataState dataState)
    {
        GameState = gameState;

        DataState = dataState;
    }

    private void AtualizePrice(UpgradeInstance upgrade)
    {
        upgrade.ActualCost = upgrade.Cost * System.Math.Pow(upgrade.ActualBuy + 1, upgrade.CostGrowth);

        upgrade.ActualCost = (int)upgrade.ActualCost;

        CompanyEvents.AfterUpgradeBuy?.Invoke(upgrade);
    }

    // Eventos
    void OnEnable()
    {
        CompanyEvents.OnUpgradeBuy += PriceAtt;
    }

    void OnDisable()
    {
        CompanyEvents.OnUpgradeBuy -= PriceAtt;
    }

    void PriceAtt(UpgradeInstance upgrade)
    {
        AtualizePrice(upgrade);
    }
}
