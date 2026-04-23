using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private ExpeditionState Expedition;
    private DataState DataState;
    private EnemyProgressService ProgressService;

    public Dictionary<string, double> EnemyWeights = new();

    int tickCounter = 0;
    double accumulatedBudget = 0;

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
            ProcessWave();
        }
    }

    void ProcessWave()
    {
        double waveBudget = GenerateWaveBudget();

        float roll = Random.Range(0f, 100f);

        if (roll < Expedition.BaseSpawnChance)
        {
            accumulatedBudget += waveBudget;
            return;
        }

        double totalBudget = waveBudget + accumulatedBudget;
        accumulatedBudget = 0;

        SpendBudget(totalBudget);

        CheckSpecialEvent();
    }

    double GenerateWaveBudget()
    {
        double baseBud = Expedition.BaseSpawnBudget;
        double growth = Expedition.BaseSpawnBudgetGrowth;

        double actualBud = baseBud * System.Math.Pow(growth, Expedition.DayCounter);

        return actualBud;
    }
        
    void SpendBudget(double budget)
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

        instance.Distance *= Expedition.BaseSpawnDistance;
        instance.Angle = SpawnAngle(30, 330);

        ProgressService.ApplyProgression(instance);

        Expedition.ActiveEnemies.Add(instance);

        CombatEvents.OnEnemySpawn?.Invoke(instance);

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
            if (enemy.Value.BossEnemy)
                continue;

            if ((enemy.Value.PathType & Expedition.ActualPath.PathType) == 0)
                continue;

            if (enemy.Value.Stage > Expedition.EnemySpawnStage)
                continue;

            if (Expedition.IsDay && enemy.Value.DayEnemy)
                validEnemies.Add(enemy.Key, enemy.Value);
            else if (!Expedition.IsDay && !enemy.Value.DayEnemy)
                validEnemies.Add(enemy.Key, enemy.Value);
        }

        return validEnemies;
    }

    void CheckSpecialEvent()
    {
        if (accumulatedBudget >= Expedition.BossThreshold)
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

        CompanyEvents.NewEnemySeen?.Invoke(enemy);
    }
}