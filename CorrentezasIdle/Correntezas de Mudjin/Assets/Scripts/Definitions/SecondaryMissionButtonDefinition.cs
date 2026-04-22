using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SecondaryMissionButtonDefinition : MonoBehaviour
{
    public Button Button;

    public void Setup()
    {
        Button.onClick.RemoveAllListeners();

        Button.interactable = true;

        Button.onClick.AddListener(OnBuyClicked);
    }

    void OnBuyClicked()
    {

    }
}
