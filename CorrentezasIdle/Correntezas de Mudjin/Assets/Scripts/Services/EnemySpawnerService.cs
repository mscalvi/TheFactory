using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static GameHelper;

public class EnemySpawnerService : MonoBehaviour, ITickable
{
    private GameDatabase DataBase;
    private TickService TickService;
    private ExpeditionState Expedition;

    [SerializeField] TextMeshProUGUI SpawnText;

    EnemyModel[] AllEnemies;
    public Dictionary<string, float> EnemyWeights = new();

    int tickCounter = 0;

    public void Initialize(ExpeditionState expeditionState, TickService Tick, GameDatabase db)
    {
        Expedition = expeditionState;

        TickService = Tick;

        DataBase = db;

        TickService.Subscribe(this);

        Debug.Log("EnemySpawnService On");

        AllEnemies = DataBase.enemies;

        foreach (var enemy in AllEnemies)
        {
            EnemyWeights[enemy.Id] = (float)enemy.Rarity;
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

        Debug.Log($"{spawn}");

        if (spawn < Expedition.BaseSpawnChance)
        {
            IncreaseChance();
            Debug.Log($"Sem Spawn. Nível aumentado.");

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

        if (chosen == null)
        {
            Debug.LogError("Nenhum inimigo selecionado!");
            return;
        }

        Expedition.ActiveEnemies.Add(new EnemyInstance(chosen, Expedition.BaseSpawnDistance));
        SpawnText.text = "New Arrival: " + chosen.Name;

        Debug.Log($"Spawn.");
    }

    List<EnemyModel> SpawnChance()
    {
        List<EnemyModel> validEnemies = new();

        foreach (var enemy in AllEnemies)
        {
            if (enemy.Rarity > 0)
            {
                if (Expedition.IsDay && enemy.DayEnemy)
                {
                    // Checar também região/tipo...
                    validEnemies.Add(enemy);
                }
                else if (!Expedition.IsDay && !enemy.DayEnemy)
                {
                    validEnemies.Add(enemy);
                }
            }
        }

        return validEnemies;
    }

    void IncreaseChance()
    {
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