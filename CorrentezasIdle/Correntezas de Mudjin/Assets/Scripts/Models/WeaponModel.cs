using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Weapon")]
public class WeaponModel : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;

    public double Damage;
    public double Range;
    public double AttackSpeed;
    public double Precision;
    public double CriticalDamage;
    public string Special;
    public WeaponHelper.AmmoType AmmoType;

    public UnlockHelper.UnlockStatus UnlockStatus;
}
