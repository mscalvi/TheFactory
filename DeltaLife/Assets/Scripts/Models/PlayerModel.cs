using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;

public class PlayerModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
}
