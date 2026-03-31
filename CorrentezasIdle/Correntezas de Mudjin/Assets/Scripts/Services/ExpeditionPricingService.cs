using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionPricingService : MonoBehaviour
{
    private ExpeditionState ExpeditionState;
    private DataState DataState;

    public void Initialize(ExpeditionState expeditionState, DataState dataState)
    {
        ExpeditionState = expeditionState;

        DataState = dataState;
    }

    private void AtualizePrice(UpgradeInstance upgrade)
    {
        Debug.Log($"Velho preço do {upgrade.Id}: {upgrade.ActualCost}");
        upgrade.ActualCost = upgrade.Cost * System.Math.Pow(upgrade.ActualBuy + 1, upgrade.CostGrowth);
        Debug.Log($"Novo preço do {upgrade.Id}: {upgrade.ActualCost}");

        ShipEvents.AfterUpgradeBuy?.Invoke(upgrade);
    }

    // Eventos
    void OnEnable()
    {
        ShipEvents.OnUpgradeBuy += PriceAtt;
    }

    void OnDisable()
    {
        ShipEvents.OnUpgradeBuy -= PriceAtt;
    }

    void PriceAtt(UpgradeInstance upgrade)
    {
        AtualizePrice(upgrade);
    }
}
