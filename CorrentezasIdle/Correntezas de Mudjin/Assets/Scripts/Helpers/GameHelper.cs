using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHelper
{
    [Flags]
    public enum GameState
    {
        Stopped,
        Paused,
        Running,
        GameOver,
    }
}
