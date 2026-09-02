using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class LandingUi : MonoBehaviour
{
    private AppState AppState;
    private DataService DataService;

    [SerializeField] TextMeshProUGUI NewGameText;
    [SerializeField] TextMeshProUGUI RegisterText;
    [SerializeField] TextMeshProUGUI StatisticsText;
    [SerializeField] TextMeshProUGUI SettingsText;

    [SerializeField] GameObject RegisterPanel;
    [SerializeField] GameObject RegisterPlayerInfoPanel;
    [SerializeField] GameObject RegisterDeckInfoPanel;
    [SerializeField] TextMeshProUGUI CancelRegisterText;
    [SerializeField] TextMeshProUGUI ConfirmRegisterText;
    [SerializeField] TextMeshProUGUI RegisterTypeText;
    [SerializeField] TextMeshProUGUI RegisterPlayerText;
    [SerializeField] TextMeshProUGUI RegisterDeckText;
    [SerializeField] TMP_InputField PlayerNameInput;
    [SerializeField] TMP_Dropdown FavoritePositionDropdown;
    [SerializeField] TMP_InputField DeckNameInput;
    [SerializeField] Toggle WhiteToggle;
    [SerializeField] Toggle BlueToggle;
    [SerializeField] Toggle BlackToggle;
    [SerializeField] Toggle RedToggle;
    [SerializeField] Toggle GreenToggle;
    [SerializeField] Toggle CommanderDamageToggle;
    [SerializeField] Toggle PoisonToggle;
    [SerializeField] Toggle ExperienceToggle;
    [SerializeField] TMP_Dropdown FavoritePlayerDropdown;
    private List<PlayerModel> DropdownPlayers = new();
    private bool RegisterDeckMode;
    private bool RegisterPlayerMode;

    public void Initialize(AppState appState, DataService data)
    {
        AppState = appState;
        DataService = data;

        if (AppState.ActualLanguage == AppState.Language.Portugues)
        {
            NewGameText.text = "Novo Jogo";
            RegisterText.text = "Cadastrar";
            StatisticsText.text = "Estatísticas";
            SettingsText.text = "Configurações";

            CancelRegisterText.text = "Cancelar";
            ConfirmRegisterText.text = "Confirmar";
        }

        if (AppState.ActualLanguage == AppState.Language.English)
        {
            NewGameText.text = "New Game";
            RegisterText.text = "Register";
            StatisticsText.text = "Statistics";
            SettingsText.text = "Settings";

            CancelRegisterText.text = "Cancel";
            ConfirmRegisterText.text = "Confirm";
        }
    }

    // Botões
    public void NewGameBtn()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void RegisterBtn()
    {
        PopulateFavoritePositions();
        PopulatePlayersDropdown();
        RegisterPanel.SetActive(true);
    }

    // Register
    private void PopulateFavoritePositions()
    {
        FavoritePositionDropdown.ClearOptions();

        List<string> options = new()
        {
            "None",
            "Player 1",
            "Player 2",
            "Player 3",
            "Player 4"
        };

        FavoritePositionDropdown.AddOptions(options);
    }
    private void PopulatePlayersDropdown()
    {
        FavoritePlayerDropdown.ClearOptions();
        DropdownPlayers.Clear();

        var players = DataService.GetPlayers();

        List<string> options = new();

        foreach (PlayerModel player in players)
        {
            options.Add(player.Name);
            DropdownPlayers.Add(player);
        }

        FavoritePlayerDropdown.AddOptions(options);
    }

    public void PlayerRegister()
    {
        RegisterDeckInfoPanel.SetActive(false);
        RegisterPlayerInfoPanel.SetActive(true);

        RegisterPlayerMode = true;
        RegisterDeckMode = false;
    }
    public void DeckRegister()
    {
        RegisterPlayerInfoPanel.SetActive(false);
        RegisterDeckInfoPanel.SetActive(true);

        RegisterPlayerMode = false;
        RegisterDeckMode = true;
    }
    public void CancelRegisterBtn()
    {
        PlayerNameInput.text = "";
        FavoritePositionDropdown.value = 0;

        DeckNameInput.text = "";

        RegisterPanel.SetActive(false);
    }
    public void ConfirmRegisterBtn()
    {
        if (RegisterPlayerMode)
        {
            PlayerModel player = new PlayerModel();

            player.Name = PlayerNameInput.text;

            if (player.Name == null) return;

            switch (FavoritePositionDropdown.value)
            {
                case 0:
                    break;
                case 1:
                    AppState.FavoritePlayer1 = player;
                    break;
                case 2:
                    AppState.FavoritePlayer2 = player;
                    break;
                case 3:
                    AppState.FavoritePlayer3 = player;
                    break;
                case 4:
                    AppState.FavoritePlayer4 = player;
                    break;
            }

            DataService.CreatePlayer(player);
        }

        if (RegisterDeckMode)
        {
            DeckModel deck = new DeckModel();

            deck.Name = DeckNameInput.text;

            if (deck.Name == null) return;

            deck.White = WhiteToggle.isOn;
            deck.Blue = BlueToggle.isOn;
            deck.Black = BlackToggle.isOn;
            deck.Red = RedToggle.isOn;
            deck.Green = GreenToggle.isOn;

            deck.CommanderDamage = CommanderDamageToggle.isOn;
            deck.Poison = PoisonToggle.isOn;
            deck.Experience = ExperienceToggle.isOn;

            int selectedIndex = FavoritePlayerDropdown.value;

            if (selectedIndex == 0)
            {
                deck.FavoritePlayerId = null;
            }
            else
            {
                deck.FavoritePlayerId = DropdownPlayers[selectedIndex - 1].Id;
            }

            DataService.CreateDeck(deck);
        }

        RegisterPanel.SetActive(false);
    }
}
