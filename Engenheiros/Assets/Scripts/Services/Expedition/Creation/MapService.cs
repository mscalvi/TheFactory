using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class MapService : MonoBehaviour
{
    [SerializeField] private float tileSize = 1f;

    [SerializeField] private GameObject tilePrefab;

    private TileModel[,] tiles;

    private MapData map;

    public void Initialize(MapData Map)
    {
        map = Map;

        CreateMap();
    }

    // Construção
    private void CreateMap()
    {
        tiles = new TileModel[map.Width, map.Height];

        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                CreateTile(x, y);
            }
        }
    }

    private void CreateTile(int x, int y)
    {
        TileHelper.TileType type = GetTileType(x, y);

        TileModel model = new TileModel(x, y, type);

        tiles[x, y] = model;

        GameObject tileObject = Instantiate(tilePrefab, transform);

        tileObject.name = $"Tile_{x}_{y}";

        tileObject.transform.position = new Vector3(
            x * tileSize,
            y * tileSize,
            0f
        );

        TileView view = tileObject.GetComponent<TileView>();

        view.Initialize(model);
    }

    private TileHelper.TileType GetTileType(int x, int y)
    {
        char tile = map.Layout[map.Height - 1 - y][x];

        return tile switch
        {
            'G' => TileHelper.TileType.Ground,
            'P' => TileHelper.TileType.Path,
            'S' => TileHelper.TileType.Spawn,
            'T' => TileHelper.TileType.Goal,
            _ => TileHelper.TileType.Ground
        };
    }

    // Consultas
    public TileModel GetTile(int x, int y)
    {
        if (x < 0 || x >= map.Width ||
            y < 0 || y >= map.Height)
        {
            return null;
        }

        return tiles[x, y];
    }
    public int Width => tiles.GetLength(0);
    public int Height => tiles.GetLength(1);
}