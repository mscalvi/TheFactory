using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExpeditionEvents
{
    // Enemy Events
    public static Action<EnemyRuntime> OnEnemySpawn;
    public static Action<EnemyRuntime> OnEnemyClicked;
    public static Action<EnemyRuntime, Vector3> OnEnemyDeath;
    public static Action<EnemyRuntime> OnMarkedEnemyDeath;
    public static Action NoWaveSpawn;
    public static Action SpawnBoss;
    public static Action<EnemyRuntime> OnBossSpawn;

    // Ship Events
    public static Action<WeaponInstance, EnemyRuntime> OnShoot;
    public static Action<ProjectileRuntime, EnemyRuntime, Vector3> OnProjectileHit;
    public static Action OnShipAtributeChange;
    public static Action<WeaponInstance> OnRechargeStart;
    public static Action<WeaponInstance> OnRechargeProgress;
    public static Action<WeaponInstance> OnRechargeEnd;

    // Expedition Status Events
    public static Action OnExpeditionStart;
    public static Action OnExpeditionEnd;
    public static Action OnFinalPopUpClose;

    // Destination Events
    public static Action BeforeDestinationArrival;
    public static Action OnDestinationArrival;
    public static Action OnPathOptionsCalculated;
    public static Action OnPathSet;

    // Day Cycle Events
    public static Action OnDayFinish;
    public static Action OnNightFinish;

    // Ui Events
    public static Action<CurrencyInstance, double> CurrencyIncome;
    public static Action<IngredientInstance, double> IngredientIncome;
}

public static class GameEvents
{
    // Test Events
    public static Action MoneyTest;
    public static Action LifeTest;

    // Main Events
    public static Action OnGameSave;
    public static Action OnGameLoad;
    public static Action OnLanguageChange;

    // Mission Events
    public static Action<MissionRuntime> OnMissionComplete;
    public static Action<MissionRuntime> OnMissionCanceled;
    public static Action<MissionRuntime> OnMissionUpdate;
    public static Action MissionSlotAtualize;

    // Progress Events
    public static Action NewDayRecord;

    // Unlock Events
    public static Action<int, string> OnUpgradeAvailable;
    public static Action<EnemyInstance> NewEnemySeen;
    public static Action OnBuildingUnlock;
    public static Action<string> OnMechanicUnlock;

    // Purchase Events
    public static Action<UpgradeInstance> OnUpgradeBuy;
    public static Action<UpgradeInstance> OnUpgradeBought;
    public static Action<CurrencyHelper.CurrencyType, CurrencyHelper.CurrencyScope> OnCanBuyChange;
    public static Action<CurrencyHelper.CurrencyType, CurrencyHelper.CurrencyScope> OnCurrencyChange;
}

