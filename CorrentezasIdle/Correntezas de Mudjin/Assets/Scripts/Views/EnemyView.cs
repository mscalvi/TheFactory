using UnityEngine;

public class EnemyView : MonoBehaviour
{
    public EnemyInstance Enemy;
    public Transform Ship;
    private SpriteRenderer spriteRenderer;

    private Vector3 targetPosition;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(EnemyInstance enemy, Transform ship)
    {
        Enemy = enemy;
        Ship = ship;

        ApplyDebugColor();

        UpdateTargetPosition();
        transform.position = targetPosition;
    }

    void Update()
    {
        if (Enemy == null) return;

        UpdateTargetPosition();

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            10f * Time.deltaTime
        );

        UpdateMarkedVisual();
    }

    void UpdateTargetPosition()
    {
        float radius = UiHelper.ToWorld(Enemy.Distance);

        float angleRad = (float)(Enemy.Angle * Mathf.Deg2Rad);

        float x = Mathf.Sin(angleRad) * radius;
        float y = -Mathf.Cos(angleRad) * radius;

        Vector3 offset = new Vector3(x, y, 0);
        targetPosition = Ship.position + offset;
    }

    void OnMouseDown()
    {
        if (Enemy == null) return;
        CombatEvents.OnEnemyClicked?.Invoke(Enemy);
    }

    void UpdateMarkedVisual()
    {
        if (Enemy.MarkedEnemy)
            spriteRenderer.color = Color.white;
        else
            ApplyDebugColor();
    }

    void ApplyDebugColor()
    {
        if (Enemy == null) return;

        switch (Enemy.Id)
        {
            case "e001":
                spriteRenderer.color = Color.cyan;
                break;

            case "e002":
                spriteRenderer.color = Color.green;
                break;

            case "e101":
                spriteRenderer.color = Color.blue;
                break;

            case "102":
                spriteRenderer.color = Color.magenta;
                break;

            default:
                spriteRenderer.color = Color.red;
                break;
        }
    }
}