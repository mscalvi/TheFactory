using UnityEngine;
using static TileHelper;

public class GridService : MonoBehaviour
{
    private GameState Game;

    private TileData[,] Grid;

    public void Initialize(GameState game)
    {
        Game = game;

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        Grid = new TileData[Game.Grid.Width, Game.Grid.Height];

        for (int x = 0; x < Game.Grid.Width; x++)
        {
            for (int y = 0; y < Game.Grid.Height; y++)
            {
                TileData tile = new TileData();

                tile.Position = new Vector2Int(x, y);

                tile.Height = TileHeight.Normal;

                tile.LowBlocked = false;
                tile.NormalBlocked = false;
                tile.HighBlocked = false;

                Grid[x, y] = tile;
            }
        }

        GameEvents.OnGridGenerated?.Invoke(Grid);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x * Game.Grid.TileSize, 0, y * Game.Grid.TileSize);
    }

    public TileData GetTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Game.Grid.Width || y >= Game.Grid.Height)
            return null;

        return Grid[x, y];
    }
}