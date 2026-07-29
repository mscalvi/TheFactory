using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecruitDefinition : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text Class;
    public TMP_Text Rarity;
    public TMP_Text Upgrades;
    public Button UpgradeButton;

    private TripulationInstance tripulation;
    private PurchaseService PurchaseService;

    public void Setup(TripulationInstance tripulationInstance, PurchaseService purchaseService, GameState GameState)
    {

        tripulation = tripulationInstance;

        PurchaseService = purchaseService;

        Name.text = tripulation.Name;

        if (GameState.ActualLanguage == GameState.Language.English)
        {

            switch (tripulation.Type)
            {
                case TripulationHelper.Type.Captain:
                    Class.text = "Captain";
                    break;
                case TripulationHelper.Type.Shipbuilder:
                    Class.text = "Shipbuilder";
                    break;
                case TripulationHelper.Type.Weaponsmith:
                    Class.text = "Weaponsmith";
                    break;
                case TripulationHelper.Type.Merchant:
                    Class.text = "Merchant";
                    break;
                case TripulationHelper.Type.Hunter:
                    Class.text = "Hunter";
                    break;
                case TripulationHelper.Type.Fisherman:
                    Class.text = "Fisherman";
                    break;
                case TripulationHelper.Type.Alchemist:
                    Class.text = "Alchemist";
                    break;
                case TripulationHelper.Type.Coach:
                    Class.text = "Coach";
                    break;
            }

            switch (tripulation.Rarity)
            {
                case GameHelper.ItemRarity.Common:
                    Rarity.text = "Common";
                    break;
                case GameHelper.ItemRarity.Uncommon:
                    Rarity.text = "Uncommon";
                    break;
                case GameHelper.ItemRarity.Rare:
                    Rarity.text = "Rare";
                    break;
                case GameHelper.ItemRarity.Legendary:
                    Rarity.text = "Legendary";
                    break;
            }

            foreach (var upgrade in GameState.DataState.upgrades.Values)
            {
                if (upgrade.UnlockId == tripulation.Id)
                {
                    Upgrades.text += upgrade.NameEN + "\n";
                }
            }
        }

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            switch (tripulation.Type)
            {
                case TripulationHelper.Type.Captain:
                    Class.text = "Capitão";
                    break;
                case TripulationHelper.Type.Shipbuilder:
                    Class.text = "Marceneiro";
                    break;
                case TripulationHelper.Type.Weaponsmith:
                    Class.text = "Ferreiro";
                    break;
                case TripulationHelper.Type.Merchant:
                    Class.text = "Mercador";
                    break;
                case TripulationHelper.Type.Hunter:
                    Class.text = "Caçador";
                    break;
                case TripulationHelper.Type.Fisherman:
                    Class.text = "Pescador";
                    break;
                case TripulationHelper.Type.Alchemist:
                    Class.text = "Alquimista";
                    break;
                case TripulationHelper.Type.Coach:
                    Class.text = "Treinador";
                    break;
            }

            switch (tripulation.Rarity) 
            {
                case GameHelper.ItemRarity.Common:
                    Rarity.text = "Comum";
                    break;
                case GameHelper.ItemRarity.Uncommon:
                    Rarity.text = "Incomum";
                    break;
                case GameHelper.ItemRarity.Rare:
                    Rarity.text = "Raro";
                    break;
                case GameHelper.ItemRarity.Legendary:
                    Rarity.text = "Lendário";
                    break;
            }

            foreach (var upgrade in GameState.DataState.upgrades.Values)
            {
                if (upgrade.UnlockId == tripulation.Id)
                {
                    Upgrades.text += upgrade.NamePT + "\n";
                }
            }
        }

        UpgradeButton.onClick.RemoveAllListeners();
        UpgradeButton.onClick.AddListener(OnBuyClicked);

        UpgradeButton.interactable = PurchaseService.CanBuyRecruit();
    }

    void OnBuyClicked()
    {
        if (PurchaseService.CanBuyRecruit())
        {
            Debug.Log("Contratando!");
            PurchaseService.BuyTripulation(tripulation);
        }
        else
        {
            Debug.Log("Te falta prestígio!");
        }
    }
}
