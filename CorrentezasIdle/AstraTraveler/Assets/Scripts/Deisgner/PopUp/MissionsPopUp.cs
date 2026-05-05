using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionsPopUp : MonoBehaviour
{    
    [SerializeField] GameObject DecisionPanel;
    [SerializeField] Button CancelButton;
    [SerializeField] Button Opt1Btn;
    [SerializeField] Button Opt2Btn;
    [SerializeField] Button Opt3Btn;
    [SerializeField] Button Opt4Btn;
    [SerializeField] TextMeshProUGUI Opt1Title;
    [SerializeField] TextMeshProUGUI Opt2Title;
    [SerializeField] TextMeshProUGUI Opt3Title;
    [SerializeField] TextMeshProUGUI Opt4Title;
    [SerializeField] TextMeshProUGUI Opt1Text;
    [SerializeField] TextMeshProUGUI Opt2Text;
    [SerializeField] TextMeshProUGUI Opt3Text;
    [SerializeField] TextMeshProUGUI Opt4Text;
    [SerializeField] TextMeshProUGUI Title;

    public void ShowMissions(List<MissionInstance> options, Action<MissionInstance> onSelected)
    {
        Hide();

        Opt1Btn.onClick.RemoveAllListeners();
        Opt2Btn.onClick.RemoveAllListeners();
        Opt3Btn.onClick.RemoveAllListeners();
        Opt4Btn.onClick.RemoveAllListeners();

        DecisionPanel.SetActive(true);

        Title.text = "Escolha um Trabalho";

        if (options.Count > 0)
        {
            Opt1Btn.gameObject.SetActive(true);
            Opt1Title.text = options[0].Name;
            Opt1Text.text = options[0].Description;
            Opt1Btn.onClick.AddListener(() => onSelected(options[0]));
        }

        if (options.Count > 1)
        {
            Opt2Btn.gameObject.SetActive(true);
            Opt2Title.text = options[1].Name;
            Opt2Text.text = options[1].Description;
            Opt2Btn.onClick.AddListener(() => onSelected(options[1]));
        }

        if (options.Count > 2)
        {
            Opt3Btn.gameObject.SetActive(true);
            Opt3Title.text = options[2].Name;
            Opt3Text.text = options[2].Description;
            Opt3Btn.onClick.AddListener(() => onSelected(options[2]));
        }

        if (options.Count > 3)
        {
            Opt4Btn.gameObject.SetActive(true);
            Opt4Title.text = options[3].Name;
            Opt4Text.text = options[3].Description;
            Opt4Btn.onClick.AddListener(() => onSelected(options[3]));
        }
    }

    public void Hide()
    {
        DecisionPanel.SetActive(false);
        Opt1Btn.gameObject.SetActive(false);
        Opt2Btn.gameObject.SetActive(false);
        Opt3Btn.gameObject.SetActive(false);
        Opt4Btn.gameObject.SetActive(false);
        Title.text = null;
    }
}
