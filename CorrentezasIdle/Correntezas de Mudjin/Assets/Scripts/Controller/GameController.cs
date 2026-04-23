using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public GameState GameState;
    public GameDatabase Database;

    [SerializeField] GameCreationService GameCreationService;
    [SerializeField] public GameRecordsService GameRecordsService;
    [SerializeField] public MissionsService MissionsService;
    [SerializeField] MainMissionsTrakerService MainMissionsTrakerService;

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
            GameState.ExpeditionState = new ExpeditionState();
            GameState.ProgressState = new ProgressState();
            GameState.CompanyState = new CompanyState();
            GameState.UnlockState = new UnlockState();
            GameState.RecordsState = new RecordsState();
            GameState.MissionsState = new MissionsState();
        } else
        {
            FirstInitialization = false;
        }

        if (FirstInitialization)
        {
            GameCreationService.Initialize(GameState, Database);
            GameRecordsService.Initialize(GameState);
            MissionsService.Initialize(GameState);
            MainMissionsTrakerService.Initialize(GameState, GameState.DataState);
        } else
        {
            // service de Load
        }
    }
}