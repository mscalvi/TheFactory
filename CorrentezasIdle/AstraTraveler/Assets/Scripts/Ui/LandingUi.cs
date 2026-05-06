using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LandingUi : MonoBehaviour
{
    private GameState GameState;

    private MissionsService MissionsService;

    [SerializeField] Button StudyButton;
    [SerializeField] Button UpgradesButton;
    [SerializeField] Button AcquisitionsButton;
    [SerializeField] Button TrainingButton;
    [SerializeField] Button ShipButton;
    [SerializeField] Button AlchemyButton;

    [SerializeField] Button ExpeditionButton;

    [SerializeField] Button BestiaryButton;

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

        MissionsPopUp.ShowMissions(missionsOptions, (selected) =>
        {
            MissionsService.AssignMissionToSlot(selected, slot);

            MissionsPopUp.Hide();

            MissionSet();
        });
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

                ui.Setup(slot.ActiveMission);
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
        TrainingButton.interactable = Unlock.Training;
        AlchemyButton.interactable = Unlock.Alchemy;
        BestiaryButton.interactable = Unlock.Bestiary;
    }

    public void ExpeditionButtonFunction()
    {
        SceneManager.LoadScene("ExpeditionScene");
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
        SceneManager.LoadScene("TrainingScene");
    }
    public void AlchemyButtonFuncion()
    {
        SceneManager.LoadScene("AlchemyScene");
    }
}
