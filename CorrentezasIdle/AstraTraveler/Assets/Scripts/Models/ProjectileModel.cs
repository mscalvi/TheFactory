using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileModel
{
    public string Id;

    public string NameEN;
    public string NamePT;

    public WeaponHelper.AmmoType Type;

    // Visual
    public WeaponHelper.PathType SpritePath;

    // Gameplay
    public float Speed;
    public float HitRadius;
    public float Lifetime;

    public WeaponHelper.BehaviorType BehaviorType;
}
