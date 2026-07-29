using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionDefinition : MonoBehaviour
{
    public TMP_Text UpgradeName;
    public TMP_Text UpgradeDescription;
    public TMP_Text TotalTime;
    public TMP_Text Level;

    public GameObject BuyContainer;
    public TMP_Text UpgradePrice;
    public Image CurrencyIcon;
    public Button UpgradeButton;

    public GameObject ProgressContainer;
    public Slider ProgressBar;
    public TMP_Text ProgressTimeText;

    private bool CanBuyUpgrade;

    private ConstructionInstance Construction;
    private PurchaseService PurchaseService;
    private GameState GameState;
    class Modifier
    {
        public double AdCompMod = 0;
        public double AdExpeMod = 0;
        public double MtCompMod = 1;
        public double MtExpeMod = 1;
    }

    public void Setup(ConstructionInstance acq, PurchaseService purchaseService, GameState game)
    {
        Construction = acq;
        PurchaseService = purchaseService;
        GameState = game;

        CalculateStart(acq);

        if (GameState.ActualLanguage == GameState.Language.English)
        {
            UpgradeName.text = Construction.NameEN;
            UpgradeDescription.text = Construction.DescriptionEN;

            Level.text = "Level: " + acq.Level.ToString();
        }

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            UpgradeName.text = Construction.NamePT;
            UpgradeDescription.text = Construction.DescriptionPT;

            Level.text = "Nível: " + acq.Level.ToString();
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

        string time = FormatTime(acq.ActualTime);
        TotalTime.text = time;

        UpgradePrice.text = NumberHelper.Format(acq.ActualCost);

        UpgradeButton.onClick.RemoveAllListeners();
        UpgradeButton.onClick.AddListener(OnBuyClicked);

        UpgradeButton.interactable = PurchaseService.CanBuyConstruction(Construction);

        RefreshState();
    }

    private void CalculateStart(ConstructionInstance acq)
    {
        var timeModifier = ApplyModifiers(UpgradeHelper.EffectType.ConstructionTime);
        var costModifier = ApplyModifiers(UpgradeHelper.EffectType.ConstructionCost);

        acq.BaseCost = (acq.StartCost + costModifier.AdCompMod) * costModifier.MtCompMod;
        acq.ActualCost = (acq.BaseCost + costModifier.AdExpeMod) * costModifier.MtExpeMod;

        acq.BaseTime = (float)((acq.StartTime + timeModifier.AdCompMod) * timeModifier.MtCompMod);
        acq.ActualTime = (float)((acq.BaseTime + timeModifier.AdExpeMod) * timeModifier.MtExpeMod);
    }

    private Modifier ApplyModifiers(UpgradeHelper.EffectType type)
    {
        var mod = new Modifier();

        foreach (var modifier in GameState.UpgradesState.Modifiers)
        {
            if (modifier.Type != type)
                continue;

            if (modifier.Opp == UpgradeHelper.UpgradeType.Additive)
            {
                if (modifier.Scope == UpgradeHelper.UpgradeScope.Company)
                {
                    mod.AdCompMod += modifier.Value;
                }
            }

            if (modifier.Opp == UpgradeHelper.UpgradeType.Multiplicative)
            {
                if (modifier.Scope == UpgradeHelper.UpgradeScope.Company)
                {
                    mod.MtCompMod *= modifier.Value;
                }
            }
        }

        return mod;
    }

    void OnBuyClicked()
    {
        PurchaseService.BuyConstruction(Construction);

        RefreshState();
    }

    void RefreshState()
    {
        if (Construction.IsRunning)
        {
            BuyContainer.SetActive(false);
            ProgressContainer.SetActive(true);
        }
        else if (Construction.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
        {
            BuyContainer.SetActive(false);
            ProgressContainer.SetActive(false);
        }
        else
        {
            BuyContainer.SetActive(true);
            ProgressContainer.SetActive(false);
        }

        UpgradeButton.interactable = PurchaseService.CanBuyConstruction(Construction);
    }

    string FormatTime(double seconds)
    {
        if (seconds < 0) seconds = 0;

        TimeSpan t = TimeSpan.FromSeconds(seconds);

        if (t.TotalHours >= 1)
            return $"{(int)t.TotalHours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";

        return $"{t.Minutes:D2}:{t.Seconds:D2}";
    }

    void OnChange(ConstructionInstance acq)
    {
        if (acq.Id != Construction.Id) return;

        RefreshState();
    }

    void OnProgress(ConstructionInstance acq, float progress, double remaining)
    {
        if (acq.Id != Construction.Id) return;

        ProgressBar.value = progress;
        ProgressTimeText.text = FormatTime(remaining);
    }

    void OnEnable()
    {
        GameEvents.OnConstructionStarted += OnChange;
        GameEvents.OnConstructionProgress += OnProgress;
    }

    void OnDisable()
    {
        GameEvents.OnConstructionStarted -= OnChange;
        GameEvents.OnConstructionProgress -= OnProgress;
    }
}
