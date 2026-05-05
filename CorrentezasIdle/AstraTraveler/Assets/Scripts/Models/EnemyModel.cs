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

    public Sprite Sprite;

    public double Experience;
    public IngredientHelper.IngredientType CommonIngredient;
    public IngredientHelper.IngredientType UncommonIngredient;
    public IngredientHelper.IngredientType RareIngredient;
    public IngredientHelper.IngredientType LegendaryIngredient;

    public double Rarity;
    public double Cost;
    public EnemyHelper.EnemyStage Stage;

    public EnemyHelper.EnemyState State;

    public EnemyHelper.EnemyType EnemyType;
    public PathHelper.PathType PathType;
    public PathHelper.PathEnvironment PathEnvironment;
    public PathHelper.PathModifier PathModifier;

    public UnlockHelper.UnlockStatus UnlockStatus;
}
