using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUi : MonoBehaviour
{
    private AppState AppState;

    [SerializeField] GameObject PlayersQuestionPanel;
    [SerializeField] GameObject BackgroundPanel;
    [SerializeField] GameObject TwoPlayersPanel;
    [SerializeField] GameObject FourPlayersPanel;
    [SerializeField] TextMeshProUGUI PalyersQuestionText;

    [SerializeField] PlayerDefinition Player14Panel;
    [SerializeField] PlayerDefinition Player24Panel;
    [SerializeField] PlayerDefinition Player34Panel;
    [SerializeField] PlayerDefinition Player44Panel;

    [SerializeField] PlayerDefinition Player12Panel;
    [SerializeField] PlayerDefinition Player22Panel;

    public void Initialize(AppState app)
    {
        AppState = app;

        PlayersQuestion();
    }

    // Decisão de Número de Players
    public void TwoPlayersBtn()
    {
        AppState.GameState.TotalPlayers = 2;
        PlayersQuestionPanel.SetActive(false);
        BackgroundPanel.SetActive(true);
        LoadScreen();
    }
    public void FourPlayersBtn()
    {
        AppState.GameState.TotalPlayers = 4;
        PlayersQuestionPanel.SetActive(false);
        BackgroundPanel.SetActive(true);
        LoadScreen();
    }
    private void PlayersQuestion()
    {
        if (AppState.ActualLanguage == AppState.Language.Portugues)
        {
            PalyersQuestionText.text = "Jogadores";
        }
        if (AppState.ActualLanguage == AppState.Language.English)
        {
            PalyersQuestionText.text = "Players";
        }

        PlayersQuestionPanel.SetActive(true);
        BackgroundPanel.SetActive(false);
    }

    // Montagem da Tela
    private void LoadScreen()
    {
        if (AppState.GameState.TotalPlayers == 2)
        {
            TwoPlayersScreen();
        }
        if (AppState.GameState.TotalPlayers == 4)
        {
            FourPlayersScreen();
        }
    }

    private void TwoPlayersScreen()
    {
        Player12Panel.Setup(AppState.GameState.GameInstance, AppState.GameState.GameInstance.Player1);
        Player22Panel.Setup(AppState.GameState.GameInstance, AppState.GameState.GameInstance.Player2);

        FourPlayersPanel.SetActive(false);
        TwoPlayersPanel.SetActive(true);
    }

    private void FourPlayersScreen()
    {
        Player14Panel.Setup(AppState.GameState.GameInstance, AppState.GameState.GameInstance.Player1);
        Player24Panel.Setup(AppState.GameState.GameInstance, AppState.GameState.GameInstance.Player2);
        Player34Panel.Setup(AppState.GameState.GameInstance, AppState.GameState.GameInstance.Player3);
        Player44Panel.Setup(AppState.GameState.GameInstance, AppState.GameState.GameInstance.Player4);

        TwoPlayersPanel.SetActive(false);
        FourPlayersPanel.SetActive(true);
    }
}
