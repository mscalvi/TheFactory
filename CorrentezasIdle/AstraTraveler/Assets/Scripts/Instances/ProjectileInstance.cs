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
    public string Name;

    public WeaponHelper.AmmoType ammoType;

    public float Speed;

    public Vector3 Position;
    public EnemyInstance Target;
    public float HitRadius;
    public Sprite Sprite;

    public ProjectileInstance(ProjectileModel model)
    {
        Id = model.Id;
        Name = model.Name;

        ammoType = model.Type;
        HitRadius = model.HitRadius;
        Sprite = model.Sprite;

        Speed = 0;
        Position = new Vector3();
        Target = null;
    }
}
