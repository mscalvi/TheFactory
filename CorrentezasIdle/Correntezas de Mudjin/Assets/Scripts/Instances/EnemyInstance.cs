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
    public double Cost;
    public EnemyHelper.EnemyStage Stage;

    public EnemyHelper.EnemyState State;

    public EnemyHelper.EnemyType EnemyType;
    public PathHelper.PathType PathType;

    public double Distance;
    public double Angle;
    public double Cooldown;
    public double CurrentLife;

    public EnemyInstance(EnemyModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Description = model.Description;

        DayEnemy = model.DayEnemy;
        BossEnemy = model.BossEnemy;

        Life = model.Life;
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

        Rarity = model.Rarity;
        Cost = model.Cost;
        Stage = model.Stage;

        State = EnemyHelper.EnemyState.Moving;

        EnemyType = model.EnemyType;
        PathType = model.PathType;

        Distance = model.SpawnDistance;
        Angle = 0;
        Cooldown = 1 / model.AttackSpeed;
        CurrentLife = model.Life;
    }

    public EnemyInstance(EnemyInstance model)
    {
        Id = model.Id;
        Name = model.Name;
        Description = model.Description;

        DayEnemy = model.DayEnemy;
        BossEnemy = model.BossEnemy;

        Life = model.Life;
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

        Rarity = model.Rarity;
        Cost = model.Cost;
        Stage = model.Stage;

        State = EnemyHelper.EnemyState.Moving;

        EnemyType = model.EnemyType;

        Distance = model.SpawnDistance;
        Angle = 0;
        Cooldown = 1 / model.AttackSpeed;
        CurrentLife = model.Life;
    }
}