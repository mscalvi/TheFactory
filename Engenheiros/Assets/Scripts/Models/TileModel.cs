using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileModel
{
    public int X { get; }
    public int Y { get; }

    public TileHelper.TileType Type { get; set; }

    public bool BuildTile { get; set; }
    public bool OccupiedTile { get; set; }

    public TileModel(int x, int y, TileHelper.TileType type)
    {
        X = x;
        Y = y;
        Type = type;
    }
}
