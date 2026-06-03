using UnityEngine;
using UnityEngine.UI;

public class ConstructionsGrid : MonoBehaviour
{
    public GridLayoutGroup Grid;

    public int Columns = 1;
    public float Spacing = 0f;
    public float Padding = 0f;

    void Start()
    {
        Resize();
    }

    void Resize()
    {
        float width = ((RectTransform)transform).rect.width;

        float totalSpacing =
            Spacing * (Columns - 1);

        float totalPadding =
            Padding * 2;

        float cellWidth =
            (width - totalSpacing - totalPadding)
            / Columns;

        Grid.cellSize =
            new Vector2(cellWidth, 250f);
    }
}