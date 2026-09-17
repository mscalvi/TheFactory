using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] BootstrapUI BootstrapUI;

    void Start()
    {
        BootstrapUI.Initialize();
    }
}
