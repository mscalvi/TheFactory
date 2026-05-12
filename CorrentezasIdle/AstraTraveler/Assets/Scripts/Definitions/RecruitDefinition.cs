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
    private UnlockService UnlockService;

    public void Setup(TripulationInstance tripulationInstance, UnlockService unlockService)
    {

        tripulation = tripulationInstance;

        UnlockService = unlockService;

        Name.text = tripulation.Name;
        Description.text = tripulation.Type.ToString();

        UpgradeButton.onClick.RemoveAllListeners();
        UpgradeButton.onClick.AddListener(OnBuyClicked);
    }

    void OnBuyClicked()
    {
        UnlockService.UnlockTripulation(tripulation);
    }
}
