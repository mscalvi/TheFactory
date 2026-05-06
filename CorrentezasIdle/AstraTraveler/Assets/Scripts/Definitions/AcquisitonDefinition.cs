using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AcquisitonDefinition : MonoBehaviour
{
    public TMP_Text UpgradeName;
    public TMP_Text UpgradeDescription;
    public TMP_Text TotalTime;

    public GameObject BuyContainer;
    public TMP_Text UpgradePrice;
    public Button UpgradeButton;

    public GameObject ProgressContainer;
    public Slider ProgressBar;
    public TMP_Text ProgressTimeText;

    private bool CanBuyUpgrade;

    private AcquisitionInstance upgrade;
    private PurchaseService PurchaseService;

    public void Setup(AcquisitionInstance upgradeInstance, PurchaseService purchaseService)
    {
        upgrade = upgradeInstance;
        PurchaseService = purchaseService;

        UpgradeName.text = upgrade.Name;
        UpgradeDescription.text = upgrade.Description;
        TotalTime.text = upgrade.TotalTime.ToString("N0");
        UpgradePrice.text = upgrade.Cost.ToString("N0");

        UpgradeButton.onClick.RemoveAllListeners();
        UpgradeButton.onClick.AddListener(OnBuyClicked);

        UpgradeButton.interactable = PurchaseService.CanBuyAcquisition(upgrade);

        RefreshState();
    }

    void OnBuyClicked()
    {
        PurchaseService.BuyAcquisition(upgrade);

        RefreshState();
    }

    void Update()
    {
        if (upgrade == null) return;

        if (upgrade.IsRunning)
        {
            UpdateProgress();
        }
    }

    void RefreshState()
    {
        if (upgrade.IsRunning)
        {
            BuyContainer.SetActive(false);
            ProgressContainer.SetActive(true);
        }
        else if (upgrade.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
        {
            // opcional: esconder tudo ou mostrar "Completo"
            BuyContainer.SetActive(false);
            ProgressContainer.SetActive(false);
        }
        else
        {
            BuyContainer.SetActive(true);
            ProgressContainer.SetActive(false);
        }

        UpgradeButton.interactable = PurchaseService.CanBuyAcquisition(upgrade);
    }

    void UpdateProgress()
    {
        if (!upgrade.IsRunning) return;

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        double total = upgrade.TotalTime;
        double remaining = upgrade.FinishTimestamp - now;
        double elapsed = total - remaining;

        float progress = (float)(elapsed / total);

        ProgressBar.value = Mathf.Clamp01(progress);

        ProgressTimeText.text = FormatTime(remaining);
    }

    string FormatTime(double seconds)
    {
        if (seconds < 0) seconds = 0;

        TimeSpan t = TimeSpan.FromSeconds(seconds);

        if (t.TotalHours >= 1)
            return $"{(int)t.TotalHours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";

        return $"{t.Minutes:D2}:{t.Seconds:D2}";
    }
    void OnEnable()
    {
        GameEvents.OnAcquisitionFinished += OnFinished;
    }

    void OnDisable()
    {
        GameEvents.OnAcquisitionFinished -= OnFinished;
    }

    void OnFinished(AcquisitionInstance acq)
    {
        if (acq.Id != upgrade.Id) return;

        RefreshState();
    }
}
