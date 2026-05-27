using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public void Initialize(GameState gameState, MissionsService missionsService)
    {
        GameState = gameState;
        MissionsService = missionsService;

        BlockButtons();
        ReleaseButtons();
        MissionSet();
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

    public void SettingsButtonFuncion()
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
}
