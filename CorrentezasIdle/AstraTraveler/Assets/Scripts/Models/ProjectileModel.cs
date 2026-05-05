using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Projectile")]
public class ProjectileModel : ScriptableObject
{
    public string Id;
    public string Name;

    public WeaponHelper.AmmoType Type;

    public float HitRadius;
    public Sprite Sprite;
}
