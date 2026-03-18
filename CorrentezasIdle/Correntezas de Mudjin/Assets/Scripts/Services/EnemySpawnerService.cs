using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static GameHelper;

public class EnemySpawnerService : MonoBehaviour, ITickable
{
    [SerializeField] TickService tick;
    [SerializeField] GameDatabase database;
    [SerializeField] ExpeditionService expedition;
    [SerializeField] DaysCycleService dayCycle;

    EnemyModel[] AllEnemies;
    public Dictionary<string, float> EnemyWeights = new();

    int SpawnTimer = 0;

    void Start()
    {
        tick.Subscribe(this);

        AllEnemies = database.enemies;

        foreach (var enemy in AllEnemies)
        {
            EnemyWeights[enemy.Id] = (float)enemy.Rarity;
        }

        Debug.Log("Spawn On.");
    }

    public void OnTick(float dt)
    {
        if (expedition.State != GameState.Running)
            return;

        SpawnTimer++;

        if (SpawnTimer >= expedition.BaseSpawnTimer)
        {
            SpawnTimer = 0;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        float spawn = Random.Range(0, 100);

        Debug.Log($"{spawn}");

        if (spawn < expedition.BaseSpawnChance)
        {
            IncreaseChance();
            return;
        }

        var validEnemies = SpawnChance();

        if (validEnemies.Count == 0)
        {
            Debug.Log("Nenhum inimigo válido para spawn!");
            return;
        }

        float totalWeight = 0;

        foreach (var enemy in validEnemies)
        {
            totalWeight += EnemyWeights[enemy.Id];
        }

        float roll = Random.Range(0, totalWeight);
        EnemyModel chosen = null;

        foreach (var enemy in validEnemies)
        {
            roll -= EnemyWeights[enemy.Id];

            if (roll <= 0)
            {
                chosen = enemy;
                break;
            }
        }

        foreach (var enemy in validEnemies)
        {
            totalWeight += EnemyWeights[enemy.Id];
        }

        expedition.ActiveEnemies.Add(new EnemyInstance(chosen, expedition.BaseSpawnDistance));

        Debug.Log($"Um {chosen.Name} selvagem apareceu.");
    }

    List<EnemyModel> SpawnChance()
    {
        List<EnemyModel> validEnemies = new();

        foreach (var enemy in AllEnemies)
        {
            if (enemy.Rarity > 0)
            {
                if (expedition.IsDay && enemy.DayEnemy)
                {
                    // Checar também região/tipo...
                    validEnemies.Add(enemy);
                }
                else if (!expedition.IsDay && !enemy.DayEnemy)
                {
                    validEnemies.Add(enemy);
                }
            }
        }

        return validEnemies;
    }

    void IncreaseChance()
    {
        Debug.Log("Dificuldade aumentada.");

        foreach (var key in new List<string>(EnemyWeights.Keys))
        {
            float value = EnemyWeights[key];

            if (value > 0)
            {
                Debug.Log($"Inimigo {key} chance anterior {EnemyWeights[key]}.");
                EnemyWeights[key] = value + ((1f / value) * 0.1f);
                Debug.Log($"Inimigo {key} nova chance {EnemyWeights[key]}.");
            }
        }
    }
}