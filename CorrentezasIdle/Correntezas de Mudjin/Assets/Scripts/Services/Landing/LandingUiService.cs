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

        if (GameState.FirstExpedition)
        {
            BlockButtons();
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
