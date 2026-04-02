using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static GameHelper;

public class EnemySpawnerService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private ExpeditionState Expedition;
    private DataState DataState;
    private EnemyProgressService ProgressService;

    public Dictionary<string, double> EnemyWeights = new();

    int tickCounter = 0;

    public void Initialize(ExpeditionState expeditionState, TickService Tick, DataState db, EnemyProgressService progress)
    {
        Expedition = expeditionState;

        TickService = Tick;

        ProgressService = progress;

        DataState = db;

        TickService.Subscribe(this);

        foreach (var enemy in DataState.enemies)
        {
            EnemyWeights.Add(enemy.Key, enemy.Value.Rarity);
        }
    }

    public void OnTick(float dt)
    {
        tickCounter++;

        if (tickCounter >= Expedition.BaseTicksPerSpawn)
        {
            tickCounter = 0;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        float spawn = Random.Range(0, 100);

        if (spawn < Expedition.BaseSpawnChance)
        {
            IncreaseChance();

            return;
        }

        var validEnemies = SpawnChance();

        if (validEnemies.Count == 0)
        {
            return;
        }

        float totalWeight = 0;

        foreach (var enemy in validEnemies)
        {
            totalWeight += (float)EnemyWeights[enemy.Key];
        }

        float roll = Random.Range(0, totalWeight);
        EnemyInstance chosen = null;

        foreach (var enemy in validEnemies)
        {
            roll -= (float)EnemyWeights[enemy.Key];

            if (roll <= 0)
            {
                chosen = enemy.Value;
                break;
            }
        }

        if (chosen == null)
        {
            return;
        }

        EnemyInstance instance = new EnemyInstance(chosen);

        instance.Distance *= Expedition.BaseSpawnDistance;

        ProgressService.ApplyProgression(instance);

        Expedition.ActiveEnemies.Add(instance);

        CombatEvents.OnEnemySpawn?.Invoke(instance);

        Debug.Log($"{instance.Name} Spawnado. Vida: {instance.Life}. Dano: {instance.Damage}");
    }

    Dictionary<string, EnemyInstance> SpawnChance()
    {
        Dictionary<string, EnemyInstance> validEnemies = new();

        foreach (var enemy in DataState.enemies)
        {
            if (enemy.Value.Rarity > 0)
            {
                if (Expedition.IsDay && enemy.Value.DayEnemy)
                {
                    // Checar também região/tipo...
                    validEnemies.Add(enemy.Key, enemy.Value);
                }
                else if (!Expedition.IsDay && !enemy.Value.DayEnemy)
                {
                    validEnemies.Add(enemy.Key, enemy.Value);
                }
            }
        }

        return validEnemies;
    }

    void IncreaseChance()
    {
        var keys = new List<string>(EnemyWeights.Keys);

        foreach (var key in keys)
        {
            if (EnemyWeights[key] > 0)
            {
                var oldValue = EnemyWeights[key];
                EnemyWeights[key] = EnemyWeights[key] + ((1f / EnemyWeights[key]) * 0.1f);
            }
        }
    }
}