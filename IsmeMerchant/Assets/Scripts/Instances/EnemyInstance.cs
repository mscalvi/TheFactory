using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyInstance
{
    public EnemyModel Model;

    public string Id;
    public string NamePT;
    public string NameEN;
    public string DescriptionPT;
    public string DescriptionEN;

    public bool DayEnemy;
    public bool BossEnemy;
    public bool MarkedEnemy;
    public EnemyHelper.EnemyType EnemyType;

    public double StartLife;
    public double ActualLife;
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

    public double Size;

    public double Experience;

    public EnemyHelper.EnemySpecial Special;

    public IngredientHelper.IngredientType CommonIngredient;
    public IngredientHelper.IngredientType UncommonIngredient;
    public IngredientHelper.IngredientType RareIngredient;
    public IngredientHelper.IngredientType LegendaryIngredient;

    public double Rarity;
    public double SpawnCost;
    public List<EnemyHelper.EnemyStage> Stage;

    public EnemyHelper.EnemyState State;

    public PathHelper.PathType PathTypes;
    public PathHelper.PathEnvironment PathEnvironments;
    public PathHelper.PathModifier PathModifiers;

    public double Distance;
    public double Angle;
    public double Cooldown;

    public bool Known;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public EnemyInstance(EnemyModel model)
    {
        Model = model;

        Id = model.Id;

        NameEN = model.NameEN;
        NamePT = model.NamePT;
        DescriptionEN = model.DescriptionEN;
        DescriptionPT = model.DescriptionPT;

        DayEnemy = model.DayEnemy;
        BossEnemy = model.BossEnemy;
        MarkedEnemy = false;
        EnemyType = model.EnemyType;

        StartLife = model.Life;
        ActualLife = model.Life;
        LifeGrowth = model.LifeGrowth;
        LifeRegen = model.LifeRegen;
        LifeRegenGrowth = model.LifeRegenGrowth;
        Speed = model.Speed;
        SpeedGrowth = model.SpeedGrowth;
        Armor = model.Armor;
        ArmorGrowth = model.ArmorGrowth;
        Range = model.Range;
        RangeGrowth = model.RangeGrowth;
        Damage = model.Damage;
        DamageGrowth = model.DamageGrowth;
        AttackSpeed = model.AttackSpeed;
        AttackSpeedGrowth = model.AttackSpeedGrowth;
        SpawnDistance = model.SpawnDistance;
        SpawnDistanceGrowth = model.SpawnDistanceGrowth;

        Size = model.Size;

        Experience = model.Experience;

        Special = model.Special;

        CommonIngredient = model.CommonIngredient;
        UncommonIngredient = model.UncommonIngredient;
        RareIngredient = model.RareIngredient;
        LegendaryIngredient = model.LegendaryIngredient;

        Rarity = model.Rarity;
        SpawnCost = model.SpawnCost;

        State = EnemyHelper.EnemyState.Moving;

        Distance = model.SpawnDistance;
        Angle = 0;
        Cooldown = 1 / model.AttackSpeed;

        Known = false;

        PathTypes = model.PathTypes;
        PathEnvironments = model.PathEnvironments;
        PathModifiers = model.PathModifiers;

        UnlockId = model.UnlockId;
        UnlockStatus = model.UnlockStatus;
    }
}