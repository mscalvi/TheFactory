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

    [SerializeField] Button StudyButton;
    public TMP_Text StudyName;
    [SerializeField] Button UpgradesButton;
    public TMP_Text UpgradesName;
    [SerializeField] Button AcquisitionsButton;
    public TMP_Text AcquisitonsName;
    [SerializeField] Button TrainingButton;
    public TMP_Text TrainingName;
    [SerializeField] Button ShipButton;
    public TMP_Text ShipName;
    [SerializeField] Button AlchemyButton;
    public TMP_Text AlchemyName;

    [SerializeField] Button ExpeditionButton;

    [SerializeField] Button BestiaryButton;
    public TMP_Text BestiaryName;

    [SerializeField] GameObject ConfigsPanel;
    [SerializeField] GameObject MenusPanel;

    [SerializeField] Transform MissionPanel;
    [SerializeField] MissionDefinition MissionDefinition;
    [SerializeField] MissionButtonDefinition MissionButtonDefinition;

    [SerializeField] MissionsPopUp MissionsPopUp;

    Dictionary<CurrencyType, CompanyCurrencyDefinition> currencyUi = new();
    [SerializeField] Transform CompanyCurrencyPanel;
    [SerializeField] CompanyCurrencyDefinition CurrencyPrefab;

    public void Initialize(GameState gameState, MissionsService missionsService)
    {
        GameState = gameState;
        MissionsService = missionsService;

        BlockButtons();
        ReleaseButtons();
        MissionSet();
        BuildCurrencies(CompanyCurrencyPanel);
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
        foreach (Transform child in MissionPanel)
            Destroy(child.gameObject);

        var slots = GameState.MissionsState.Slots;

        foreach (var slot in slots)
        {
            if (slot.ActiveMission != null)
            {
                var go = Instantiate(MissionDefinition, MissionPanel);
                var ui = go.GetComponent<MissionDefinition>();

                ui.Setup(slot.ActiveMission, GameState);
            }
            else if (IsSlotOnCooldown(slot))
            {
                var go = Instantiate(MissionButtonDefinition, MissionPanel);
                var ui = go.GetComponent<MissionButtonDefinition>();

                ui.SetupCooldown(slot);
            }
            else
            {
                var go = Instantiate(MissionButtonDefinition, MissionPanel);
                var ui = go.GetComponent<MissionButtonDefinition>();

                ui.SetupAvailable(this, slot);
            }
        }
    }

    // Buttons
    private void BlockButtons()
    {
        BestiaryButton.interactable = false;

        StudyButton.interactable = false;
        UpgradesButton.interactable = false;
        AcquisitionsButton.interactable = false;
        ShipButton.interactable = false;
        TrainingButton.interactable = false;
        AlchemyButton.interactable = false;
    }
    private void ReleaseButtons()
    {
        var Unlock = GameState.UnlockState;

        if (GameState.ExpeditionState.ExpeditionStatus == GameHelper.ExpeditionStatus.Running)
            return;

        StudyButton.interactable = Unlock.Studies;
        UpgradesButton.interactable = Unlock.Company;
        AcquisitionsButton.interactable = Unlock.Acquisitions;
        ShipButton.interactable = Unlock.Ship;
        TrainingButton.interactable = Unlock.Training || Unlock.Recruiting;
        AlchemyButton.interactable = Unlock.Alchemy;

        BestiaryButton.interactable = Unlock.Bestiary;

        if (Unlock.Studies)
        {
            StudyName.text = "Estudos";
        }
        if (Unlock.Company)
        {
            UpgradesName.text = "Melhorias";
        }
        if (Unlock.Acquisitions)
        {
            AcquisitonsName.text = "Construções";
        }
        if (Unlock.Ship)
        {
            ShipName.text = "Navio";
        }
        if (Unlock.Training || Unlock.Recruiting)
        {
            TrainingName.text = "Tripulação";
        }
        if (Unlock.Alchemy)
        {
            AlchemyName.text = "Alquimia";
        }

        if (Unlock.Bestiary)
        {
            BestiaryName.text = "Bestiário";
        }
    }
    public void ExpeditionButtonFunction()
    {
        SceneManager.LoadScene("ExpeditionScene");
    }
    public void SettingsButtonFunction()
    {
        bool active = ConfigsPanel.activeSelf;

        ConfigsPanel.SetActive(!active);
        MenusPanel.SetActive(active);
    }
    public void BotaoSecretoDaGrana()
    {
        GameEvents.MoneyTest?.Invoke();
    }


    public void BestiaryButtonFunction()
    {
        SceneManager.LoadScene("BestiaryScene");
    }


    public void StudiesButtonFuncion()
    {
        SceneManager.LoadScene("StudiesScene");
    }
    public void UpgradesButtonFuncion()
    {
        SceneManager.LoadScene("BuildingsScene");
    }
    public void AcquisitionButtonFuncion()
    {
        SceneManager.LoadScene("AcquisitionsScene");
    }
    public void ShipButtonFuncion()
    {
        SceneManager.LoadScene("ShipScene");
    }
    public void TrainingButtonFuncion()
    {
        SceneManager.LoadScene("TripulationScene");
    }
    public void AlchemyButtonFuncion()
    {
        SceneManager.LoadScene("AlchemyScene");
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
}