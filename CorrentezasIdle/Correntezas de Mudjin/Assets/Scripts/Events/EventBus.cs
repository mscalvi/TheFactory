using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CombatEvents
{
    public static Action<EnemyInstance> OnEnemyDeath;
    public static Action<EnemyInstance> OnMarkedEnemyDeath;
    public static Action<EnemyInstance> OnEnemySpawn;
    public static Action<WeaponRoomInstance, EnemyInstance> OnShoot;
    public static Action<EnemyInstance> OnEnemyClicked;
}

public static class RunEvents
{
    public static Action OnExpeditionStart;
    public static Action OnExpeditionEnd;
    public static Action OnFinalPopUpClose;

    public static Action OnDayFinish;
    public static Action OnNightFinish;

    public static Action OnDestinationChose;
    public static Action OnDestinationArrival;
    public static Action OnPathOptionsCalculated;

    public static Action<CurrencyHelper.CurrencyType, CurrencyHelper.CurrencyScope> OnCurrencyChange;
}

public static class ShipEvents
{
    public static Action OnAtributeChange;
    public static Action<UpgradeInstance> OnUpgradeBuy;
    public static Action<UpgradeInstance> AfterUpgradeBuy;
    public static Action<UpgradeInstance> OnCanBuyChange;
}

public static class CompanyEvents
{
    public static Action<UpgradeInstance> OnUpgradeBuy;
    public static Action<UpgradeInstance> AfterUpgradeBuy;
    public static Action<CurrencyHelper.CurrencyType, CurrencyHelper.CurrencyScope> OnCurrencyChange;
    public static Action OnBuildingUnlock;
}

public static class ProgressEvents
{
    public static Action OnFirstExpeditionFinish;
}
