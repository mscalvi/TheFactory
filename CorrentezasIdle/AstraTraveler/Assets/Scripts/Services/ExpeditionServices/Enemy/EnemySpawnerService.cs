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

    int tickCounter = 0;
    double accumulatedBudget = 0;
    int spawnTickCounter = 0;
    Queue<EnemyInstance> spawnQueue = new();

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
        tickCounter++;

        if (tickCounter >= Expedition.ActualTicksPerSpawn)
        {
            tickCounter = 0;
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

        instance.Angle = SpawnAngle(30, 330);

        ProgressService.ApplyProgression(instance);

        Expedition.ActiveEnemies.Add(instance);

        ExpeditionEvents.OnEnemySpawn?.Invoke(instance);

        if (instance.UnlockStatus == UnlockHelper.UnlockStatus.Unknow)
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

        double cheapest = GetCheapestCost(validEnemies);

        while (budget >= cheapest)
        {
            var chosen = ChooseEnemy(validEnemies);

            if (chosen == null)
                break;

            double cost = chosen.Cost;

            if (budget < cost)
                break;

            budget -= cost;

            spawnQueue.Enqueue(new EnemyInstance(chosen));
        }
    }

    double GenerateWaveBudget()
    {
        double baseBud = Expedition.ActualSpawnBudget;
        double growth = Expedition.ActualSpawnBudgetGrowth;

        double actualBud = baseBud * (1 + Expedition.DayCounter * growth);

        return actualBud;
    }
        
    void SpendBudget(double budget)
    {
        var validEnemies = SpawnChance();

        int maxEnemiesPerWave = 20;
        int spawned = 0;

        if (validEnemies.Count == 0)
            return;

        double cheapest = GetCheapestCost(validEnemies);

        while (budget >= cheapest && spawned < maxEnemiesPerWave)
        {
            var chosen = ChooseEnemy(validEnemies);

            if (chosen == null)
                break;

            double cost = chosen.Cost;

            if (budget < cost)
                break;

            budget -= cost;

            spawned++;

            SpawnInstance(chosen);
        }
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

    double GetCheapestCost(Dictionary<string, EnemyInstance> enemies)
    {
        double min = double.MaxValue;

        foreach (var e in enemies)
        {
            if (e.Value.Cost < min)
                min = e.Value.Cost;
        }

        return min;
    }

    void SpawnInstance(EnemyInstance chosen)
    {
        EnemyInstance instance = new EnemyInstance(chosen);

        instance.Angle = SpawnAngle(30, 330);

        ProgressService.ApplyProgression(instance);

        Expedition.ActiveEnemies.Add(instance);

        ExpeditionEvents.OnEnemySpawn?.Invoke(instance);

        if (instance.UnlockStatus == UnlockHelper.UnlockStatus.Unknow)
            EnemyKnow(chosen);
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
            if (Expedition.IsDay && enemy.Value.DayEnemy)
                validEnemies.Add(enemy.Key, enemy.Value);
            else if (!Expedition.IsDay && !enemy.Value.DayEnemy)
                validEnemies.Add(enemy.Key, enemy.Value);
        }

        return validEnemies;
    }

    void CheckSpecialEvent()
    {
        if (accumulatedBudget >= Expedition.ActualBossThreshold)
        {
            Debug.Log("Boss disparado!");

            accumulatedBudget = 0;
        }
    }

    private void EnemyKnow(EnemyInstance enemy)
    {
        if (enemy == null)
            return;

        if (enemy.UnlockStatus != UnlockHelper.UnlockStatus.Unknow)
            return;

        enemy.UnlockStatus = UnlockHelper.UnlockStatus.Available;


        Debug.Log($"{enemy.Name}: Inimigo Avistado!");

        GameEvents.NewEnemySeen?.Invoke(enemy);
    }
}