using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;

public class DeckModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    public bool White { get; set; }
    public bool Blue { get; set; }
    public bool Black { get; set; }
    public bool Red { get; set; }
    public bool Green { get; set; }

    public bool CommanderDamage { get; set; }
    public bool Poison { get; set; }
    public bool Experience { get; set; }

    public int? FavoritePlayerId { get; set; }
}