using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyView : MonoBehaviour
{
    private EnemyModel model;
    private SpriteRenderer spriteRenderer;

    public void Initialize(EnemyModel model)
    {
        this.model = model;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}