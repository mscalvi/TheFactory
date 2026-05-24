using Unity.VisualScripting;
using UnityEngine;

public class AmmoInstance
{
    public AmmoModel Model;

    public string Id;
    public string NamePT;
    public string NameEN;

    public string DescriptionPT;
    public string DescriptionEN;

    public WeaponHelper.AmmoType Type;

    public double StartDamage;
    public double BaseDamage;
    public double ActualDamage;

    public double StartRecharge;
    public double BaseRecharge;
    public double ActualRecharge;
    public double CurrentRecharge;

    public bool IsReloading;

    public int StartAmmount;
    public int BaseAmmount;
    public int ActualAmmount;
    public int CurrentAmmount;

    public WeaponHelper.SpecialType Special;

    public string ProjectileId;
    public ProjectileModel Projectile;

    public float StartProjectileSpeed;
    public float BaseProjectileSpeed;
    public float ActualProjectileSpeed;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public AmmoInstance(AmmoModel model)
    {
        Model = model;

        Id = model.Id;

        NamePT = model.NamePT;
        NameEN = model.NameEN;
        DescriptionPT = model.DescriptionPT;
        DescriptionEN = model.DescriptionEN;

        Type = model.Type;

        StartDamage = model.Damage;
        BaseDamage = model.Damage;
        ActualDamage = model.Damage;

        StartRecharge = model.Recharge;
        BaseRecharge = model.Recharge;
        ActualRecharge = model.Recharge;
        CurrentRecharge = model.Recharge;

        IsReloading = false;

        StartAmmount = model.Ammount;
        BaseAmmount = model.Ammount;
        ActualAmmount = model.Ammount;
        CurrentAmmount = model.Ammount;

        Special = model.Special;

        ProjectileId = model.ProjectileId;

        StartProjectileSpeed = model.ProjectileSpeed;
        BaseProjectileSpeed = model.ProjectileSpeed;
        ActualProjectileSpeed = model.ProjectileSpeed;

        UnlockId = model.UnlockId;
        UnlockStatus = model.UnlockStatus;
    }

    public AmmoInstance()
    {

    }
}