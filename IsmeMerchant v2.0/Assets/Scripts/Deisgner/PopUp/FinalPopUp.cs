using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalPopUp : MonoBehaviour
{
    [SerializeField] GameObject FinalPanel;
    [SerializeField] Button Close;
    [SerializeField] TextMeshProUGUI Title;
    [SerializeField] TextMeshProUGUI Info;

    private ExpeditionState ExpeditionState;

    public void ShowResults(ExpeditionState expeditionState, Action<bool> finalResult)
    {
        Hide();

        ExpeditionState = expeditionState;

        Close.onClick.RemoveAllListeners();

        FinalPanel.SetActive(true);
        Close.gameObject.SetActive(true);

        Title.text = "Naufrágio!";

        Info.text = "A Expedição chegou ao final... Hora de se preparar para a próxima!";

        Close.onClick.AddListener(() => finalResult?.Invoke(true));
    }

    public void Hide()
    {
        FinalPanel.SetActive(false);
        Title.text = null;
        Info.text = null;
    }
}
