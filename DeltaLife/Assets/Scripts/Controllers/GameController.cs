using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEditor.MPE;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] GameUi GameUi;
    [SerializeField] GameCreationService GameCreationService;

    private void Awake()
    {
        var AppState = AppController.Instance.AppState;

        if (AppState == null)
        {
            Debug.LogError("GameController - App NULL!");
            return;
        }

        AppState.GameState = new GameState();
        GameCreationService.Initialize(AppState);
        GameUi.Initialize(AppState);
    }
}
