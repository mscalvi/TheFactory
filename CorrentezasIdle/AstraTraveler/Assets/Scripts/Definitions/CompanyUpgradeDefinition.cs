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


        if(upgrade.Scope == UpgradeHelper.UpgradeScope.Expedition)
        {
            if(upgrade.UpgradeType == UpgradeHelper.UpgradeType.Additive)
            {
                UpgradeActualValue.text = "+" + (upgrade.ActualUpgradeValue).ToString("N2");
            }

            if (upgrade.UpgradeType == UpgradeHelper.UpgradeType.Multiplicative)
            {
                UpgradeActualValue.text = "x" + (upgrade.ActualUpgradeValue).ToString("N2");
            }
        }
        else
        {
            UpgradeActualValue.text = "";
        }

        UpgradePrice.text = upgrade.ActualCost.ToString("N0");

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

        CanBuyUpgrade = PurchaseService.CanBuyUpgrade(upgrade);
        UpgradeButton.interactable = CanBuyUpgrade;
    }

    void OnBuyClicked()
    {
        PurchaseService.BuyUpgrade(upgrade);
    }
}
