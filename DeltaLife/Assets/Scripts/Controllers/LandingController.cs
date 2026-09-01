using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingController : MonoBehaviour
{
    [SerializeField] LandingUi LandingUi;

    private void Awake()
    {
        var AppState = AppController.Instance.AppState;

        if (AppState == null)
        {
            Debug.LogError("LandingController - App NULL!");
            return;
        }

        var DataService = AppController.Instance.DataService;

        LandingUi.Initialize(AppState, DataService);
    }
}
