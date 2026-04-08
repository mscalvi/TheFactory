using UnityEngine;

public class EnemyView : MonoBehaviour
{
    public EnemyInstance Enemy;
    public Transform Ship;
    private SpriteRenderer SpriteRenderer;

    public void Setup(EnemyInstance enemy, Transform ship)
    {
        Enemy = enemy;
        Ship = ship;

        //if (SpriteRenderer == null)
        //    SpriteRenderer = GetComponent<SpriteRenderer>();

        //SpriteRenderer.sprite = enemy.Sprite;
    }

    void Update()
    {
        if (Enemy == null) return;

        UpdatePosition();
    }

    void UpdatePosition()
    {
        float radius = UiHelper.ToWorld(Enemy.Distance);

        float angleRad = (float)(Enemy.Angle * Mathf.Deg2Rad);

        float x = Mathf.Sin(angleRad) * radius;
        float y = -Mathf.Cos(angleRad) * radius;

        Vector3 offset = new Vector3(x, y, 0);
        transform.position = Ship.position + offset;
    }
}