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

        ApplySprite();

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

        Vector3 direction = Ship.position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

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
        ExpeditionEvents.OnEnemyClicked?.Invoke(Enemy);
    }

    void UpdateMarkedVisual()
    {
        ApplySprite();

        if (Enemy.MarkedEnemy)
        {
            transform.localScale = Vector3.one * 1.15f;
        }
    }

    void ApplySprite()
    {
        if (Enemy == null) return;

        Sprite sprite = Resources.Load<Sprite>($"Sprites/Enemies/{Enemy.Id}");

        if (sprite != null)
            spriteRenderer.sprite = sprite;
    }
}