using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DestinationsPopUp : MonoBehaviour
{    
    [SerializeField] GameObject MissionPanel;
    [SerializeField] Button Opt1Btn;
    [SerializeField] Button Opt2Btn;
    [SerializeField] Button Opt3Btn;
    [SerializeField] TextMeshProUGUI Opt1Title;
    [SerializeField] TextMeshProUGUI Opt2Title;
    [SerializeField] TextMeshProUGUI Opt3Title;
    [SerializeField] TextMeshProUGUI Opt1Text;
    [SerializeField] TextMeshProUGUI Opt2Text;
    [SerializeField] TextMeshProUGUI Opt3Text;
    [SerializeField] TextMeshProUGUI Title;

    public void ShowMissions(List<TripulationInstance> options, Action<TripulationInstance> onSelected)
    {
        Hide();

        Opt1Btn.onClick.RemoveAllListeners();
        Opt2Btn.onClick.RemoveAllListeners();
        Opt3Btn.onClick.RemoveAllListeners();

        MissionPanel.SetActive(true);

        Title.text = "Novo Membro para a Tripulação!";

        if (options.Count > 0)
        {
            Opt1Btn.gameObject.SetActive(true);
            Opt1Title.text = options[0].Name;
            Opt1Text.text = options[0].DescriptionPT;
            Opt1Btn.onClick.AddListener(() => onSelected(options[0]));
        }

        if (options.Count > 1)
        {
            Opt2Btn.gameObject.SetActive(true);
            Opt2Title.text = options[1].Name;
            Opt2Text.text = options[1].DescriptionPT;
            Opt2Btn.onClick.AddListener(() => onSelected(options[1]));
        }

        if (options.Count > 2)
        {
            Opt3Btn.gameObject.SetActive(true);
            Opt3Title.text = options[2].Name;
            Opt3Text.text = options[2].DescriptionPT;
            Opt3Btn.onClick.AddListener(() => onSelected(options[2]));
        }
    }

    public void Hide()
    {
        MissionPanel.SetActive(false);
        Opt1Btn.gameObject.SetActive(false);
        Opt2Btn.gameObject.SetActive(false);
        Opt3Btn.gameObject.SetActive(false);
        Title.text = null;
    }
}
