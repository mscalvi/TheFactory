using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState game)
    {
        GameState = game;
    }
}
