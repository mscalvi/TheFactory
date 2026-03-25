using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        var Ship = GameController.Instance.GameState.ShipState;

        if (Ship == null)
        {
            Debug.LogError("ShipState NULL -> Criando Nova");
            return;
        }

        var db = GameController.Instance.Database;

        // Expedition
        DaysCycleService.Initialize(Expedition, TickService);

        EnemySpawnerService.Initialize(Expedition, TickService, db);

        EnemyControllerService.Initialize(Expedition, TickService);

        // Ship
        ShipControlService.Initialize(Ship, Game, TickService);

        UnnamedTripulationService.Initialize(Ship, TickService);


        // Ambos
        CombatService.Initialize(Expedition, Ship, TickService);

        ExpeditionService.Initialize(Expedition, Ship, TickService);

        WeaponRoomsService.Initialize(Expedition, Ship, TickService, CombatService);
    }
}