using System.Collections.Generic;
using UnityEngine;

public class PathService : MonoBehaviour
{
    private MapService mapService;

    private TileModel spawnTile;
    private TileModel goalTile;

    private List<TileModel> path;

    public void Initialize(MapService MapService)
    {
        mapService = MapService;

        path = new List<TileModel>();

        FindSpecialTiles();
        FindPath();

        Debug.Log($"Path encontrado: {path.Count} tiles.");
    }

    private void FindSpecialTiles()
    {
        for (int x = 0; x < mapService.Width; x++)
        {
            for (int y = 0; y < mapService.Height; y++)
            {
                TileModel tile = mapService.GetTile(x, y);

                if (tile.Type == TileHelper.TileType.Spawn)
                    spawnTile = tile;

                if (tile.Type == TileHelper.TileType.Goal)
                    goalTile = tile;
            }
        }
    }

    private void FindPath()
    {
        if (spawnTile == null || goalTile == null)
        {
            Debug.LogError("Spawn ou Goal não encontrado no mapa.");
            return;
        }

        path.Clear();

        TileModel current = spawnTile;
        path.Add(current);

        while (current != goalTile)
        {
            TileModel next = FindNextTile(current);

            if (next == null)
            {
                Debug.LogError("Não foi possível encontrar o caminho até o Goal.");
                return;
            }

            path.Add(next);
            current = next;
        }
    }

    private TileModel FindNextTile(TileModel current)
    {
        TileModel[] neighbors =
        {
            mapService.GetTile(current.X + 1, current.Y),
            mapService.GetTile(current.X - 1, current.Y),
            mapService.GetTile(current.X, current.Y + 1),
            mapService.GetTile(current.X, current.Y - 1)
        };

        foreach (TileModel tile in neighbors)
        {
            if (tile == null)
                continue;

            if (path.Contains(tile))
                continue;

            if (tile.Type == TileHelper.TileType.Path ||
                tile.Type == TileHelper.TileType.Goal)
            {
                return tile;
            }
        }

        return null;
    }

    public IReadOnlyList<TileModel> GetPath()
    {
        return path;
    }

    public TileModel GetSpawnTile()
    {
        return spawnTile;
    }

    public TileModel GetGoalTile()
    {
        return goalTile;
    }
}