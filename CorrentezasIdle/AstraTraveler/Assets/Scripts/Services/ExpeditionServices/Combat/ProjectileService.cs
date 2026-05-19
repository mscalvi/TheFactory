using System.Collections.Generic;
using UnityEngine;

public class ProjectileService : MonoBehaviour
{
    private GameState GameState;

    public GameObject ProjectilePrefab;
    public Transform Container;
    public Transform Ship;

    private List<ProjectileInstance> projectiles = new();
    private Dictionary<ProjectileInstance, ProjectileView> views = new();

    public void Initialize(GameState game)
    {
        GameState = game;
    }

    void Update()
    {
        for (int i = projectiles.Count - 1; i >= 0; i--)
        {
            var proj = projectiles[i];

            if (proj.Target == null || proj.Target.State == EnemyHelper.EnemyState.Dead)
            {
                RemoveProjectile(proj);
                continue;
            }

            Vector3 targetPos = GetEnemyWorldPosition(proj.Target);

            proj.Position = Vector3.MoveTowards(
                proj.Position,
                targetPos,
                proj.Speed * Time.deltaTime * GameState.ActualGameSpeed
            );

            views[proj].SetPosition(proj.Position);

            if (Vector3.Distance(proj.Position, targetPos) <= proj.HitRadius)
            {
                ExpeditionEvents.OnProjectileHit?.Invoke(proj, proj.Target);
                RemoveProjectile(proj);
            }
        }
    }

    void SpawnProjectile(WeaponInstance weapon, EnemyInstance enemy)
    {
        var instance = new ProjectileInstance(weapon.Ammo.Projectile)
        {
            Position = Ship.position,
            Target = enemy,
            Speed = (float)weapon.Ammo.ActualProjectileSpeed,
            HitRadius = 0.01f
        };

        projectiles.Add(instance);

        var obj = Instantiate(ProjectilePrefab, Container);
        var view = obj.GetComponent<ProjectileView>();

        views[instance] = view;

        view.SetPosition(instance.Position);
    }

    void RemoveProjectile(ProjectileInstance proj)
    {
        if (views.TryGetValue(proj, out var view))
        {
            Destroy(view.gameObject);
            views.Remove(proj);
        }

        projectiles.Remove(proj);
    }

    Vector3 GetEnemyWorldPosition(EnemyInstance enemy)
    {
        float radius = UiHelper.ToWorld(enemy.Distance);
        float angleRad = (float)(enemy.Angle * Mathf.Deg2Rad);

        float x = Mathf.Sin(angleRad) * radius;
        float y = -Mathf.Cos(angleRad) * radius;

        return Ship.position + new Vector3(x, y, 0);
    }

    void OnEnable()
    {
        ExpeditionEvents.OnShoot += SpawnProjectile;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnShoot -= SpawnProjectile;
    }
}