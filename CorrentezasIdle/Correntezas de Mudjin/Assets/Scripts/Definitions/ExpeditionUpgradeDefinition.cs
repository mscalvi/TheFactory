using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionUpgradeDefinition : MonoBehaviour
{
    public TMP_Text UpgradeName;
    public TMP_Text UpgradeDescription;
    public TMP_Text UpgradeActualValue;
    public TMP_Text UpgradePrice;
    public Button UpgradeButton;

    private bool CanBuyUpgrade;

    private UpgradeInstance upgrade;
    private ExpeditionPurchaseService ExpeditionPurchaseService;

    public void Setup(UpgradeInstance upgradeInstance, ExpeditionPurchaseService purchaseService)
    {
        upgrade = upgradeInstance;

        ExpeditionPurchaseService = purchaseService;

        UpgradeName.text = upgrade.Name;
        UpgradeDescription.text = upgrade.Description;

        if(upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
        {
            if(upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
            {
                UpgradeActualValue.text = "+" + (upgrade.ActualValue).ToString("N2");
            }

            if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
            {
                UpgradeActualValue.text = "x" + (upgrade.ActualValue).ToString("N2");
            }
        }
        else
        {
            UpgradeActualValue.text = "";
        }

        UpgradePrice.text = upgrade.ActualCost.ToString("N0");

        UpgradeButton.onClick.RemoveAllListeners();
        UpgradeButton.onClick.AddListener(OnBuyClicked);

        CanBuyUpgrade = upgrade.CanBuy;

        UpgradeButton.interactable = CanBuyUpgrade;
    }

    void OnBuyClicked()
    {
        ExpeditionPurchaseService.BuyUpgrade(upgrade);
    }
}
