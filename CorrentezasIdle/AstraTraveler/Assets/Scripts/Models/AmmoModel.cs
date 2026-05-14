using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoModel
{
    public string Id;

    public string NamePT;
    public string NameEN;

    public string DescriptionPT;
    public string DescriptionEN;

    public WeaponHelper.AmmoType Type;

    public double Damage;
    public double Recharge;
    public int Ammount;
    public WeaponHelper.SpecialType Special;

    public ProjectileModel Projectile;
    public float ProjectileSpeed;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;
}
