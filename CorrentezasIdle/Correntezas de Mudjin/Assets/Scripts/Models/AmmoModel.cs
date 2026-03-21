using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Ammo")]
public class AmmoModel : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;

    public WeaponHelper.AmmoType Type;

    public double Damage;
    public string Special;

    public UnlockHelper.UnlockStatus UnlockStatus;
}
