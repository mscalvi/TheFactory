using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CombatEvents
{
    public static Action<EnemyInstance> OnEnemyDeath;
    public static Action<EnemyInstance> OnEnemySpawn;
}

public static class RunEvents
{
    public static Action OnExpeditionStart;
    public static Action OnExpeditionEnd;
    public static Action OnFinalPopUpClose;

    public static Action OnDayFinish;
    public static Action OnNightFinish;

    public static Action OnDestinationArrival;

    public static Action<CurrencyHelper.CurrencyType, CurrencyHelper.CurrencyScope> OnCurrencyChange;
}

public static class ShipEvents
{
    public static Action OnAtributeChange;
    public static Action<UpgradeInstance> OnUpgradeBuy;
    public static Action<UpgradeInstance> AfterUpgradeBuy;
    public static Action<UpgradeInstance> OnCanBuyChange;

}

public static class BuildingEvents
{
    public static Action<UpgradeInstance> OnUpgradeBuy;
}
