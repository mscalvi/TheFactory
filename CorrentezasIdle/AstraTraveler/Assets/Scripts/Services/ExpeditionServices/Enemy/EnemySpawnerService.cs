using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private GameState GameState;
    private ExpeditionState Expedition;
    private DataState DataState;
    private EnemyProgressService ProgressService;

    public Dictionary<string, double> EnemyWeights = new();

    double accumulatedBudget = 0;
    int spawnTickCounter = 0;
    float spawnTimer = 4f;

    Queue<EnemyRuntime> spawnQueue = new();

    public void Initialize(GameState gameState, TickService Tick, EnemyProgressService progress)
    {
        GameState = gameState;
        Expedition = GameState.ExpeditionState;
        DataState = GameState.DataState;

        TickService = Tick;
        ProgressService = progress;

        TickService.Subscribe(this);

        foreach (var enemy in DataState.enemies)
        {
            EnemyWeights.Add(enemy.Key, enemy.Value.Rarity);
        }
    }

    public void OnTick(float dt)
    {
        spawnTimer += dt;

        if (spawnTimer >= Expedition.ActualSpawnInterval)
        {
            spawnTimer = 0;
            ProcessWave();
        }

        ProcessSpawnQueue();
    }

    void ProcessWave()
    {
        double waveBudget = GenerateWaveBudget();

        float roll = Random.Range(0f, 100f);

        if (roll < Expedition.ActualSpawnChance)
        {
            accumulatedBudget += waveBudget;
            ExpeditionEvents.NoWaveSpawn?.Invoke();
            Debug.Log("Sem Wave! Se Prepare!");
            return;
        }

        double totalBudget = waveBudget + accumulatedBudget;
        accumulatedBudget = 0;

        FillSpawnQueue(totalBudget);
    }

    void ProcessSpawnQueue()
    {
        if (spawnQueue.Count == 0)
            return;

        spawnTickCounter++;

        if (spawnTickCounter < GameState.ExpeditionState.ticksBetweenSpawns)
            return;

        spawnTickCounter = 0;

        var instance = spawnQueue.Dequeue();

        ProgressService.ApplyProgression(instance);

        instance.Angle = SpawnAngle(30, 330);

        Expedition.ActiveEnemies.Add(instance);

        ExpeditionEvents.OnEnemySpawn?.Invoke(instance);

        //Debug.Log($"Spawn: {instance.NamePT} \n-Vida: {instance.ActualLife} -Dano: {instance.Damage} -Speed: {instance.Speed}");

        if (instance.Known == false)
        {
            foreach (var enemy in DataState.enemies)
            {
                if (enemy.Key == instance.Id)
                {
                    EnemyKnow(enemy.Value);
                }
            }
        }
    }

    void FillSpawnQueue(double budget)
    {
        var validEnemies = SpawnChance();

        if (validEnemies.Count == 0)
            return;

        double cheapest = double.MaxValue;

        foreach (var e in validEnemies)
        {
            if (e.Value.SpawnCost < cheapest)
                cheapest = e.Value.SpawnCost;
        }

        while (budget >= cheapest)
        {
            var chosen = ChooseEnemy(validEnemies);

            if (chosen == null)
                break;

            double cost = chosen.SpawnCost;

            if (budget < cost)
                break;

            budget -= cost;

            spawnQueue.Enqueue(new EnemyRuntime(chosen));
        }
    }

    double GenerateWaveBudget()
    {
        double baseBud = Expedition.ActualSpawnBudget;
        double growth = Expedition.ActualSpawnBudgetGrowth;

        double actualBud = baseBud * (Expedition.DayCounter * growth);

        return actualBud;
    }

    EnemyInstance ChooseEnemy(Dictionary<string, EnemyInstance> validEnemies)
    {
        float totalWeight = 0;

        foreach (var enemy in validEnemies)
            totalWeight += (float)EnemyWeights[enemy.Key];

        float roll = Random.Range(0, totalWeight);

        foreach (var enemy in validEnemies)
        {
            roll -= (float)EnemyWeights[enemy.Key];

            if (roll <= 0)
                return enemy.Value;
        }

        return null;
    }

    double SpawnAngle(double min, double max)
    {
        min %= 360;
        max %= 360;

        if (min <= max)
        {
            return Random.Range((float)min, (float)max);
        }
        else
        {
            double range1 = 360 - min;
            double range2 = max;

            double total = range1 + range2;
            double roll = Random.Range(0f, (float)total);

            if (roll < range1)
                return min + roll;
            else
                return roll - range1;
        }
    }

    Dictionary<string, EnemyInstance> SpawnChance()
    {
        Dictionary<string, EnemyInstance> validEnemies = new();

        foreach (var enemy in DataState.enemies)
        {            
            if (enemy.Value.UnlockStatus != UnlockHelper.UnlockStatus.Available)
                continue;

            if (Expedition.IsDay && enemy.Value.DayEnemy)
                validEnemies.Add(enemy.Key, enemy.Value);
            else if (!Expedition.IsDay && !enemy.Value.DayEnemy)
                validEnemies.Add(enemy.Key, enemy.Value);
        }

        return validEnemies;
    }

    private void EnemyKnow(EnemyInstance enemy)
    {
        if (enemy == null)
            return;

        if (enemy.Known)
            return;

        enemy.Known = true;

        GameEvents.NewEnemySeen?.Invoke(enemy);
    }
}