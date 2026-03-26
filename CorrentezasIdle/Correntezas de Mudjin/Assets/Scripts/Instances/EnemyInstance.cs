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

    public double Rarity;

    public EnemyHelper.EnemyState State;

    public EnemyHelper.EnemyType EnemyType;

    public double Distance;
    public double Cooldown;
    public double CurrentLife;

    public EnemyInstance(EnemyModel model, double BaseSpawnDistance)
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

        Rarity = model.Rarity;

        State = EnemyHelper.EnemyState.Moving;

        EnemyType = model.EnemyType;

        Distance = BaseSpawnDistance * model.SpawnDistance;
        Cooldown = 1 / model.AttackSpeed;
        CurrentLife = model.Life;
    }
}