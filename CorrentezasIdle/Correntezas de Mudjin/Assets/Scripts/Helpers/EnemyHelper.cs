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
    }

    public enum EnemyStage
    {
        Early = 1,
        MidEarly = 2,
        Mid = 3,
        MidLate = 4,
        Late = 5,
        UltraLate = 6,
        HighUltraLate = 7,
    }
}
