using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy")]
public class EnemyModel : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;

    public bool DayEnemy;
    public bool BossEnemy;

    public double Life;
    public double LifeGrowth;
    public double LifeRegen;
    public double LifeRegenGrowth;
    public double Speed;
    public double SpeedGrowth;
    public double Armor;
    public double ArmorGrowth;
    public double Range;
    public double RangeGrowth;
    public double Damage;
    public double DamageGrowth;
    public double AttackSpeed;
    public double AttackSpeedGrowth;
    public double SpawnDistance;
    public double SpawnDistanceGrowth;

    public double Experience;

    public double Rarity;

    public EnemyHelper.EnemyState State;

    public EnemyHelper.EnemyType EnemyType;
    public PathHelper.EnemiesType PathType;

    public UnlockHelper.UnlockStatus UnlockStatus;
}
