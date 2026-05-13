using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecruitDefinition : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text Description;
    public Button UpgradeButton;

    private TripulationInstance tripulation;
    private PurchaseService PurchaseService;

    public void Setup(TripulationInstance tripulationInstance, PurchaseService purchaseService)
    {

        tripulation = tripulationInstance;

        PurchaseService = purchaseService;

        Name.text = tripulation.Name;
        Description.text = tripulation.Type.ToString();

        UpgradeButton.onClick.RemoveAllListeners();
        UpgradeButton.onClick.AddListener(OnBuyClicked);
    }

    void OnBuyClicked()
    {
        if (PurchaseService.CanBuyRecruit())
        {
            Debug.Log("Contratando!");
            GameEvents.OnTripulationPurchase?.Invoke(tripulation);
        } else
        {
            Debug.Log("Te falta prestígio!");
        }
    }
}
