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
    [SerializeField] ExpeditionPurchaseService ExpeditionPurchaseService;
    [SerializeField] ExpeditionUiService ExpeditionUiService;

    // Expedition
    [SerializeField] DaysCycleService DaysCycleService;
    [SerializeField] EnemyProgressService EnemyProgressService;
    [SerializeField] EnemySpawnerService EnemySpawnerService;
    [SerializeField] EnemyControllerService EnemyControllerService;
    [SerializeField] ExpeditionPricingService ExpeditionPricingService;

    // Ship
    [SerializeField] ShipControlService ShipControlService;
    [SerializeField] UnnamedTripulationService UnnamedTripulationService;
    [SerializeField] WeaponRoomsService WeaponRoomsService;

    // Ambos
    [SerializeField] ExpeditionService ExpeditionService;
    [SerializeField] CombatService CombatService;
    [SerializeField] DecisionsService DecisionsService;
    [SerializeField] ExpeditionUpgradeService ExpeditionUpgradeService;

    // Outros Mecanismos Úteis
    [SerializeField] DecisionsPopUpDesigner DecisionsPanel;
    [SerializeField] FinalPopUpDesigner FinalPanel;
    [SerializeField] EndExpeditionService EndExpeditionService;

    private void Awake()
    {
        var Game = GameController.Instance.GameState;

        if (Game == null)
        {
            Debug.LogError("Game NULL!");
            return;
        }

        StartExpeditionService.Initialize(Game);

        var Expedition = GameController.Instance.GameState.ExpeditionState;

        if (Expedition == null)
        {
            Debug.LogError("ExpeditionState NULL!");
            return;
        }

        Expedition.ExpeditionStatus = ExpeditionStatus.Paused;
        Expedition.ExpeditionUpgrades = new Dictionary<string, UpgradeInstance>();

        var Ship = GameController.Instance.GameState.ShipState;

        if (Ship == null)
        {
            Debug.LogError("ShipState NULL -> Criando Nova");
            return;
        }

        var db = GameController.Instance.GameState.DataState;

        // Base
        TickService.Initialize();

        CurrencyService.Initialize(Expedition);

        ExpeditionPurchaseService.Initialize(Expedition, db, CurrencyService);

        ExpeditionUiService.Initialize(Expedition, Ship, Game, db, ExpeditionPurchaseService);

        // Expedition
        DaysCycleService.Initialize(Game, Expedition, TickService, ExpeditionUiService);

        EnemyProgressService.Initialize(Expedition);

        EnemySpawnerService.Initialize(Expedition, TickService, db, EnemyProgressService);

        EnemyControllerService.Initialize(Expedition, TickService);

        ExpeditionPricingService.Initialize(Expedition, db);

        // Ship
        ShipControlService.Initialize(Ship, Game, TickService);

        UnnamedTripulationService.Initialize(Ship, TickService);

        // Ambos
        CombatService.Initialize(Expedition, Ship, TickService, ExpeditionUiService);

        ExpeditionService.Initialize(Expedition, Ship, Game, TickService, ExpeditionUiService);

        WeaponRoomsService.Initialize(Expedition, Ship, TickService, CombatService);

        ExpeditionUpgradeService.Initialize(Expedition, db, Ship);

        // Outros
        DecisionsService.Initialize(Expedition, Ship, Game, DecisionsPanel, TickService, db, ExpeditionService, FinalPanel);

        EndExpeditionService.Initialize(Expedition, Game, DecisionsService);

        // Events
        RunEvents.OnExpeditionStart?.Invoke();
    }
}