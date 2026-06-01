using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AcquisitonDefinition : MonoBehaviour
{
    public TMP_Text UpgradeName;
    public TMP_Text UpgradeDescription;
    public TMP_Text TotalTime;

    public GameObject BuyContainer;
    public TMP_Text UpgradePrice;
    public Image CurrencyIcon;
    public Button UpgradeButton;

    public GameObject ProgressContainer;
    public Slider ProgressBar;
    public TMP_Text ProgressTimeText;

    private bool CanBuyUpgrade;

    private AcquisitionInstance Acquisition;
    private PurchaseService PurchaseService;
    private GameState GameState;

    public void Setup(AcquisitionInstance acq, PurchaseService purchaseService, GameState game)
    {
        Acquisition = acq;
        PurchaseService = purchaseService;
        GameState = game;

        if (GameState.ActualLanguage == GameState.Language.English)
        {
            UpgradeName.text = Acquisition.NameEN;
            UpgradeDescription.text = Acquisition.DescriptionEN;
        }

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            UpgradeName.text = Acquisition.NamePT;
            UpgradeDescription.text = Acquisition.DescriptionPT;
        }

        string curId = ".";

        foreach (var currency in GameState.DataState.currencies.Values)
        {
            if (currency.Type == acq.Currency)
            {
                curId = currency.Id;
            }
        }

        Sprite icon = Resources.Load<Sprite>($"Sprites/Currencies/{curId}");

        if (icon != null)
            CurrencyIcon.sprite = icon;

        string time = FormatTime(acq.TotalTime);
        TotalTime.text = time;

        UpgradePrice.text = NumberHelper.Format(acq.ActualCost);

        UpgradeButton.onClick.RemoveAllListeners();
        UpgradeButton.onClick.AddListener(OnBuyClicked);

        UpgradeButton.interactable = PurchaseService.CanBuyAcquisition(Acquisition);

        RefreshState();
    }

    void OnBuyClicked()
    {
        PurchaseService.BuyAcquisition(Acquisition);

        RefreshState();
    }

    void RefreshState()
    {
        if (Acquisition.IsRunning)
        {
            BuyContainer.SetActive(false);
            ProgressContainer.SetActive(true);
        }
        else if (Acquisition.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
        {
            BuyContainer.SetActive(false);
            ProgressContainer.SetActive(false);
        }
        else
        {
            BuyContainer.SetActive(true);
            ProgressContainer.SetActive(false);
        }

        UpgradeButton.interactable = PurchaseService.CanBuyAcquisition(Acquisition);
    }

    string FormatTime(double seconds)
    {
        if (seconds < 0) seconds = 0;

        TimeSpan t = TimeSpan.FromSeconds(seconds);

        if (t.TotalHours >= 1)
            return $"{(int)t.TotalHours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";

        return $"{t.Minutes:D2}:{t.Seconds:D2}";
    }

    void OnChange(AcquisitionInstance acq)
    {
        if (acq.Id != Acquisition.Id) return;

        RefreshState();
    }

    void OnProgress(AcquisitionInstance acq, float progress, double remaining)
    {
        if (acq.Id != Acquisition.Id) return;

        ProgressBar.value = progress;
        ProgressTimeText.text = FormatTime(remaining);
    }

    void OnEnable()
    {
        GameEvents.OnAcquisitionStarted += OnChange;
        GameEvents.OnAcquisitionProgress += OnProgress;
    }

    void OnDisable()
    {
        GameEvents.OnAcquisitionStarted -= OnChange;
        GameEvents.OnAcquisitionProgress -= OnProgress;
    }
}
