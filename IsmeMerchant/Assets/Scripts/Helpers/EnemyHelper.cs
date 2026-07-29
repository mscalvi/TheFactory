using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHelper
{
    public enum EnemyType
    {
        Fish,
        Snake,
        Shark,
        Squid,
        Bird,
        Human,
        Monster,
    }

    public enum EnemyState
    {
        Moving,
        Arrival,
        Damaging,
        Cooldown,
        Dead,
        Dying,
    }

    public enum EnemyStage
    {
        Early,
        MidEarly,
        Mid,
        MidLate,
        Late,
        UltraLate,
        HighUltraLate,
    }

    public enum EnemySpecial
    {
        None,
    }
}
