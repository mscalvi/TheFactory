using UnityEngine;

public class ProjectileView : MonoBehaviour
{
    private Vector3 start;
    private Vector3 target;
    private float speed;

    public void Setup(Vector3 origin, Vector3 targetPos, float projectileSpeed)
    {
        start = origin;
        target = targetPos;
        speed = projectileSpeed;

        transform.position = origin;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}