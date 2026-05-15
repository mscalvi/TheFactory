using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using static EnemyHelper;
using static PathHelper;
using static UnlockHelper;

public class ProjectileInstance
{
    ProjectileModel model;

    public string Id;
    public string NameEN;
    public string NamePT;

    public WeaponHelper.AmmoType AmmoType;

    public float Speed;
    public float HitRadius;
    public float Lifetime;

    public WeaponHelper.PathType SpritePath;
    public WeaponHelper.BehaviorType BehaviorType;

    public Vector3 Position;
    public EnemyInstance Target;

    public ProjectileInstance(ProjectileModel model)
    {
        Id = model.Id;

        NameEN = model.NameEN;
        NamePT = model.NamePT;

        AmmoType = model.Type;

        Speed = 0;
        HitRadius = model.HitRadius;
        Lifetime = model.Lifetime;

        SpritePath = model.SpritePath;
        BehaviorType = model.BehaviorType;

        Position = new Vector3();
        Target = null;
    }
}
