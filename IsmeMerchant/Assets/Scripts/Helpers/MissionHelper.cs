using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionHelper
{
    public enum MissionType
    {
        MainMission,
        EnemyKilling,
        DaySurvival,
        DayNoDamage,
        IngredientFinding,
    }

    public enum MissionStatus
    {
        Available,
        OnGoing,
        Finished,
        Canceled,
    }
    public static string MissionKey(string missionId, string variant)
    {
        return $"{missionId}|{variant}";
    }
}
