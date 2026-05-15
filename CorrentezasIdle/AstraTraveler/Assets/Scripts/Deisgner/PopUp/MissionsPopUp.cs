using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
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

    public void ShowMissions(List<MissionInstance> options, Action<MissionInstance> onSelected, GameState GameState)
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

            if (GameState.ActualLanguage == GameState.Language.English)
            {
                Opt1Title.text = options[0].NameEN;

                Opt1Text.text = options[0].DescriptionEN;
            }

            if (GameState.ActualLanguage == GameState.Language.Portugues)
            {
                Opt1Title.text = options[0].NamePT;

                Opt1Text.text = options[0].DescriptionPT;
            }

            Opt1Btn.onClick.AddListener(() => onSelected(options[0]));
        }

        if (options.Count > 1)
        {
            Opt2Btn.gameObject.SetActive(true);

            if (GameState.ActualLanguage == GameState.Language.English)
            {
                Opt2Title.text = options[1].NameEN;

                Opt2Text.text = options[1].DescriptionEN;
            }

            if (GameState.ActualLanguage == GameState.Language.Portugues)
            {
                Opt2Title.text = options[1].NamePT;

                Opt2Text.text = options[1].DescriptionPT;
            }

            Opt2Btn.onClick.AddListener(() => onSelected(options[1]));
        }

        if (options.Count > 2)
        {
            Opt3Btn.gameObject.SetActive(true);

            if (GameState.ActualLanguage == GameState.Language.English)
            {
                Opt3Title.text = options[2].NameEN;

                Opt3Text.text = options[2].DescriptionEN;
            }

            if (GameState.ActualLanguage == GameState.Language.Portugues)
            {
                Opt3Title.text = options[2].NamePT;

                Opt3Text.text = options[2].DescriptionPT;
            }

            Opt3Btn.onClick.AddListener(() => onSelected(options[2]));
        }

        if (options.Count > 3)
        {
            Opt4Btn.gameObject.SetActive(true);

            if (GameState.ActualLanguage == GameState.Language.English)
            {
                Opt4Title.text = options[3].NameEN;

                Opt4Text.text = options[3].DescriptionEN;
            }

            if (GameState.ActualLanguage == GameState.Language.Portugues)
            {
                Opt4Title.text = options[3].NamePT;

                Opt4Text.text = options[3].DescriptionPT;
            }

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
