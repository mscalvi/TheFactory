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

    [Flags]
    public enum RegionType
    {
        None = 0,
        Sea = 1 << 0,
        HighSea = 1 << 1,
        Entrilhas = 1 << 2,
        Urban = 1 << 3,
        Cold = 1 << 4,
        River = 1 << 5,
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
