using Unity.VisualScripting;
using UnityEngine;

public class WeaponInstance
{
    public WeaponModel Model;

    public string Id;
    public string NamePT;
    public string NameEN;

    public string DescriptionPT;
    public string DescriptionEN;

    public double StartDamage;
    public double StartRange;
    public double StartAttackSpeed;
    public double StartPrecision;
    public double StartCriticalDamage;

    public double BaseDamage;
    public double BaseRange;
    public double BaseAttackSpeed;
    public double BasePrecision;
    public double BaseCriticalDamage;

    public double ActualDamage;
    public double ActualRange;
    public double ActualAttackSpeed;
    public double ActualPrecision;
    public double ActualCriticalDamage;

    public int Angle;
    public int AngleMin;
    public int AngleMax;

    public WeaponHelper.SpecialType Special;

    public WeaponHelper.AmmoType AmmoType;
    public AmmoInstance Ammo;
    
    public WeaponHelper.WeaponTarget TargetType;
    public EnemyRuntime CurrentTarget;
    public double Cooldown;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public WeaponInstance(WeaponModel model)
    {
        Model = model;

        Id = model.Id;

        NamePT = model.NamePT;
        NameEN = model.NameEN;
        DescriptionPT = model.DescriptionPT;
        DescriptionEN = model.DescriptionEN;

        StartDamage = model.Damage;
        StartRange = model.Range;
        StartAttackSpeed = model.AttackSpeed;
        StartPrecision = model.Precision;
        StartCriticalDamage = model.CriticalDamage;

        BaseDamage = model.Damage;
        BaseRange = model.Range;
        BaseAttackSpeed = model.AttackSpeed;
        BasePrecision = model.Precision;
        BaseCriticalDamage = model.CriticalDamage;

        ActualDamage = model.Damage;
        ActualRange = model.Range;
        ActualAttackSpeed = model.AttackSpeed;
        ActualPrecision = model.Precision;
        ActualCriticalDamage = model.CriticalDamage;

        Angle = model.Angle;
        AngleMax = model.AngleMax;
        AngleMin = model.AngleMin;

        AmmoType = model.AmmoType;
        Ammo = null;

        TargetType = WeaponHelper.WeaponTarget.Closest;
        CurrentTarget = null;
        Cooldown = 0;

        UnlockId = model.UnlockId;
        UnlockStatus = model.UnlockStatus;
    }

    public WeaponInstance()
    {

    }
}