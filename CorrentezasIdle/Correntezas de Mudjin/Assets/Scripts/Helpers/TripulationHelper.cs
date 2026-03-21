using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripulationHelper
{
    [Flags]
    public enum Jobs
    {
        None = 0,
        Shooter = 1 << 0,
        Merchant = 1 << 1,
        Cartographer = 1 << 2,
        Sailor = 1 << 3,
    }

    public enum Type
    {
        Captain,
        Worker,
        Hunter,
        Merchant,
    }

    public enum Status
    {
        InShip,
        InRoom,
        InBase,
        Idle,
    }
}
