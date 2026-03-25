using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHelper
{
    public enum WeaponRoomType
    {
        SmallPanoramic,
        NormalPanoramic,
        BigPanoramic,
        Left,
        Right,
        LeftCorner,
        RightCorner,
        Frontal,
    }

    public enum RoomStatus
    {
        Empty,
        InUse,
        Iddle,
    }

    public enum RoomTarget
    {
        None,
        Closest,
        Farest,
        LowestHp,
        HighestHp,
        BossFirst,
        SpecialFirst,
        HighestLevel,
        LowerLevel,
    }
}
