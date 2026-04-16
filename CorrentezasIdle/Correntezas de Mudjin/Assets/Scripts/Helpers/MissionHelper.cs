using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionHelper
{
    public enum MissionType
    {
        EnemiesKilling,
        BossHunt,
        DaySurvival,
        DayNoDamage,
        IngredientFinding,
        CoinFinding,
        EventTrigger,
        LocalFinding,
    }

    public enum MissionStatus
    {
        Available,
        OnGoing,
        Finished,
        Canceled,
    }

    public enum MissionRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary,
    }
}
