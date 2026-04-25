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

    // Ship Events
    public static Action<WeaponRoomInstance, EnemyInstance> OnShoot;
    public static Action OnShipAtributeChange;

    // Expedition Status Events
    public static Action OnExpeditionStart;
    public static Action OnExpeditionEnd;
    public static Action OnFinalPopUpClose;
    public static Action<bool> OnShipDeath;

    // Destination Events
    public static Action OnDestinationChose;
    public static Action<DestinationInstance> OnDestinationArrival;
    public static Action OnPathOptionsCalculated;

    // Day Cycle Events
    public static Action OnDayFinish;
    public static Action OnNightFinish;
}

public static class GameEvents
{
    // Mission Events
    public static Action<MissionInstance> MainMissionFinished;
    public static Action<MissionInstance> OnMissionComplete;
    public static Action<MissionInstance> OnMissionCanceled;
    public static Action MissionSlotAtualize;

    // Records Events
    public static Action NewDayRecord;

    // Unlock Events
    public static Action<EnemyInstance> NewEnemySeen;
    public static Action OnBuildingUnlock;

    // Event Events
    public static Action<EventInstance> OnEventTrigger;

    // Purchase Events
    public static Action<UpgradeInstance> OnUpgradeBuy;
    public static Action<CurrencyHelper.CurrencyType, CurrencyHelper.CurrencyScope> OnCanBuyChange;
    public static Action<CurrencyHelper.CurrencyType, CurrencyHelper.CurrencyScope> OnCurrencyChange;
}

