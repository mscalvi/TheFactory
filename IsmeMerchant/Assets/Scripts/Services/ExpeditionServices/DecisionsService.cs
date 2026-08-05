using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static GameHelper;

public class DecisionsService : MonoBehaviour
{
    private TickService TickService;
    private PathService PathService;

    private GameState GameState;
    private DataState DataState;
    private ExpeditionState ExpeditionState;

    //[SerializeField] EventPopUp EventPanel;
    [SerializeField] FinalPopUp FinalPanel;

    private bool isShowingEvent = false;

    public void Initialize(GameState game, TickService tick, PathService path)
    {
        TickService = tick;
        PathService = path;

        GameState = game;
        ExpeditionState = GameState.ExpeditionState;
        DataState = GameState.DataState;
    }

    // Game Over
    public void LastDecision()
    {
        TickService.Pause();

        FinalPanel.ShowResults(ExpeditionState, LastSelecion);
    }

    private void LastSelecion(bool victory)
    {
        ExpeditionEvents.OnFinalPopUpClose?.Invoke();
    }

    // Events

    void OnEnable()
    {
        ExpeditionEvents.OnExpeditionEnd += LastDecision;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnExpeditionEnd -= LastDecision;
    }
}
