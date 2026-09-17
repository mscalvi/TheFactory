using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TileView : MonoBehaviour
{
    private TileModel model;
    private SpriteRenderer spriteRenderer;

    public void Initialize(TileModel model)
    {
        this.model = model;

        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        spriteRenderer.color = GetColor(model.Type);
    }

    private Color GetColor(TileHelper.TileType type)
    {
        return type switch
        {
            TileHelper.TileType.Ground => Color.white,
            TileHelper.TileType.Path => Color.gray,
            TileHelper.TileType.Spawn => Color.green,
            TileHelper.TileType.Goal => Color.red,

            _ => Color.white
        };
    }
}