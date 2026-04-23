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
    [SerializeField] public CompanyCurrencyService CompanyCurrencyService;
    [SerializeField] public MainMissionsTrakerService MainMissionsTrakerService;
    [SerializeField] public CompanyPurchaseService CompanyPurchaseService;
    [SerializeField] public CompanyUpgradeService CompanyUpgradeService;
    [SerializeField] public CompanyPricingService CompanyPricingService;
    [SerializeField] public ShipInitializeService ShipInitializeService;
    [SerializeField] public UnlockService UnlockService;

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
            Debug.LogError("GameController - GameDatabase NÃO atribuída no GameController!");
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
            GameState.ShipState = new ShipState();
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
            CompanyCurrencyService.Initialize(GameState);
            CompanyPurchaseService.Initialize(GameState, GameState.DataState, CompanyCurrencyService);
            UnlockService.Initialize(GameState, GameState.DataState);
            CompanyUpgradeService.Initialize(GameState, GameState.DataState, GameState.ShipState, UnlockService);
            CompanyPricingService.Initialize(GameState, GameState.DataState);
            ShipInitializeService.Initialize(GameState);
        } else
        {
            // service de Load
        }
    }
}