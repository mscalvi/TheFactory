using UnityEngine;
using UnityEngine.UI;

public class UpgradesGrid : MonoBehaviour
{
    public GridLayoutGroup Grid;

    public int Columns = 2;
    public float Spacing = 10f;
    public float Padding = 10f;

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
            new Vector2(cellWidth, 120f);
    }
}