using UnityEngine;
using UnityEngine.UI;

public class CurrencyGrid : MonoBehaviour
{
    public GridLayoutGroup Grid;

    public int Columns = 4;
    public int Lines = 1;
    public float Spacing = 10f;
    public float Padding = 10f;

    void Start()
    {
        Resize();
    }

    void Resize()
    {
        float width = ((RectTransform)transform).rect.width;
        float height = ((RectTransform)transform).rect.height;

        float totalSpacing =
            Spacing * (Columns - 1);

        float totalPadding =
            Padding * 2;

        float cellWidth =
            (width - totalSpacing - totalPadding)
            / Columns;

        float cellHeight =
            (height - totalSpacing - totalPadding)
            / Lines;

        Grid.cellSize =
            new Vector2(cellWidth, cellHeight);
    }
}