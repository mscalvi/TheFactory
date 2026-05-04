using UnityEngine;

public class ProjectileService : MonoBehaviour
{
    private GameState GameState;

    public GameObject ProjectilePrefab;
    public Transform Container;
    public Transform Ship;

    public void Initialize(GameState game)
    {
        GameState = game;
    }

    private void Update()
    {
        CheckHit();
    }

    void SpawnProjectile(WeaponInstance weapon, EnemyInstance enemy)
    {
        var obj = Instantiate(ProjectilePrefab, Container);
        var proj = obj.GetComponent<ProjectileView>();

        Vector3 origin = Ship.position;

        float radius = UiHelper.ToWorld(enemy.Distance);
        float angleRad = (float)(enemy.Angle * Mathf.Deg2Rad);

        float x = Mathf.Sin(angleRad) * radius;
        float y = -Mathf.Cos(angleRad) * radius;

        Vector3 target = Ship.position + new Vector3(x, y, 0);

        proj.Setup(origin, target, (float)weapon.ProjectileSpeed, GameState.ExpeditionState, Ship, enemy);
    }

    private void CheckHit()
    {
        foreach (var projectile in GameState.ExpeditionState.ActiveProjectiles)
        {

        }
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