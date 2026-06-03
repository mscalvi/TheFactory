using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CompanyUpgradeDefinition : MonoBehaviour
{
    public TMP_Text UpgradeName;
    public TMP_Text UpgradeDescription;
    public TMP_Text UpgradeActualValue;
    public TMP_Text UpgradePrice;
    public Button UpgradeButton;
    public Image CurrencyIcon;

    private bool CanBuyUpgrade;

    private UpgradeInstance upgrade;
    private PurchaseService PurchaseService;

    public void Setup(UpgradeInstance upgradeInstance, PurchaseService purchaseService, GameState GameState)
    {
        upgrade = upgradeInstance;

        PurchaseService = purchaseService;

        if (GameState.ActualLanguage == GameState.Language.English)
        {
            UpgradeName.text = upgrade.NameEN;
            UpgradeDescription.text = upgrade.DescriptionEN;
        }

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            UpgradeName.text = upgrade.NamePT;
            UpgradeDescription.text = upgrade.DescriptionPT;
        }


        if (upgrade.ActualBuy > 0)
        {
            if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
            {
                UpgradeActualValue.text = "+" + (upgrade.CurrentValue).ToString("N2");
            }

            if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
            {
                UpgradeActualValue.text = "x" + (upgrade.CurrentValue).ToString("N2");
            }
        }
        else
        {
            if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
            {
                UpgradeActualValue.text = "+0,00";
            }

            if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
            {
                UpgradeActualValue.text = "x1,00";
            }
        }

        UpgradePrice.text = NumberHelper.Format(upgrade.ActualCost);

        string curId = ".";

        foreach (var currency in GameState.DataState.currencies.Values)
        {
            if (currency.Type == upgrade.Currency)
            {
                curId = currency.Id;
            }
        }

        Sprite icon = Resources.Load<Sprite>($"Sprites/Currencies/{curId}");

        if (icon != null)
            CurrencyIcon.sprite = icon;

        UpgradeButton.onClick.RemoveAllListeners();
        UpgradeButton.onClick.AddListener(OnBuyClicked);

        UpgradeButton.interactable = PurchaseService.CanBuyUpgrade(upgrade);
    }

    void OnBuyClicked()
    {
        PurchaseService.BuyUpgrade(upgrade);
    }
}
