using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHelper
{
    [Flags]
    public enum EnemyType
    {
        None = 0,
        Small = 1 << 0,
        Big = 1 << 1,
        Human = 1 << 2,
        Monster = 1 << 3,
    }

    public enum EnemyState
    {
        Moving,
        Arrival,
        Damaging,
        Cooldown,
        Dead,
    }
}
