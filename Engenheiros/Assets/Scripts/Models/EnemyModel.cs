using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel
{
    public float Speed { get; set; }
    public int Health { get; set; }

    public EnemyModel(float speed, int health)
    {
        Speed = speed;
        Health = health;
    }
}
