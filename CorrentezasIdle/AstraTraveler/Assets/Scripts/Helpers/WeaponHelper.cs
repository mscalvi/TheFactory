using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHelper
{    public enum AmmoType
    {
        None,
        Throw,
        Arrows,
        Bolts,
        Siege,
        Special,
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

    public enum SpecialType
    {
        None,
        Piercing,
    }

}
