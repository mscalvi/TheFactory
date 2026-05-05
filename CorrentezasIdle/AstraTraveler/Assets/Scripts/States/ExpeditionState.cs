using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionState
{
    // Valores Base
    public int StartDay = 1;

    public double StartSpawnChance = 1;
    public int StartTicksPerSpawn = 15;
    public double StartSpawnBudget = 5;
    public double StartSpawnBudgetGrowth = 1.05;
    public double StartBossThreshold = 200;
    public double StartDayReward = 1;
    public double StartNightReward = 1;
    public int StartMaxMarkedEnemies = 1;
    public int StartMaxMarkedLoot = 1;
    public double StartNextLootChance = 0;
    public double StartNextLootDecay = 0;
    public double StartExperienceKillBonus = 1;

    public double BaseSpawnChance = 1;
    public int BaseTicksPerSpawn = 15;
    public double BaseSpawnBudget = 5;
    public double BaseSpawnBudgetGrowth = 1.05;
    public double BaseBossThreshold = 200;
    public double BaseDayReward = 1;
    public double BaseNightReward = 1;
    public int BaseMaxMarkedEnemies = 1;
    public int BaseMaxMarkedLoot = 1;
    public double BaseNextLootChance = 0;
    public double BaseNextLootDecay = 0;
    public double BaseExperienceKillBonus = 1;

    // Main
    public GameHelper.ExpeditionStatus ExpeditionStatus;

    // Ship
    public ShipInstance Ship;
    public int Tripulation = 0;

    // Day/Night Service
    public int BaseTicksPerPhase = 150;             // Padrão: 150
    public bool IsDay { get; set; } = true;
    public int DayCounter = 1;    

    // Path
    public PathInstance ActualPath;                // Onde está durante viagem

    // Controlador de Inimigos
    public bool DamageTaken = false;
    public double ActualSpawnChance = 1;
    public int ActualTicksPerSpawn = 15;
    public double ActualSpawnBudget = 5;
    public double ActualSpawnBudgetGrowth = 1.05;
    public double ActualBossThreshold = 200;
    public List<EnemyInstance> ActiveEnemies = new();

    // Currency
    public double ActualExperienceKillBonus = 1;
    public double ActualDayReward = 1;
    public double ActualNightReward = 1;

    // Ingredients
    public int ActualMaxMarkedEnemies = 1;
    public int ActualMaxMarkedLoot = 1;
    public double ActualNextLootChance = 0;
    public double ActualNextLootDecay = 0;
    public Dictionary<IngredientHelper.IngredientRarity, float> IngredientRarityBaseWeights;
}
