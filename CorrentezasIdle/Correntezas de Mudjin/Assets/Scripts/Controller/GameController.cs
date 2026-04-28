using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public GameState GameState;
    public GameDatabase Database;

    [SerializeField] GameCreationService GameCreationService;
    [SerializeField] public MissionsService MissionsService;
    [SerializeField] public MissionsPrimaryTrackerService MissionsPrimaryTrackerService;
    [SerializeField] public MissionsSecondaryTrackerService MissionsSecondaryTrackerService;
    [SerializeField] public RecordesService RecordesService;
    [SerializeField] public CurrencyService CurrencyService;
    [SerializeField] public PurchaseService PurchaseService;
    [SerializeField] public IngredientService IngredientService;
    [SerializeField] public PermanentUpgradeService PermanentUpgradeService;
    [SerializeField] public TemporaryUpgradeService TemporaryUpgradeService;
    [SerializeField] public UnlockService UnlockService;
    [SerializeField] public EventService EventService;
    [SerializeField] public TripulationService TripulationService;

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
            GameState.TripulationState = new TripulationState();
            GameState.BestiaryState = new BestiaryState();
        } else
        {
            FirstInitialization = false;
        }

        if (FirstInitialization)
        {
            GameCreationService.Initialize(GameState, Database);
            RecordesService.Initialize(GameState);
            MissionsService.Initialize(GameState);
            MissionsPrimaryTrackerService.Initialize(GameState, GameState.DataState);
            MissionsSecondaryTrackerService.Initialize(GameState.MissionsState, MissionsService, GameState.ExpeditionState);
            CurrencyService.Initialize(GameState);
            PurchaseService.Initialize(GameState, GameState.DataState, CurrencyService);
            UnlockService.Initialize(GameState, GameState.DataState);
            PermanentUpgradeService.Initialize(GameState, GameState.DataState, GameState.ShipState, UnlockService);
            TemporaryUpgradeService.Initialize(GameState.ExpeditionState, GameState.DataState, GameState.ShipState);
            IngredientService.Initialize(GameState);
            TripulationService.Initialize(GameState);
            EventService.Initialize(GameState, TripulationService, UnlockService);
        } else
        {
            // service de Load
        }
    }
}