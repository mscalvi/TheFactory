using UnityEngine;

public class ProjectileView : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;
    private float hitRadius = 0.01f;

    private ExpeditionState expeditionState;
    private Transform ship;

    private EnemyInstance enemy;

    public void Setup(Vector3 origin, Vector3 targetPos, float projectileSpeed, ExpeditionState state, Transform shipTransform, EnemyInstance enemyInstance)
    {
        transform.position = origin;
        targetPosition = targetPos;
        speed = projectileSpeed;

        expeditionState = state;
        ship = shipTransform;

        enemy = enemyInstance;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        Vector3 enemyPos = GetEnemyWorldPosition(enemy);

        if (Vector3.Distance(transform.position, enemyPos) <= hitRadius)
        {
            ExpeditionEvents.OnProjectileHit?.Invoke(enemy);

            Destroy(this.gameObject);
            return;
        }
    }

    Vector3 GetEnemyWorldPosition(EnemyInstance enemy)
    {
        float radius = UiHelper.ToWorld(enemy.Distance);
        float angleRad = (float)(enemy.Angle * Mathf.Deg2Rad);

        float x = Mathf.Sin(angleRad) * radius;
        float y = -Mathf.Cos(angleRad) * radius;

        return ship.position + new Vector3(x, y, 0);
    }
}