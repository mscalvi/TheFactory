using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawnerService : MonoBehaviour, ITickable
{
    [SerializeField] TickService tick;

    public GameDatabase database;
    public ExpeditionService expedition;
    public DaysCycleService dayCycle;

    int SpawnTimer = 0;

    void Start()
    {
        tick.Subscribe(this);

        Debug.Log("Spawn On.");
    }

    public void OnTick(float dt)
    {
        SpawnTimer++;

        if (SpawnTimer >= expedition.BaseSpawnTimer)
        {
            Debug.Log("Spawn chamado.");
            SpawnTimer = 0;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        EnemyModel[] enemies = database.enemies;

        List<EnemyModel> validEnemies = new();

        foreach (var enemy in enemies)
        {
            if (expedition.IsDay && enemy.DayEnemy)
                validEnemies.Add(enemy);

            if (!expedition.IsDay && !enemy.DayEnemy)
                validEnemies.Add(enemy);
        }

        if (validEnemies.Count == 0)
        {
            Debug.LogWarning("Nenhum inimigo válido para spawn!");
            return;
        }

        EnemyModel chosen = validEnemies[Random.Range(0, validEnemies.Count)];

        expedition.ActiveEnemies.Add(new EnemyInstance(chosen));

        Debug.Log("Spawn: " + chosen.Name);
    }
}