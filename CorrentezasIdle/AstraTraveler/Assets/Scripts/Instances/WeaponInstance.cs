using Unity.VisualScripting;
using UnityEngine;

public class WeaponInstance
{
    public WeaponModel Model;

    public string Id;
    public string Name;
    public string Description;

    public double StartDamage;
    public double StartRange;
    public double StartAttackSpeed;
    public double StartPrecision;
    public double StartCriticalDamage;

    public double ActualDamage;
    public double ActualRange;
    public double ActualAttackSpeed;
    public double ActualPrecision;
    public double ActualCriticalDamage;

    public double BaseDamage;
    public double BaseRange;
    public double BaseAttackSpeed;
    public double BasePrecision;
    public double BaseCriticalDamage;

    public string Special;

    public WeaponHelper.AmmoType AmmoType;
    public AmmoInstance Ammo;
    public double ProjectileSpeed;

    public WeaponHelper.WeaponTarget TargetType;
    public EnemyInstance CurrentTarget;
    public double Cooldown;

    public UnlockHelper.UnlockStatus UnlockStatus;

    public WeaponInstance(WeaponModel model)
    {
        Id = model.Id;

        StartDamage = model.Damage;
        StartRange = model.Range;
        StartAttackSpeed = model.AttackSpeed;
        StartPrecision = model.Precision;
        StartCriticalDamage = model.CriticalDamage;

        ActualDamage = model.Damage;
        ActualRange = model.Range;
        ActualAttackSpeed = model.AttackSpeed;
        ActualPrecision = model.Precision;
        ActualCriticalDamage = model.CriticalDamage;

        BaseDamage = model.Damage;
        BaseRange = model.Range;
        BaseAttackSpeed = model.AttackSpeed;
        BasePrecision = model.Precision;
        BaseCriticalDamage = model.CriticalDamage;

        AmmoType = model.AmmoType;

        TargetType = WeaponHelper.WeaponTarget.Closest;
        CurrentTarget = null;
        Cooldown = 0;

        UnlockStatus = model.UnlockStatus;
    }
}