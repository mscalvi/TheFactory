using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationsService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState game)
    {
        GameState = game;
    }

    public void IncreaseGameSpeed()
    {
        Debug.Log(GameState.MaxGameSpeed);

        if (GameState.ActualGameSpeed < GameState.MaxGameSpeed)
        {
            GameState.ActualGameSpeed += 0.5f;
        }
    }
    public void DecreaseGameSpeed()
    {
        Debug.Log(GameState.MaxGameSpeed);

        if (GameState.ActualGameSpeed > 1)
        {
            GameState.ActualGameSpeed -= 0.5f;
        }
    }
    public void SelectLanguage(int index)
    {
        GameState.ActualLanguage = (GameState.Language)index;

        GameEvents.OnLanguageChange?.Invoke();
    }
}
