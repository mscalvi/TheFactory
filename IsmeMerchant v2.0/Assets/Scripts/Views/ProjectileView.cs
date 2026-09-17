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

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }    
}