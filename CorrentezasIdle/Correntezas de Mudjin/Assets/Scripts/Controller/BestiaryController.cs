using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static GameHelper;

public class BestiaryController : MonoBehaviour
{
    // Base
    [SerializeField] BestiaryUiService BestiaryUiService;

    private void Awake()
    {
        var Game = GameController.Instance.GameState;

        if (Game == null)
        {
            Debug.LogError("ExpeditionController - Game NULL!");
            return;
        }

        BestiaryUiService.Initialize(Game);
    }
}