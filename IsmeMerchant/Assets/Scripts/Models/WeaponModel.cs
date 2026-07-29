using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModel
{
    public string Id;

    public string NamePT;
    public string NameEN;

    public string DescriptionPT;
    public string DescriptionEN;

    public double Damage;
    public double Range;
    public double AttackSpeed;
    public double Precision;
    public double CriticalDamage;
    public WeaponHelper.SpecialType Special;

    public int Angle;
    public int AngleMin;
    public int AngleMax;

    public WeaponHelper.AmmoType AmmoType;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;
}
