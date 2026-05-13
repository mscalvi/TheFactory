using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionState
{

    // Main
    public ShipInstance Ship;
    public GameHelper.ExpeditionStatus ExpeditionStatus;

    public float PhaseDuration = 15f;
    public int ticksBetweenSpawns = 1;

    public bool IsDay { get; set; } = true;
    public int DayCounter = 1;
    public int StartDay = 1;
    public int NextDestination = 0;
    public int ActualDestination = 0;
    public int ReachedDestinations = 0;
    public PathHelper.PathTagSet LastPath = new PathHelper.PathTagSet();
    public PathHelper.PathTagSet ActualPath = new PathHelper.PathTagSet();

    public List<EnemyInstance> ActiveEnemies = new();
    public bool DamageTaken = false;


    // Valores Start
    public Dictionary<GameHelper.ItemRarity, float> StartIngredientRarityWeights;

    public double StartSpawnChance = 1;
    public float StartSpawnInterval = 10f;
    public double StartSpawnBudget = 3;
    public double StartSpawnBudgetGrowth = 0.69;
    public double StartBossThreshold = 200;

    public double StartExperienceKillBonus = 1;
    public double StartDayReward = 1;
    public double StartNightReward = 1;
    public double StartDestinationReward = 1;

    public int StartMaxMarkedEnemies = 1;
    public int StartMaxMarkedLoot = 1;
    public double StartNextLootChance = 0;
    public double StartNextLootDecay = 0;

    public int StartMinimalDestinationGap = 5;
    public int StartMaximalDestinationGap = 7;
    public int StartDestinationGapIncrease = 2;

    // Valores Base
    public Dictionary<GameHelper.ItemRarity, float> BaseIngredientRarityWeights;
    public double BaseSpawnChance = 1;
    public float BaseSpawnInterval = 10f;
    public double BaseSpawnBudget = 3;
    public double BaseSpawnBudgetGrowth = 0.69;
    public double BaseBossThreshold = 200;

    public double BaseExperienceKillBonus = 1;
    public double BaseDayReward = 1;
    public double BaseNightReward = 1;
    public double BaseDestinationReward = 1;

    public int BaseMaxMarkedEnemies = 1;
    public int BaseMaxMarkedLoot = 1;
    public double BaseNextLootChance = 0;
    public double BaseNextLootDecay = 0;

    public int BaseMinimalDestinationGap = 5;
    public int BaseMaximalDestinationGap = 7;
    public int BaseDestinationGapIncrease = 2;

    // Valores Atuais
    public Dictionary<GameHelper.ItemRarity, float> ActualIngredientRarityWeights;

    public double ActualSpawnChance = 1;
    public float ActualSpawnInterval = 10f;
    public double ActualSpawnBudget = 3;
    public double ActualSpawnBudgetGrowth = 0.69;
    public double ActualBossThreshold = 200;

    public double ActualExperienceKillBonus = 1;
    public double ActualDayReward = 1;
    public double ActualNightReward = 1;
    public double ActualDestinationReward = 1;

    public int ActualMaxMarkedEnemies = 1;
    public int ActualMaxMarkedLoot = 1;
    public double ActualNextLootChance = 0;
    public double ActualNextLootDecay = 0;

    public int ActualMinimalDestinationGap = 5;
    public int ActualMaximalDestinationGap = 7;
    public int ActualDestinationGapIncrease = 2;
}
