using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState GameState;
    //public GameDatabase Database;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        //if (Database == null)
        //{
        //    Database = DatabaseService.Initialize();
        //}

        if (GameState == null)
        {
            GameState = new GameState();
            GameState.Grid = new GridState();
            GameState.Expedition = new ExpeditionState();
        }
        else
        {

        }
    }
}