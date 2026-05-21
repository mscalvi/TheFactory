using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileHelper;

public class TileData
{
    // Informações do Tile
    public Vector2Int Position;

    public TileHeight Height;

    public TileType TileType;

    public bool IsEntrance;

    public bool LowBlocked;
    public bool NormalBlocked;
    public bool HighBlocked;

    public GameObject Occupant;
}
