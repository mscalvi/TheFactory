using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class LandingUiService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    private MissionsService MissionsService;

    [SerializeField] Button CrewButton;
    [SerializeField] Button CompanyButton;
    [SerializeField] Button ShipButton;
    [SerializeField] Button MapButton;
    [SerializeField] Button ExpeditionButton;

    [SerializeField] Transform MainMissionPanel;
    [SerializeField] MainMissionDefiniton MainMissionDefiniton;

    [SerializeField] Transform SecondaryMissionPanel;
    [SerializeField] SecondaryMissionDefinition SecondaryMissionDefinition;
    [SerializeField] SecondaryMissionButtonDefinition SecondaryMissionButtonDefinition;

    [SerializeField] MissionPopUpDesigner MissionsPopUp;

    public void Initialize(GameState gameState, DataState db, MissionsService missionsService, MissionPopUpDesigner missionPanel)
    {
        GameState = gameState;
        DataState = db;
        MissionsService = missionsService;

        MissionsPopUp = missionPanel;

        BlockButtons();
        MainMissionSet();
        SecondaryMissionSet();

        if (GameState.UnlockState.Crew)
        {
            CrewButton.enabled = true;
        }

        if (GameState.UnlockState.Company)
        {
            CompanyButton.enabled = true;
        }

        if (GameState.UnlockState.Ship)
        {
            ShipButton.enabled = true;
        }

        if (GameState.UnlockState.Map)
        {
            MapButton.enabled = true;
        }
    }

    public void SelectNewMission()
    {
        var missionsOptions = MissionsService.GenerateMissionOptions(GameState.MissionsState.MaxMissionsOptions);

        MissionsPopUp.ShowMissions(missionsOptions, MissionsSelection);
    }

    private void MissionsSelection(MissionInstance selected)
    {
        GameState.MissionsState.ActiveMissions.Add(selected);

        MissionsPopUp.Hide();

        SecondaryMissionSet();
    }

    private void BlockButtons()
    {
        CrewButton.enabled = false;
        CompanyButton.enabled = false;
        ShipButton.enabled = false;
        MapButton.enabled = false;
    }

    private void MainMissionSet()
    {
        var mission = GameState.MainMission;

        var go = Instantiate(MainMissionDefiniton, MainMissionPanel.transform);
        var ui = go.GetComponent<MainMissionDefiniton>();

        ui.Setup(mission);
    }

    private void SecondaryMissionSet()
    {
        foreach (Transform child in SecondaryMissionPanel)
            Destroy(child.gameObject);

        var activeMissions = GameState.MissionsState.ActiveMissions;

        for (int i = 0; i < GameState.MissionsState.MaxOnGoingMissions; i++)
        {
            if (i < activeMissions.Count)
            {
                var mission = activeMissions[i];

                var go = Instantiate(SecondaryMissionDefinition, SecondaryMissionPanel);
                var ui = go.GetComponent<SecondaryMissionDefinition>();

                ui.Setup(mission);
            }
            else
            {
                var go = Instantiate(SecondaryMissionButtonDefinition, SecondaryMissionPanel);
                var ui = go.GetComponent<SecondaryMissionButtonDefinition>();

                ui.Setup(this);
            }
        }
    }
}
