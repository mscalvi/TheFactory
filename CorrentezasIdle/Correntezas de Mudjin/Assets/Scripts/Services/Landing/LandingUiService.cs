using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandingUiService : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    [SerializeField] Button CrewButton;
    [SerializeField] Button CompanyButton;
    [SerializeField] Button ShipButton;
    [SerializeField] Button MapButton;
    [SerializeField] Button ExpeditionButton;

    public void Initialize(GameState gameState, DataState db)
    {
        GameState = gameState;
        DataState = db;

        BlockButtons();

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

    private void BlockButtons()
    {
        CrewButton.enabled = false;
        CompanyButton.enabled = false;
        ShipButton.enabled = false;
        MapButton.enabled = false;
    }
}
