using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EventPopUpDesigner : MonoBehaviour
{
    [SerializeField] GameObject DecisionPanel;
    [SerializeField] Button Btn;
    [SerializeField] TextMeshProUGUI Text;
    [SerializeField] TextMeshProUGUI Title;

    public void ShowEvent(EventInstance evento, Action<bool> confirm)
    {
        Hide();

        Btn.onClick.RemoveAllListeners();

        DecisionPanel.SetActive(true);

        Btn.gameObject.SetActive(true);
        Title.text = evento.Title;
        Text.text = evento.Info;
        Btn.onClick.AddListener(() => confirm?.Invoke(true));
    }

    public void Hide()
    {
        DecisionPanel.SetActive(false);
    }
}
