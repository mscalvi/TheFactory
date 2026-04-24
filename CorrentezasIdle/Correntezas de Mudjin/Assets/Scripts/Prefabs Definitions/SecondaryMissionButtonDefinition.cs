using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SecondaryMissionButtonDefinition : MonoBehaviour
{
    public Button Button;

    private LandingUiService LandingUiService;

    public void Setup(LandingUiService landingUiService)
    {
        LandingUiService = landingUiService;

        Button.onClick.RemoveAllListeners();
        Button.interactable = true;

        Button.onClick.AddListener(OnBuyClicked);
    }

    void OnBuyClicked()
    {
        LandingUiService.SelectNewMission();
    }
}
