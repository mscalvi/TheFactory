using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static GameHelper;

public class ExpeditionController : MonoBehaviour
{
    [SerializeField] TickService TickService;
    [SerializeField] StartExpeditionService StartExpeditionService;

    // Expedition
    [SerializeField] DaysCycleService DaysCycleService;
    [SerializeField] EnemySpawnerService EnemySpawnerService;
    [SerializeField] EnemyControllerService EnemyControllerService;

    // Ship
    [SerializeField] ShipControlService ShipControlService;
    [SerializeField] UnnamedTripulationService UnnamedTripulationService;
    [SerializeField] WeaponRoomsService WeaponRoomsService;

    // Ambos
    [SerializeField] ExpeditionService ExpeditionService;
    [SerializeField] CombatService CombatService;
    [SerializeField] DecisionsService DecisionsService;

    // Outros Mecanismos Úteis
    [SerializeField] DecisionsPopUpDesigner DecisionsPanel;
    [SerializeField] ExpeditionUiService ExpeditionUiService;

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

        var Ship = GameController.Instance.GameState.ShipState;

        if (Ship == null)
        {
            Debug.LogError("ShipState NULL -> Criando Nova");
            return;
        }

        var db = GameController.Instance.Database;

        // Base

        TickService.Initialize();

        ExpeditionUiService.Initialize(Expedition, Ship, Game);

        // Expedition
        DaysCycleService.Initialize(Expedition, TickService, ExpeditionUiService);

        EnemySpawnerService.Initialize(Expedition, TickService, db);

        EnemyControllerService.Initialize(Expedition, TickService, ExpeditionUiService);

        // Ship
        ShipControlService.Initialize(Ship, Game, TickService);

        UnnamedTripulationService.Initialize(Ship, TickService);

        // Ambos

        CombatService.Initialize(Expedition, Ship, TickService, ExpeditionUiService);

        ExpeditionService.Initialize(Expedition, Ship, TickService, ExpeditionUiService);

        WeaponRoomsService.Initialize(Expedition, Ship, TickService, CombatService);

        DecisionsService.Initialize(Expedition, Ship, Game, DecisionsPanel, TickService, db, ExpeditionService);
    }
}