using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalPopUpDesigner : MonoBehaviour
{
    [SerializeField] GameObject FinalPanel;
    [SerializeField] Button Close;
    [SerializeField] TextMeshProUGUI Title;
    [SerializeField] TextMeshProUGUI Info;

    private ExpeditionState ExpeditionState;

    public void ShowResults(bool victory, ExpeditionState expeditionState, Action<bool> finalResult)
    {
        Hide();

        ExpeditionState = expeditionState;

        Close.onClick.RemoveAllListeners();

        FinalPanel.SetActive(true);
        Close.gameObject.SetActive(true);

        if (victory) 
        {
            Title.text = "Você chegou ao seu Destino!";
        }
        else
        {
            Title.text = "A Expedição fracassou.";
        }

        Close.onClick.AddListener(() => finalResult?.Invoke(true));
    }

    public void Hide()
    {
        FinalPanel.SetActive(false);
        Title.text = null;
        Info.text = null;
    }
}
