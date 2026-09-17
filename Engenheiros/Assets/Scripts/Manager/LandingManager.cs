using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingManager : MonoBehaviour
{
    [SerializeField] LandingUI LandingUI;

    void Start()
    {
        LandingUI.Initialize();
    }
}
