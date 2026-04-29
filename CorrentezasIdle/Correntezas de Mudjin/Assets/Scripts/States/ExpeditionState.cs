using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionState
{
    // Valores Base
    public int StartDay = 1;
    public double BaseSpawnDistance = 1.5;
    public double BaseSpawnChance = 1;
    public int BaseTicksPerSpawn = 15;
    public double BaseSpawnBudget = 5;
    public double BaseSpawnBudgetGrowth = 1.05;
    public double BaseBossThreshold = 200;
    public EnemyHelper.EnemyStage BaseEnemySpawnStage = EnemyHelper.EnemyStage.Early;
    public double BaseDayReward = 1;
    public double BaseNightReward = 1;
    public int BaseMaxMarkedEnemies = 1;
    public int BaseMaxMarkedLoot = 1;
    public double BaseNextLootChance = 0;
    public double BaseNextLootDecay = 0;


    // Day/Night Service
    public int BaseTicksPerPhase = 30; // Padrão: 150
    public bool IsDay { get; set; } = true;
    public int DestinationDayCounter = 1;
    public int DayCounter = 1;    

    // Destination
    public DestinationInstance OldDestination;      // De onde veio
    public DestinationInstance NewDestination;      // Para onde vai

    public DestinationInstance ActualDestination;  // Onde está parado
    public PathInstance ActualPath;                // Onde está durante viagem

    public int DestinationArrival = 0;
    public GameHelper.ExpeditionStatus ExpeditionStatus;

    // Controlador de Inimigos
    public bool DamageTaken = false;
    public double ActualSpawnDistance = 1.5;
    public double ActualSpawnChance = 1;
    public int ActualTicksPerSpawn = 15;
    public double ActualSpawnBudget = 5;
    public double ActualSpawnBudgetGrowth = 1.05;
    public double ActualBossThreshold = 200;
    public List<EnemyInstance> ActiveEnemies = new();
    public EnemyHelper.EnemyStage ActualEnemySpawnStage = EnemyHelper.EnemyStage.Early;

    // Currency
    public double ActualDayReward = 1;
    public double ActualNightReward = 1;

    // Ingredients
    public int ActualMaxMarkedEnemies = 1;
    public int ActualMaxMarkedLoot = 1;
    public double ActualNextLootChance = 0;
    public double ActualNextLootDecay = 0;
    public Dictionary<IngredientHelper.IngredientRarity, float> IngredientRarityBaseWeights;
}
