using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel
{
    public string Id;
    public string NamePT;
    public string NameEN;
    public string DescriptionPT;
    public string DescriptionEN;

    public bool DayEnemy;
    public bool BossEnemy;
    public EnemyHelper.EnemyType EnemyType;

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

    public EnemyHelper.EnemySpecial Special;

    public IngredientHelper.IngredientType CommonIngredient;
    public IngredientHelper.IngredientType UncommonIngredient;
    public IngredientHelper.IngredientType RareIngredient;
    public IngredientHelper.IngredientType LegendaryIngredient;

    public double Rarity;
    public double SpawnCost;
    public List<EnemyHelper.EnemyStage> Stage;

    public List<PathHelper.PathType> PathTypes;
    public List<PathHelper.PathEnvironment> PathEnvironments;
    public List<PathHelper.PathModifier> PathModifiers;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;
}
