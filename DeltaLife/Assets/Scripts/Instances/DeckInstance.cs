using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckInstance
{
    public DeckModel Model;

    public string Name;

    public bool White;
    public bool Blue;
    public bool Black;
    public bool Red;
    public bool Green;
    public bool Colorless;

    public bool CommanderDamage;
    public bool Poison;
    public bool Experience;

    public PlayerModel FavoritePlayer;

    public DeckInstance(DeckModel model)
    {
        Model = model;

        Name = "Custom Deck";

        White = false;
        Blue = false;
        Black = false;
        Red = false;
        Green = false;
        Colorless = false;

        CommanderDamage = false;
        Poison = false;
        Experience = false;

        FavoritePlayer = null;
    }
}
