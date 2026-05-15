using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileModel
{
    public string Id;

    public string NameEN;
    public string NamePT;

    public WeaponHelper.AmmoType Type;


    // Gameplay
    public float HitRadius;
    public float Lifetime;

    // Visual
    public WeaponHelper.PathType SpritePath;
    public WeaponHelper.BehaviorType BehaviorType;
}
