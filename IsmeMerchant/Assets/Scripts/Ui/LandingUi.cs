using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CurrencyHelper;

public class LandingUi : MonoBehaviour
{
    private GameState GameState;

    private MissionsService MissionsService;
    private UnlockService UnlockService;
    private PurchaseService PurchaseService;

    [SerializeField] Button ExpeditionButton;

    [SerializeField] GameObject BigBtnPanel;
    [SerializeField] GameObject SmallBtnPanel;

    [SerializeField] MissionDefinition MissionDefinition;
    [SerializeField] MissionButtonDefinition MissionButtonDefinition;

    [SerializeField] MissionsPopUp MissionsPopUp;
    [SerializeField] UpgradesPopUp UpgradesPopUp;
    [SerializeField] BuildingsPopUp BuildingsPopUp;
    [SerializeField] BestiaryPopUp BestiaryPopUp;

    [SerializeField] GameObject ConfigsPopUp;
    [SerializeField] Transform MissionPopUp;

    Dictionary<CurrencyType, CompanyCurrencyDefinition> currencyUi = new();
    [SerializeField] Transform CompanyCurrencyPanel;
    [SerializeField] CompanyCurrencyDefinition CurrencyPrefab;

    public void Initialize(GameState gameState, MissionsService missionsService, UnlockService unlockService, PurchaseService purchaseSercice)
    {
        GameState = gameState;
        MissionsService = missionsService;
        UnlockService = unlockService;
        PurchaseService = purchaseSercice;

        BlockButtons();
        // MissionSet();
        BuildCurrencies(CompanyCurrencyPanel);
        CheckUpgrades();
        ReleaseButtons();
    }

    // Missions
    public void SelectNewMission(MissionSlotModel slot)
    {
        if (IsSlotOnCooldown(slot))
            return;

        var missionsOptions = MissionsService.GenerateMissionOptions(GameState.MissionsState.MaxMissionsOptions);

        if (missionsOptions.Count == 0)
        {
            return;
        }

        MissionsPopUp.ShowMissions(missionsOptions, (selected) =>
        {
            MissionsService.AssignMissionToSlot(selected, slot);

            MissionsPopUp.Hide();

            MissionSet();
        }, GameState);
    }
    private bool IsSlotOnCooldown(MissionSlotModel slot)
    {
        long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return slot.CooldownEnd > now;
    }
    private void MissionSet()
    {
        foreach (Transform child in MissionPopUp)
            Destroy(child.gameObject);

        var slots = GameState.MissionsState.Slots;

        foreach (var slot in slots)
        {
            if (slot.ActiveMission != null)
            {
                var go = Instantiate(MissionDefinition, MissionPopUp);
                var ui = go.GetComponent<MissionDefinition>();

                ui.Setup(slot.ActiveMission, GameState);
            }
            else if (IsSlotOnCooldown(slot))
            {
                var go = Instantiate(MissionButtonDefinition, MissionPopUp);
                var ui = go.GetComponent<MissionButtonDefinition>();

                ui.SetupCooldown(slot);
            }
            else
            {
                var go = Instantiate(MissionButtonDefinition, MissionPopUp);
                var ui = go.GetComponent<MissionButtonDefinition>();

                ui.SetupAvailable(this, slot);
            }
        }
    }

    // Buttons
    private void BlockButtons()
    {
        SmallBtnPanel.SetActive(false);
        BigBtnPanel.SetActive(false);
    }

    private void ReleaseButtons()
    {
        var Unlock = GameState.ProgressState;

        if (GameState.ExpeditionState.ExpeditionStatus == GameHelper.ExpeditionStatus.Running)
            return;

        if (GameState.ExpeditionState.ExpeditionsDone < 1) 
            return;

        SmallBtnPanel.SetActive(true);
        BigBtnPanel.SetActive(true);
    }    

    public void BotaoSecretoDaGrana()
    {
        GameEvents.MoneyTest?.Invoke();
    }

    public void ExpeditionButtonFunction()
    {
        SceneManager.LoadScene("ExpeditionScene");
    }

    public void BestiaryButtonFuncion()
    {
        BestiaryPopUp.Show(GameState);
    }
    public void RoomsButtonFuncion()
    {
        BuildingsPopUp.Show(GameState, PurchaseService);
    }

    // Currency
    public void BuildCurrencies(Transform parent)
    {
        var currencies = GameState.DataState.currencies;

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        currencyUi.Clear();

        var ordered = new List<CurrencyInstance>();

        foreach (var pair in currencies)
        {
            var c = pair.Value;

            if (c.Scope != CurrencyScope.Company)
                continue;

            if (c.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
                continue;

            ordered.Add(c);
        }

        ordered.Sort((a, b) => string.Compare(a.Id, b.Id));

        foreach (var currency in ordered)
        {
            var obj = Instantiate(CurrencyPrefab, parent);

            var ui = obj.GetComponent<CompanyCurrencyDefinition>();
            ui.Setup(currency, GameState.DataState);

            currencyUi[currency.Type] = ui;
        }
    }

    // Upgrades
    private void CheckUpgrades()
    {
        Debug.Log($"Checando Upgrades: {GameState.ProgressState.UnlockableExpeditionUpgrades}/{GameState.ProgressState.UnlockableCompanyUpgrades}");
        if (GameState.ProgressState.UnlockableExpeditionUpgrades > 0)
        {
            Debug.Log("Encontrado Upgrade de Expedição");
            for (int i = GameState.ProgressState.UnlockableExpeditionUpgrades; i > 0; i--)
            {
                var expUpgradesOptions = UnlockService.UpgradeOptions(UpgradeHelper.UpgradeScope.Expedition);

                UpgradesPopUp.ShowUpgrades(expUpgradesOptions, (selected) =>
                {
                    selected.UnlockStatus = UnlockHelper.UnlockStatus.Available;

                    UpgradesPopUp.Hide();
                }, GameState);
            }
        }
        if (GameState.ProgressState.UnlockableCompanyUpgrades > 0)
        {
            Debug.Log("Encontrado Upgrade de Companhia");
            for (int i = GameState.ProgressState.UnlockableCompanyUpgrades; i > 0; i--)
            {
                var compUpgradesOptions = UnlockService.UpgradeOptions(UpgradeHelper.UpgradeScope.Company);

                UpgradesPopUp.ShowUpgrades(compUpgradesOptions, (selected) =>
                {
                    selected.UnlockStatus = UnlockHelper.UnlockStatus.Available;

                    UpgradesPopUp.Hide();
                }, GameState);
            }
        }

        GameState.ProgressState.UnlockableExpeditionUpgrades = 0;
        GameState.ProgressState.UnlockableCompanyUpgrades = 0;
    }

    // Eventos
    void OnEnable()
    {
        GameEvents.OnCurrencyChange += RefreshCurrencyUi;
        GameEvents.OnCanBuyChange += RefreshCurrencyUi;
    }

    void OnDisable()
    {
        GameEvents.OnCurrencyChange -= RefreshCurrencyUi;
        GameEvents.OnCanBuyChange -= RefreshCurrencyUi;
    }

    void RefreshCurrencyUi(CurrencyType type, CurrencyScope scope)
    {
        BuildCurrencies(CompanyCurrencyPanel);
    }
}