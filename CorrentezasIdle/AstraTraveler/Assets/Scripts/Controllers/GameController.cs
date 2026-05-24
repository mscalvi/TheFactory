using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public GameState GameState;
    public GameDatabase Database;

    [SerializeField] GameCreationService GameCreationService;
    [SerializeField] DatabaseService DatabaseService;
    [SerializeField] SaveService SaveService;
    [SerializeField] public ProgressTrackerService ProgressTrackerService;
    [SerializeField] public MissionsService MissionsService;
    [SerializeField] public MissionsTrackerService MissionsTrackerService;
    [SerializeField] public UnlockService UnlockService;
    [SerializeField] public CurrencyService CurrencyService;
    [SerializeField] public PurchaseService PurchaseService;
    [SerializeField] public IngredientService IngredientService;
    [SerializeField] public UpgradeService UpgradeService;
    [SerializeField] public EventService EventService;
    [SerializeField] public AcquisitonsService AcquisitonsService;
    [SerializeField] public RecruitmentService RecruitmentService;
    [SerializeField] public RewardService RewardService;
    [SerializeField] public ConfigurationsService ConfigurationsService;

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
            Database = DatabaseService.Initialize();
        }

        GameState = SaveService.Load();

        if (GameState == null)
        {
            GameState = new GameState();
            GameState.DataState = new DataState();
            GameState.ExpeditionState = new ExpeditionState();
            GameState.ProgressState = new ProgressState();
            GameState.CompanyState = new CompanyState();
            GameState.UnlockState = new UnlockState();
            GameState.MissionsState = new MissionsState();
            GameState.BestiaryState = new BestiaryState();

            FirstInitialization = true;
        } else
        {
            FirstInitialization = false;
        }

        if (FirstInitialization)
        {
            GameCreationService.Initialize(GameState, Database);
        } else
        {
            // service de Load
        }

        SaveService.Initialize(GameState);
        ProgressTrackerService.Initialize(GameState);
        MissionsService.Initialize(GameState);
        MissionsTrackerService.Initialize(GameState, MissionsService);
        CurrencyService.Initialize(GameState);
        PurchaseService.Initialize(GameState, CurrencyService);
        UnlockService.Initialize(GameState);
        UpgradeService.Initialize(GameState, UnlockService);
        IngredientService.Initialize(GameState);
        EventService.Initialize(GameState, UnlockService);
        AcquisitonsService.Initialize(GameState);
        RecruitmentService.Initialize(GameState);
        RewardService.Initialize(GameState, CurrencyService);
        ConfigurationsService.Initialize(GameState);

        SaveService.Save();
    }
}