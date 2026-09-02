using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInstance
{
    public PlayerModel Model;

    public string Name;
    public int CurrentLife;
    public DeckInstance CurrentDeck;

    public PlayerInstance(PlayerModel model)
    {
        Model = model;

        Name = "Guest Player";

        CurrentLife = 20;
        CurrentDeck = null;
    }
}
