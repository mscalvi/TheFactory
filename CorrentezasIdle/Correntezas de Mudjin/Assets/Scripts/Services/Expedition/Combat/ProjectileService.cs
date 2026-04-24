using UnityEngine;

public class ProjectileService : MonoBehaviour
{
    public GameObject ProjectilePrefab;
    public Transform Container;
    public Transform Ship;

    // Substituir por versão da arma
    public float ProjectileSpeed = 10f;

    void SpawnProjectile(WeaponRoomInstance room, EnemyInstance enemy)
    {
        if (enemy == null)
            return;

        var obj = Instantiate(ProjectilePrefab, Container);
        var proj = obj.GetComponent<ProjectileView>();

        Vector3 origin = Ship.position;

        float radius = (float)enemy.Distance * UiHelper.Scale;
        float angleRad = (float)(enemy.Angle * Mathf.Deg2Rad);

        float x = Mathf.Sin(angleRad) * radius;
        float y = -Mathf.Cos(angleRad) * radius;

        Vector3 target = Ship.position + new Vector3(x, y, 0);

        proj.Setup(origin, target, ProjectileSpeed);

        //proj.Setup(origin, target, room.Weapon.ProjectileSpeed);
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