using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExpeditionEvents
{
    // Enemy Events
    public static Action<EnemyInstance> OnEnemySpawn;
    public static Action<EnemyInstance> OnEnemyClicked;
    public static Action<EnemyInstance> OnEnemyDeath;
    public static Action<EnemyInstance> OnMarkedEnemyDeath;
    public static Action NoWaveSpawn;

    // Ship Events
    public static Action<WeaponInstance, EnemyInstance> OnShoot;
    public static Action<ProjectileInstance, EnemyInstance> OnProjectileHit;
    public static Action OnShipAtributeChange;

    // Expedition Status Events
    public static Action OnExpeditionStart;
    public static Action OnExpeditionEnd;
    public static Action OnFinalPopUpClose;

    // Destination Events
    public static Action OnDestinationArrival;
    public static Action OnPathOptionsCalculated;

    // Day Cycle Events
    public static Action OnDayFinish;
    public static Action OnNightFinish;
}

public static class GameEvents
{
    // Main Events
    public static Action OnGameSave;
    public static Action OnGameLoad;

    // Mission Events
    public static Action<MissionInstance> OnMissionComplete;
    public static Action<MissionInstance> OnMissionCanceled;
    public static Action MissionSlotAtualize;

    // Progress Events
    public static Action NewDayRecord;

    // Unlock Events
    public static Action<EnemyInstance> NewEnemySeen;
    public static Action OnBuildingUnlock;
    public static Action<TripulationInstance> OnTripulationUnlock;
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

    // Tripulation Events
    public static Action<TripulationInstance> OnTripulationChange;
}

