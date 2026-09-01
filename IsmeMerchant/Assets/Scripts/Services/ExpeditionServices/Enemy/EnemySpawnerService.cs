using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawnerService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private GameState GameState;
    private EnemyProgressService ProgressService;

    private Dictionary<string, double> ValidEnemies = new();

    private float spawnTimer = 0;
    private float SpawnInterval = 20f;

    private Queue<EnemyRuntime> spawnQueue = new();


    public void Initialize(
        GameState gameState,
        TickService Tick,
        EnemyProgressService progress)
    {
        GameState = gameState;

        TickService = Tick;
        ProgressService = progress;

        TickService.Subscribe(this);

        ProcessWave();
    }

    public void OnTick(float dt)
    {
        spawnTimer += dt;

        if (spawnTimer >= SpawnInterval)
        {
            spawnTimer = 0;
            ProcessSpawnQueue();
        }
    }

    // WAVE
    private void ProcessWave()
    {
        ValidEnemies.Clear();

        SpawnInterval = GameState.ExpeditionState.PhaseDuration / GameState.ExpeditionState.ActualWaveSize;

        float bossRoll = Random.Range(0f, 100f);

        if (bossRoll < GameState.ExpeditionState.ActualBossChance)
        {
            SpawnBoss();
        }

        BuildValidEnemies();

        if (ValidEnemies.Count == 0)
        {
            Debug.Log("Nenhum inimigo válido para spawn.");
            return;
        }

        FillSpawnQueue();
    }

    private void BuildValidEnemies()
    {
        foreach (var enemy in GameState.DataState.enemies)
        {
            EnemyInstance data = enemy.Value;

            if (data.BossEnemy)
                continue;

            if (data.UnlockStatus != UnlockHelper.UnlockStatus.Available)
                continue;

            if ((data.PathTypes & GameState.ExpeditionState.ActualPath.Type) == 0)
                continue;

            if ((data.PathEnvironments & GameState.ExpeditionState.ActualPath.Environment) == 0)
                continue;

            if (GameState.ExpeditionState.IsDay)
            {
                if (!data.DayEnemy)
                    continue;
            }
            else
            {
                if (data.DayEnemy)
                    continue;
            }

            ValidEnemies.Add(
                enemy.Key,
                data.Rarity
            );
        }
    }

    // SPAWN QUEUE
    private void FillSpawnQueue()
    {
        int waveSize = GameState.ExpeditionState.ActualWaveSize;

        for (int i = 0; i < waveSize; i++)
        {
            EnemyInstance chosen = ChooseEnemy();

            if (chosen == null)
                continue;

            EnemyRuntime runtime = new EnemyRuntime(chosen);

            spawnQueue.Enqueue(runtime);
        }
    }

    private void ProcessSpawnQueue()
    {
        if (spawnQueue.Count == 0)
            return;

        EnemyRuntime instance = spawnQueue.Dequeue();

        SpawnEnemy(instance);

        if (!instance.Known)
        {
            EnemyInstance enemy = GameState.DataState.enemies[instance.Id];

            EnemyKnow(enemy);
        }
    }

    // ESCOLHA DO INIMIGO
    private EnemyInstance ChooseEnemy()
    {
        double totalRarity = 0;

        foreach (var enemy in ValidEnemies)
        {
            totalRarity += enemy.Value;
        }

        if (totalRarity <= 0)
            return null;

        float roll = Random.Range(
            0f,
            (float)totalRarity
        );

        foreach (var enemy in ValidEnemies)
        {
            roll -= (float)enemy.Value;

            if (roll <= 0)
            {
                return GameState.DataState.enemies[enemy.Key];
            }
        }

        return null;
    }

    // SPAWN
    private void SpawnEnemy(EnemyRuntime instance)
    {
        ProgressService.ApplyProgression(instance);

        instance.Angle = SpawnAngle(30, 330);

        GameState.ExpeditionState.ActiveEnemies.Add(instance);

        ExpeditionEvents.OnEnemySpawn?.Invoke(instance);

    }

    // ANGLE
    private double SpawnAngle(double min, double max)
    {
        min %= 360;
        max %= 360;

        if (min <= max)
        {
            return Random.Range(
                (float)min,
                (float)max
            );
        }

        double range1 = 360 - min;
        double range2 = max;

        double total = range1 + range2;

        double roll = Random.Range(
            0f,
            (float)total
        );

        if (roll < range1)
            return min + roll;

        return roll - range1;
    }

    // BESTIARY
    private void EnemyKnow(EnemyInstance enemy)
    {
        if (enemy == null)
            return;

        if (enemy.Known)
            return;

        enemy.Known = true;

        GameEvents.NewEnemySeen?.Invoke(enemy);
    }

    // BOSS
    private void SpawnBoss()
    {
        List<EnemyInstance> validBosses = new();

        if (GameState.ExpeditionState.DayCounter < 10)
            return;

        foreach (var enemy in GameState.DataState.enemies.Values)
        {
            if (!enemy.BossEnemy)
                continue;

            if ((enemy.PathTypes &
                 GameState.ExpeditionState.ActualPath.Type) == 0)
                continue;

            if ((enemy.PathEnvironments &
                 GameState.ExpeditionState.ActualPath.Environment) == 0)
                continue;

            if (enemy.UnlockStatus !=
                UnlockHelper.UnlockStatus.Available)
                continue;

            //if (GameState.ExpeditionState.IsDay)
            //{
            //    if (!enemy.DayEnemy)
            //        continue;
            //}
            //else
            //{
            //    if (enemy.DayEnemy)
            //        continue;
            //}

            validBosses.Add(enemy);
        }

        if (validBosses.Count == 0)
            return;

        int randomIndex = Random.Range(
            0,
            validBosses.Count
        );

        EnemyInstance chosen = validBosses[randomIndex];

        EnemyRuntime chosenRuntime =
            new EnemyRuntime(chosen);

        SpawnEnemy(chosenRuntime);

        ExpeditionEvents.OnBossSpawn?.Invoke(
            chosenRuntime
        );
    }


    // Aumenta o Tamanho da Wave
    private void UpdateWaveSize()
    {
        GameState.ExpeditionState.ActualWaveSize++;
        GameState.ExpeditionState.ActualWaveSize++;

        spawnQueue.Clear();

        if (GameState.ExpeditionState.ActualWaveSize > GameState.ExpeditionState.MaxWaveSize)
            GameState.ExpeditionState.ActualWaveSize = GameState.ExpeditionState.MaxWaveSize; 
    
    }

    // EVENTOS
    private void OnEnable()
    {
        ExpeditionEvents.SpawnBoss += SpawnBoss;

        ExpeditionEvents.OnNightFinish += UpdateWaveSize;
        ExpeditionEvents.OnNightFinish += ProcessWave;
        ExpeditionEvents.OnDayFinish += ProcessWave;
    }

    private void OnDisable()
    {
        ExpeditionEvents.SpawnBoss -= SpawnBoss;

        ExpeditionEvents.OnNightFinish -= UpdateWaveSize;
        ExpeditionEvents.OnNightFinish -= ProcessWave;
        ExpeditionEvents.OnDayFinish -= ProcessWave;
    }
}