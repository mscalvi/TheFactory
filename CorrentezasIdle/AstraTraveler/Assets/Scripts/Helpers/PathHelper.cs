using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHelper
{
    [Flags]
    public enum PathType
    {
        None = 0,
        Sea = 1 << 0,
        HighSea = 1 << 1,
        Entrilhas = 1 << 2,
        Urban = 1 << 3,
        Cold = 1 << 4,
        River = 1 << 5,
    }
}
