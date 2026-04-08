using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiHelper
{
    public static float Scale = 0.2f;

    public static float ToWorld(double value)
    {
        return (float)value * Scale;
    }
}
