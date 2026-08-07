using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionState
{
    // Main
    public ShipInstance Ship;
    public GameHelper.ExpeditionStatus ExpeditionStatus;

    public int ExpeditionsDone = 0;

    public float phaseTimer = 0f;
    public float PhaseDuration = 20f;
    public int ticksBetweenSpawns = 5;
    public bool IsDay { get; set; } = true;
    public int DayCounter = 1;
    public int StartDay = 1;
    public int NextDestination = 0;
    public int ActualDestination = 0;
    public int ReachedDestinations = 0;
    public PathHelper.PathTagSet LastPath = new PathHelper.PathTagSet();
    public PathHelper.PathTagSet ActualPath = new PathHelper.PathTagSet();

    public List<EnemyRuntime> ActiveEnemies = new List<EnemyRuntime>();
    public bool DamageTaken = false;

    public List<AmmoInstance> ActiveAmmos;

    // Valores Start
    public Dictionary<GameHelper.ItemRarity, float> StartIngredientRarityWeights = new Dictionary<GameHelper.ItemRarity, float>();

    public double StartSpawnChance = 1;
    public float StartSpawnInterval = 5f;
    public double StartSpawnBudget = 4;
    public double StartSpawnBudgetGrowth = 0.55;
    public double StartBossThreshold = 200;

    public double StartStartExperience = 0;
    public double StartExperienceKillBonus = 1;
    public double StartDayReward = 1;
    public double StartNightReward = 1;
    public double StartDestinationReward = 1;

    public int StartMaxMarkedEnemies = 1;
    public int StartMaxMarkedLoot = 1;
    public double StartNextLootChance = 0.05;
    public double StartNextLootDecay = 0.01;
    public double StartClickDamage = 1;

    public int StartMinimalDestinationGap = 5;
    public int StartMaximalDestinationGap = 7;
    public int StartDestinationGapIncrease = 2;

    public int StartMaxTripulation = 1;

    // Valores Base
    public Dictionary<GameHelper.ItemRarity, float> BaseIngredientRarityWeights = new Dictionary<GameHelper.ItemRarity, float>();
    
    public double BaseSpawnChance = 1;
    public float BaseSpawnInterval = 5f;
    public double BaseSpawnBudget = 4;
    public double BaseSpawnBudgetGrowth = 0.55;
    public double BaseBossThreshold = 200;

    public double BaseStartExperience = 0;
    public double BaseExperienceKillBonus = 1;
    public double BaseDayReward = 1;
    public double BaseNightReward = 1;
    public double BaseDestinationReward = 1;

    public int BaseMaxMarkedEnemies = 1;
    public int BaseMaxMarkedLoot = 1;
    public double BaseNextLootChance = 0.05;
    public double BaseNextLootDecay = 0.01;
    public double BaseClickDamage = 1;

    public int BaseMinimalDestinationGap = 5;
    public int BaseMaximalDestinationGap = 7;
    public int BaseDestinationGapIncrease = 2;

    public int BaseMaxTripulation = 1;
    
    // Valores Atuais
    public Dictionary<GameHelper.ItemRarity, float> ActualIngredientRarityWeights = new Dictionary<GameHelper.ItemRarity, float>();

    public double ActualSpawnChance = 1;
    public float ActualSpawnInterval = 5f;
    public double ActualSpawnBudget = 4;
    public double ActualSpawnBudgetGrowth = 0.55;
    public double ActualBossThreshold = 200;

    public double ActualStartExperience = 0;
    public double ActualExperienceKillBonus = 1;
    public double ActualDayReward = 1;
    public double ActualNightReward = 1;
    public double ActualDestinationReward = 1;

    public int ActualMaxMarkedEnemies = 1;
    public int ActualMaxMarkedLoot = 1;
    public double ActualNextLootChance = 0.05;
    public double ActualNextLootDecay = 0.01;
    public double ActualClickDamage = 1;

    public int ActualMinimalDestinationGap = 5;
    public int ActualMaximalDestinationGap = 7;
    public int ActualDestinationGapIncrease = 2;

    public int ActualMaxTripulation = 1;
}
