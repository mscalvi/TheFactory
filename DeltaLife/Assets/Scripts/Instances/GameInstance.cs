using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstance
{
    public GameModel Model;

    public string Id;
    public FormatHelper.Format Format;

    public PlayerInstance Player1;
    public PlayerInstance Player2;
    public PlayerInstance Player3;
    public PlayerInstance Player4;

    public GameInstance(GameModel model)
    {
        Model = model;

        Id = model.Id;
        Format = model.Format;

        Player1 = null;
        Player2 = null;
        Player3 = null;
        Player4 = null;
    }
}
