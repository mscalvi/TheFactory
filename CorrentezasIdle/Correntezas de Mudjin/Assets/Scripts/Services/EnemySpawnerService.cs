using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawnerService : MonoBehaviour, ITickable
{
    TickService tick;

    public GameDatabase database;
    public DaysCycleService dayCycle;

    [SerializeField] TextMeshProUGUI EnemyFishText;
    [SerializeField] TextMeshProUGUI EnemyPirateText;
    [SerializeField] TextMeshProUGUI EnemySnakeText;
    [SerializeField] TextMeshProUGUI EnemySharkText;

    int SpawnTimer = 0;
    // Muda de acordo com a área e a dificuldade?
    int TimePerSpawn = 10;

    void Start()
    {
        tick = FindObjectOfType<TickService>();
        tick.Subscribe(this);

        EnemyFishText.text = "Fish: 0";
        EnemyPirateText.text = "Pirate: 0";
        EnemySnakeText.text = "Snake: 0";
        EnemySharkText.text = "Shark: 0";
    }

    void OnDestroy()
    {
        tick?.Unsubscribe(this);
    }

    public void OnTick(float dt)
    {
        SpawnTimer++;

        if (SpawnTimer >= TimePerSpawn)
        {
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
            if (dayCycle.IsDay && enemy.DayEnemy)
                validEnemies.Add(enemy);

            if (!dayCycle.IsDay && !enemy.DayEnemy)
                validEnemies.Add(enemy);
        }

        // Função complexa de probabilidade por área?
        EnemyModel chosen = validEnemies[Random.Range(0, validEnemies.Count)];

        Debug.Log(chosen.Name);

        Dictionary<string, int> ActiveEnemies = new();

        if (!ActiveEnemies.ContainsKey(chosen.Id))
        {
            ActiveEnemies[chosen.Id] = 0;
        }

        // A lista é limpa toda vez, precisa mudar pra uma propriedade, talvez no próprio model?
        ActiveEnemies[chosen.Id]++;
        Debug.Log(ActiveEnemies[chosen.Id]);

        switch (chosen.Id)
        {
            case "e001":
                EnemyFishText.text = "Fishs: " + ActiveEnemies[chosen.Id];
                break;

            case "e002":
                EnemyPirateText.text = "Pirates: " + ActiveEnemies[chosen.Id];
                break;

            case "e003":
                EnemySnakeText.text = "Snakes: " + ActiveEnemies[chosen.Id];
                break;

            case "e004":
                EnemySharkText.text = "Sharks: " + ActiveEnemies[chosen.Id];
                break;
        }
    }
}