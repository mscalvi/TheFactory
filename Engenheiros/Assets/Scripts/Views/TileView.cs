using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileHelper;

public class TileView : MonoBehaviour
{
    private TileData Tile;

    public void Setup(TileData tile)
    {
        Tile = tile;
        Debug.Log("Aqui Chegou 5");
    }

    public void SetHeight(TileHeight height)
    {

    }

    public void SetBlocked(bool blocked)
    {

    }
}
