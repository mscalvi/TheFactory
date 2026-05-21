using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridView : MonoBehaviour
{
    private GridService GridService;

    private TileView TilePrefab;

    public void Setup(GridService gridService, TileView tileView)
    {
        GridService = gridService;

        TilePrefab = tileView;
    }

    public void BuildGrid(TileData[,] Grid)
    {
        Debug.Log("Aqui Chegou 3");
        var width = Grid.GetLength(0);
        var height = Grid.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileData tile = GridService.GetTile(x, y);

                Vector3 position = GridService.GetWorldPosition(x, y);

                TileView view = Instantiate(
                    TilePrefab,
                    position,
                    Quaternion.identity,
                    transform
                );

                view.Setup(tile);
            }
        }
        Debug.Log("Aqui Chegou 4");
    }
}
