using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static GameHelper;

public class ExpeditionController : MonoBehaviour
{
    [SerializeField] StartExpeditionService StartExpeditionService;

    // Base
    [SerializeField] TickService TickService;
    [SerializeField] ExpeditionCurrencyService CurrencyService;
    [SerializeField] IngredientService IngredientService;
    [SerializeField] ExpeditionPurchaseService ExpeditionPurchaseService;
    [SerializeField] ExpeditionUiService ExpeditionUiService;
    [SerializeField] MissionsTrackerService MissionsTrackerService;

    // Expedition
    [SerializeField] DaysCycleService DaysCycleService;
    [SerializeField] EnemyProgressService EnemyProgressService;
    [SerializeField] EnemySpawnerService EnemySpawnerService;
    [SerializeField] EnemyControllerService EnemyControllerService;
    [SerializeField] EnemyMarkingService EnemyMarkingService;
    [SerializeField] ExpeditionPricingService ExpeditionPricingService;
    [SerializeField] PathService PathService;

    // Ship
    [SerializeField] ShipControlService ShipControlService;
    [SerializeField] UnnamedTripulationService UnnamedTripulationService;
    [SerializeField] WeaponRoomsService WeaponRoomsService;
    [SerializeField] RangeViewService RangeViewService;
    [SerializeField] ProjectileService ProjectileService;

    // Ambos
    [SerializeField] ExpeditionService ExpeditionService;
    [SerializeField] CombatService CombatService;
    [SerializeField] DecisionsService DecisionsService;
    [SerializeField] ExpeditionUpgradeService ExpeditionUpgradeService;

    // Outros Mecanismos Úteis
    [SerializeField] DecisionsPopUpDesigner DecisionsPanel;
    [SerializeField] EventPopUpDesigner EventPanel;
    [SerializeField] FinalPopUpDesigner FinalPanel;
    [SerializeField] EndExpeditionService EndExpeditionService;
    [SerializeField] EventService EventService;

    private void Awake()
    {
        var Game = GameController.Instance.GameState;

        if (Game == null)
        {
            Debug.LogError("ExpeditionController - Game NULL!");
            return;
        }

        var GameRecords = GameController.Instance.GameRecordsService;
        var Missions = GameController.Instance.MissionsService;

        StartExpeditionService.Initialize(Game);

        var Expedition = GameController.Instance.GameState.ExpeditionState;

        if (Expedition == null)
        {
            Debug.Log("ExpeditionController - ExpeditionState NULL!");
            return;
        }

        Expedition.ExpeditionStatus = ExpeditionStatus.Paused;
        Expedition.ExpeditionUpgrades = new Dictionary<string, UpgradeInstance>();

        var Ship = GameController.Instance.GameState.ShipState;

        if (Ship == null)
        {
            Debug.Log("ExpeditionController - ShipState NULL");
            return;
        }
        else
        {
            Debug.Log($"ExpeditionController - Navio Carregado: {Ship.Ship.Name}");
        }

        var Data = GameController.Instance.GameState.DataState;
        var Unlock = GameController.Instance.GameState.UnlockState;

        // Base
        TickService.Initialize();

        CurrencyService.Initialize(Expedition, Game);

        IngredientService.Initialize(Game, Expedition);

        ExpeditionPurchaseService.Initialize(Expedition, Data, CurrencyService);

        ExpeditionUiService.Initialize(Expedition, Ship, Game, Data, ExpeditionPurchaseService);

        MissionsTrackerService.Initialize(Game.MissionsState, Missions, Expedition);

        // Expedition
        DaysCycleService.Initialize(Game, Expedition, TickService);

        EnemyProgressService.Initialize(Expedition);

        EnemySpawnerService.Initialize(Expedition, TickService, Data, EnemyProgressService);

        EnemyControllerService.Initialize(Expedition, TickService);

        EnemyMarkingService.Initialize(Expedition, Unlock);

        ExpeditionPricingService.Initialize(Expedition, Data);

        PathService.Initialize(Expedition, Ship, Game, Data);

        // Ship
        ShipControlService.Initialize(Ship, Game, TickService);

        UnnamedTripulationService.Initialize(Ship, TickService);

        RangeViewService.Initialize(Ship.Ship.WeaponsRooms);

        // Ambos
        CombatService.Initialize(Expedition, Ship, TickService, ExpeditionUiService);

        ExpeditionService.Initialize(Expedition, Ship, Game, TickService);

        WeaponRoomsService.Initialize(Expedition, Ship, TickService, CombatService);

        ExpeditionUpgradeService.Initialize(Expedition, Data, Ship);

        // Outros
        DecisionsService.Initialize(Expedition, Ship, Game, DecisionsPanel, TickService, Data, FinalPanel, PathService, EventPanel);

        EndExpeditionService.Initialize(Expedition, Game, DecisionsService);

        EventService.Initialize(Game, Data, Expedition);

        // Events
        ExpeditionEvents.OnExpeditionStart?.Invoke();
    }
}