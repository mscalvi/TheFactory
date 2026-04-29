using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static GameHelper;

public class ExpeditionController : MonoBehaviour
{
    // Base
    [SerializeField] TickService TickService;
    [SerializeField] ExpeditionControlService ExpeditionControlService;
    [SerializeField] ExpeditionUiService ExpeditionUiService;
    [SerializeField] PathService PathService;
    [SerializeField] DaysCycleService DaysCycleService;
    [SerializeField] DecisionsService DecisionsService;
    [SerializeField] EventTriggerService EventTriggerService;

    // Enemy
    [SerializeField] EnemyProgressService EnemyProgressService;
    [SerializeField] EnemySpawnerService EnemySpawnerService;
    [SerializeField] EnemyControllerService EnemyControllerService;
    [SerializeField] EnemyMarkingService EnemyMarkingService;
    [SerializeField] BestiaryTrackerService BestiaryTrackerService;

    // Ship
    [SerializeField] UnnamedTripulationService UnnamedTripulationService;
    [SerializeField] WeaponRoomsService WeaponRoomsService;
    [SerializeField] RangeViewService RangeViewService;
    [SerializeField] ProjectileService ProjectileService;
    [SerializeField] CombatService CombatService;

    // Outros Mecanismos Úteis
    [SerializeField] DecisionsPopUpDesigner DecisionsPanel;
    [SerializeField] EventPopUpDesigner EventPanel;
    [SerializeField] FinalPopUpDesigner FinalPanel;

    private void Awake()
    {
        var Game = GameController.Instance.GameState;

        if (Game == null)
        {
            Debug.LogError("ExpeditionController - Game NULL!");
            return;
        }

        TickService.Initialize();

        var GameRecords = GameController.Instance.RecordesService;
        var Missions = GameController.Instance.MissionsService;
        var CurrencyService = GameController.Instance.CurrencyService;
        var PurchaseService = GameController.Instance.PurchaseService;

        ExpeditionControlService.Initialize(Game, TickService);

        var Expedition = GameController.Instance.GameState.ExpeditionState;

        if (Expedition == null)
        {
            Debug.Log("ExpeditionController - ExpeditionState NULL!");
            return;
        }

        Expedition.ExpeditionStatus = ExpeditionStatus.Paused;

        var Ship = GameController.Instance.GameState.ShipState;

        if (Ship == null)
        {
            Debug.Log("ExpeditionController - ShipState NULL");
            return;
        }

        var Data = GameController.Instance.GameState.DataState;
        var Unlock = GameController.Instance.GameState.UnlockState;

        // Base

        ExpeditionUiService.Initialize(Expedition, Ship, Game, Data, PurchaseService);

        // Expedition
        DaysCycleService.Initialize(Game, Expedition, TickService);

        EnemyProgressService.Initialize(Expedition);

        EnemySpawnerService.Initialize(Expedition, TickService, Data, EnemyProgressService);

        EnemyControllerService.Initialize(Expedition, TickService);

        EnemyMarkingService.Initialize(Expedition, Unlock);

        PathService.Initialize(Expedition, Ship, Game, Data);

        BestiaryTrackerService.Initialize(Game);

        // Ship

        UnnamedTripulationService.Initialize(Game, Ship, TickService);

        RangeViewService.Initialize(Ship.Ship.WeaponsRooms);

        // Ambos
        CombatService.Initialize(Expedition, Ship, TickService, ExpeditionUiService);

        WeaponRoomsService.Initialize(Game, Expedition, Ship, TickService, CombatService);

        // Outros
        DecisionsService.Initialize(Expedition, Ship, Game, DecisionsPanel, TickService, Data, FinalPanel, PathService, EventPanel);

        EventTriggerService.Initialize(Game, Data, Expedition);
    }

    private void Start()
    {
        ExpeditionEvents.OnExpeditionStart?.Invoke();
    }
}