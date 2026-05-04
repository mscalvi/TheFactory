using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHelper
{
    [Flags]
    public enum AmmoType
    {
        None = 0,
        Rocks = 1 << 0,
        Vials = 1 << 1,
        Arrows = 1 << 2,
        Bolts = 1 << 3,
        Siege = 1 << 4,
        Special = 1 << 5,
    }

    public enum WeaponTarget
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
