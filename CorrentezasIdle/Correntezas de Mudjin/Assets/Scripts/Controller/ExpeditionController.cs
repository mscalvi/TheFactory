using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameHelper;

public class ExpeditionController : MonoBehaviour
{
    [SerializeField] TickService TickService;
    [SerializeField] DaysCycleService DaysCycleService;
    [SerializeField] EnemySpawnerService EnemySpawnerService;
    [SerializeField] EnemyControllerService EnemyControllerService;

    private void Awake()
    {
        var Expedition = GameController.Instance.GameState.Expedition;

        if (Expedition == null)
        {
            Debug.LogError("ExpeditionState NULL!");
            return;
        }

        var ExpeditionConfiguration = Expedition.CurrentExpedition;

        Debug.Log($"Navio Ativo: {ExpeditionConfiguration.Ship}");
        Debug.Log($"Navio Ativo: {ExpeditionConfiguration.Rooms[0].Tripulation}");
        Debug.Log($"Navio Ativo: {ExpeditionConfiguration.Rooms[0].Weapon}");
        Debug.Log($"Navio Ativo: {ExpeditionConfiguration.Rooms[0].Ammo}");

    }

    private void Start()
    {
        var expedition = GameController.Instance.GameState.Expedition;
        var db = GameController.Instance.Database;

        if (expedition == null)
        {
            Debug.LogError("ExpeditionState NULL!");
            return;
        }

        DaysCycleService.Initialize(expedition, TickService);

        EnemySpawnerService.Initialize(expedition, TickService, db);

        EnemyControllerService.Initialize(expedition, TickService);
    }
}