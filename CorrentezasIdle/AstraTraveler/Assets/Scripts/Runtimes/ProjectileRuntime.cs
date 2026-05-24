using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using static EnemyHelper;
using static PathHelper;
using static UnlockHelper;

public class ProjectileRuntime
{
    public ProjectileModel Model;

    public Vector3 Position;

    public EnemyRuntime Target;

    public float Speed;

    public float HitRadius;
}
