using UnityEngine;

public class EnemyView : MonoBehaviour
{
    public EnemyRuntime Enemy;
    public Transform Ship;
    private SpriteRenderer spriteRenderer;

    private Vector3 targetPosition;
    private Vector3 originalScale;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(EnemyRuntime enemy, Transform ship)
    {
        Enemy = enemy;
        Ship = ship;

        originalScale = transform.localScale;

        ApplySprite();

        UpdateTargetPosition();
        transform.position = targetPosition;
    }

    void Update()
    {
        if (Enemy == null) return;

        UpdateMarkedVisual();

        UpdateTargetPosition();

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            10f * Time.deltaTime
        );

        Vector3 direction = Ship.position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
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
        if (Enemy.MarkedEnemy)
        {
            transform.localScale = originalScale * 1.25f;
        }
        else
        {
            transform.localScale = originalScale;
        }
    }

    void ApplySprite()
    {
        if (Enemy == null) return;

        Sprite sprite = Resources.Load<Sprite>($"Sprites/Enemies/{Enemy.Id}");

        if (sprite != null)
            spriteRenderer.sprite = sprite;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }
}