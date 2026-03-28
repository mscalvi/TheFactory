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
    public static Action OnRunStart;
    public static Action OnRunEnd;

    public static Action OnDayFinish;
    public static Action OnNightFinish;

    public static Action OnDestinationArrival;

    public static Action<CurrencyHelper.CurrencyType, CurrencyHelper.CurrencyScope> OnCurrencyChange;
}
