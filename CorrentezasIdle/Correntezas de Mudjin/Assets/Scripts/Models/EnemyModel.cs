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
    public double AtackSpeed;
    public double AtackSpeedGrowth;
    public double SpawnDistance;
    public double SpawnDistanceGrowth;

    public double Rarity;

    public EnemyHelper.EnemyType EnemyType;
    public EnemyHelper.RegionType RegionType;
}
