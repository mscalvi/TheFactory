using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppState
{
    public GameState GameState;

    // Linguagem
    public Language ActualLanguage = Language.Portugues;

    public enum Language
    {
        English,
        Portugues,
    }

    // Configurações Salvas
    public PlayerModel FavoritePlayer1 = null;
    public PlayerModel FavoritePlayer2 = null;
    public PlayerModel FavoritePlayer3 = null;
    public PlayerModel FavoritePlayer4 = null;
}
