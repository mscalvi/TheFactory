using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExpeditionEvents
{
    // Enemy Events
    public static Action<EnemyRuntime> OnEnemySpawn;
    public static Action<EnemyRuntime> OnEnemyClicked;
    public static Action<EnemyRuntime> OnEnemyDeath;
    public static Action<EnemyRuntime> OnMarkedEnemyDeath;
    public static Action NoWaveSpawn;

    // Ship Events
    public static Action<WeaponInstance, EnemyRuntime> OnShoot;
    public static Action<ProjectileRuntime, EnemyRuntime> OnProjectileHit;
    public static Action OnShipAtributeChange;

    // Expedition Status Events
    public static Action OnExpeditionStart;
    public static Action OnExpeditionEnd;
    public static Action OnFinalPopUpClose;

    // Destination Events
    public static Action OnDestinationArrival;
    public static Action OnPathOptionsCalculated;
    public static Action OnPathSet;

    // Day Cycle Events
    public static Action OnDayFinish;
    public static Action OnNightFinish;
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
    public static Action<EnemyInstance> NewEnemySeen;
    public static Action OnBuildingUnlock;
    public static Action<TripulationInstance> OnTripulationPurchase;
    public static Action<string> OnMechanicUnlock;

    // Acquisition Events
    public static Action<AcquisitionInstance> OnAcquisitionFinished;
    public static Action<AcquisitionInstance> OnAcquisitionStarted;
    public static Action<AcquisitionInstance, float, double> OnAcquisitionProgress;

    // Event Events
    public static Action<EventInstance> OnEventTrigger;

    // Purchase Events
    public static Action<UpgradeInstance> OnUpgradeBuy;
    public static Action<AcquisitionInstance> OnAcquisitionBuy;
    public static Action<CurrencyHelper.CurrencyType, CurrencyHelper.CurrencyScope> OnCanBuyChange;
    public static Action<CurrencyHelper.CurrencyType, CurrencyHelper.CurrencyScope> OnCurrencyChange;
}

