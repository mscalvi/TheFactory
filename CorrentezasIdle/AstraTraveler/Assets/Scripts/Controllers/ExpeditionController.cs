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
    [SerializeField] ExpeditionUi ExpeditionUiService;
    [SerializeField] PathService PathService;
    [SerializeField] DaysCycleService DaysCycleService;
    [SerializeField] DecisionsService DecisionsService;
    [SerializeField] ExpeditionStartService StartService;

    // Enemy
    [SerializeField] EnemyProgressService EnemyProgressService;
    [SerializeField] EnemySpawnerService EnemySpawnerService;
    [SerializeField] EnemyControllerService EnemyControllerService;
    [SerializeField] EnemyMarkingService EnemyMarkingService;
    [SerializeField] BestiaryTrackerService BestiaryTrackerService;

    // Ship
    [SerializeField] ProjectileService ProjectileService;
    [SerializeField] WeaponsService WeaponsService;
    [SerializeField] RepairService RepairService;

    private void Awake()
    {
        var Game = GameController.Instance.GameState;

        if (Game == null)
        {
            Debug.LogError("ExpeditionController - Game NULL!");
            return;
        }

        TickService.Initialize(Game);

        var Missions = GameController.Instance.MissionsService;
        var CurrencyService = GameController.Instance.CurrencyService;
        var PurchaseService = GameController.Instance.PurchaseService;

        PathService.Initialize(Game);

        var Expedition = GameController.Instance.GameState.ExpeditionState;
        if (Expedition == null)
        {
            Debug.Log("ExpeditionController - ExpeditionState NULL!");
            return;
        }

        Expedition.ExpeditionStatus = ExpeditionStatus.Loading;

        StartService.Initialize(Game, PathService);

        var Ship = GameController.Instance.GameState.ExpeditionState.Ship;
        if (Ship == null)
        {
            Debug.Log("ExpeditionController - ShipState NULL");
            return;
        }

        var Configs = GameController.Instance.ConfigurationsService;

        ExpeditionControlService.Initialize(Game, TickService);

        ExpeditionUiService.Initialize(Game, PurchaseService, Configs);

        DaysCycleService.Initialize(Game, TickService);

        EnemyProgressService.Initialize(Game);

        EnemySpawnerService.Initialize(Game, TickService, EnemyProgressService);

        EnemyControllerService.Initialize(Game, TickService);

        EnemyMarkingService.Initialize(Game);

        BestiaryTrackerService.Initialize(Game);

        WeaponsService.Initialize(Game, TickService);

        ProjectileService.Initialize(Game);

        DecisionsService.Initialize(Game, TickService, PathService);

        RepairService.Initialize(Game);

        ExpeditionEvents.OnExpeditionStart?.Invoke();
        Expedition.ExpeditionStatus = ExpeditionStatus.Running;
    }
}