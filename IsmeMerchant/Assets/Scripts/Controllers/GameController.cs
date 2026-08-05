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
    [SerializeField] public SaveService SaveService;
    [SerializeField] public ProgressTrackerService ProgressTrackerService;
    [SerializeField] public MissionsService MissionsService;
    [SerializeField] public MissionsTrackerService MissionsTrackerService;
    [SerializeField] public UnlockService UnlockService;
    [SerializeField] public ModifierService ModifierService;
    [SerializeField] public CurrencyService CurrencyService;
    [SerializeField] public PurchaseService PurchaseService;
    [SerializeField] public IngredientService IngredientService;
    [SerializeField] public UpgradeService UpgradeService;
    [SerializeField] public RewardService RewardService;
    [SerializeField] public ConfigurationsService ConfigurationsService;

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
        Debug.Log(Application.persistentDataPath);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (GameState == null)
        {
            GameState = new GameState();
            GameState.DataState = new DataState();
            GameState.ExpeditionState = new ExpeditionState();
            GameState.ProgressState = new ProgressState();
            GameState.CompanyState = new CompanyState();
            GameState.MissionsState = new MissionsState();
            GameState.BestiaryState = new BestiaryState();
            GameState.UpgradesState = new UpgradesState();

            GameCreationService.Initialize(GameState, Database);
        }

        SaveService.Initialize(GameState);
        ProgressTrackerService.Initialize(GameState);
        MissionsService.Initialize(GameState);
        MissionsTrackerService.Initialize(GameState, MissionsService);
        CurrencyService.Initialize(GameState);
        PurchaseService.Initialize(GameState, CurrencyService);
        UnlockService.Initialize(GameState);
        ModifierService.Initialize(GameState);
        UpgradeService.Initialize(GameState, UnlockService, ModifierService);
        IngredientService.Initialize(GameState);
        RewardService.Initialize(GameState, CurrencyService);
        ConfigurationsService.Initialize(GameState);

        SaveService.Save();
    }
}