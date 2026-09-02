using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCreationService : MonoBehaviour
{
    private AppState AppState;

    public void Initialize(AppState App)
    {
        AppState = App;

        AppState.GameState.GameInstance = new GameInstance(new GameModel());

        FavoriteFormat();
        FavoritePlayers();
    }

    private void FavoriteFormat()
    {
        AppState.GameState.GameInstance.Format = FormatHelper.Format.Other;
    }

    private void FavoritePlayers()
    {
        AppState.GameState.GameInstance.Player1 = new PlayerInstance(new PlayerModel());
        AppState.GameState.GameInstance.Player1.CurrentDeck = new DeckInstance(new DeckModel());
        AppState.GameState.GameInstance.Player2 = new PlayerInstance(new PlayerModel());
        AppState.GameState.GameInstance.Player2.CurrentDeck = new DeckInstance(new DeckModel());
        AppState.GameState.GameInstance.Player3 = new PlayerInstance(new PlayerModel());
        AppState.GameState.GameInstance.Player3.CurrentDeck = new DeckInstance(new DeckModel());
        AppState.GameState.GameInstance.Player4 = new PlayerInstance(new PlayerModel());
        AppState.GameState.GameInstance.Player4.CurrentDeck = new DeckInstance(new DeckModel());
    }
}
