using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionState
{
    // Day/Night Service
    public int BaseTicksPerPhase = 150;
    public bool IsDay { get; set; } = true;
    public int DestinationDayCounter = 1;
    public int DayCounter = 1;

    // Destination
    public DestinationInstance OldDestination;
    public DestinationInstance CurrentDestination;
    public DestinationInstance NewDestination;
    public PathInstance CurrentPath;
    public int DestinationArrival = 0;
    public GameHelper.ExpeditionStatus ExpeditionStatus;

    // Controlador de Inimigos
    public double BaseSpawnDistance = 1.5;
    public double BaseSpawnChance = 1;
    public int BaseTicksPerSpawn = 15;
    public double BaseSpawnBudget = 5;
    public double BaseSpawnBudgetGrowth = 1.05;
    public double BossThreshold = 200;
    public EnemyHelper.EnemyStage EnemySpawnStage = EnemyHelper.EnemyStage.Early;
    public List<EnemyInstance> ActiveEnemies = new();

    // Currency
    public Dictionary<CurrencyHelper.CurrencyType, CurrencyInstance> ExpeditionCurrency;
    public double BaseDayReward = 1;
    public double BaseNightReward = 1;

    // Upgrades
    public Dictionary<string, UpgradeInstance> ExpeditionUpgrades;

    // Ingredients
    public int MaxMarkedEnemies = 1;
    public int MaxMarkedLoot = 1;
    public double NextLootChance = 0;
    public double NextLootDecay = 0;
    public Dictionary<IngredientHelper.IngredientRarity, float> RarityBaseWeights;
}
