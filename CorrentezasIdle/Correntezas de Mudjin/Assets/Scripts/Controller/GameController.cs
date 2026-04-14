using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public GameState GameState;
    public GameDatabase Database;

    [SerializeField] GameCreationService GameCreationService;
    [SerializeField] public GameProgressService GameProgressService;

    bool FirstInitialization = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        if (Database == null)
        {
            Debug.LogError("GameDatabase NÃO atribuída no GameController!");
        }

        if (GameState == null)
        {
            GameState = new GameState();
            GameState.DataState = new DataState();
            GameState.ProgressState = new ProgressState();
            GameState.CompanyState = new CompanyState();
            GameState.UnlockState = new UnlockState();
        } else
        {
            FirstInitialization = false;
        }

        if (FirstInitialization)
        {
            GameCreationService.Initialize(GameState, Database);
            GameProgressService.Initialize(GameState);
        } else
        {
            // service de Load
        }
    }
}