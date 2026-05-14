using Unity.VisualScripting;
using UnityEngine;

public class EnemyInstance
{
    public EnemyModel Model;

    public string Id;
    public string Name;
    public string Description;

    public bool DayEnemy;
    public bool BossEnemy;
    public bool MarkedEnemy;

    public double StartLife;
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
    public PathHelper.PathModifier PathModifier;
    public PathHelper.PathEnvironment PathEnvironment;

    public double Distance;
    public double Angle;
    public double Cooldown;
    public double CurrentLife;

    public UnlockHelper.UnlockStatus UnlockStatus;

    public EnemyInstance(EnemyModel model)
    {
        Id = model.Id;

        DayEnemy = model.DayEnemy;
        BossEnemy = model.BossEnemy;
        MarkedEnemy = false;

        StartLife = model.Life;
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


        Experience = model.Experience;
        CommonIngredient = model.CommonIngredient;
        UncommonIngredient = model.UncommonIngredient;
        RareIngredient = model.RareIngredient;
        LegendaryIngredient = model.LegendaryIngredient;

        Rarity = model.Rarity;

        State = EnemyHelper.EnemyState.Moving;

        EnemyType = model.EnemyType;

        Distance = model.SpawnDistance;
        Angle = 0;
        Cooldown = 1 / model.AttackSpeed;
        CurrentLife = model.Life;

        UnlockStatus = model.UnlockStatus;
    }

    public EnemyInstance(EnemyInstance model)
    {
        Id = model.Id;
        Name = model.Name;
        Description = model.Description;

        DayEnemy = model.DayEnemy;
        BossEnemy = model.BossEnemy;
        MarkedEnemy = false;

        StartLife = model.StartLife;
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

        Sprite = model.Sprite;

        Experience = model.Experience;
        CommonIngredient = model.CommonIngredient;
        UncommonIngredient = model.UncommonIngredient;
        RareIngredient = model.RareIngredient;
        LegendaryIngredient = model.LegendaryIngredient;

        Rarity = model.Rarity;
        Cost = model.Cost;
        Stage = model.Stage;

        State = EnemyHelper.EnemyState.Moving;

        EnemyType = model.EnemyType;
        PathType = model.PathType;
        PathEnvironment = model.PathEnvironment;
        PathModifier = model.PathModifier;

        Distance = model.SpawnDistance;
        Angle = 0;
        Cooldown = 1 / model.AttackSpeed;
        CurrentLife = model.StartLife;

        UnlockStatus = model.UnlockStatus;
    }
}