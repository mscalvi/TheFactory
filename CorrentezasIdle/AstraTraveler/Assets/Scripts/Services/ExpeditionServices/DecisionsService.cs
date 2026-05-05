using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SearchService;
using UnityEngine;
using static GameHelper;
using static UnityEditor.Progress;

public class DecisionsService : MonoBehaviour
{
    private TickService TickService;
    private PathService PathService;

    private GameState GameState;
    private DataState DataState;
    private ExpeditionState ExpeditionState;

    //[SerializeField] EventPopUp EventPanel;
    [SerializeField] DestinationsPopUp DecisionPanel;
    [SerializeField] FinalPopUp FinalPanel;

    private Queue<EventInstance> eventQueue = new Queue<EventInstance>();
    private bool isShowingEvent = false;

    public void Initialize(GameState game, TickService tick, PathService path)
    {
        TickService = tick;
        PathService = path;

        GameState = game;
        ExpeditionState = GameState.ExpeditionState;
        DataState = GameState.DataState;
    }

    // Events
    private void EventHappen(EventInstance eventInstance)
    {
        eventQueue.Enqueue(eventInstance);

        TryShowNextEvent();
    }

    private void TryShowNextEvent()
    {
        if (isShowingEvent) return;

        if (eventQueue.Count == 0) return;

        var nextEvent = eventQueue.Dequeue();

        isShowingEvent = true;

        //EventPanel.ShowEvent(nextEvent, EventConfirm);
    }

    private void EventConfirm(bool Confirmed)
    {
        //EventPanel.Hide();

        isShowingEvent = false;

        TryShowNextEvent();
    }

    // Destination
    private void DestinationOptions()
    {
        TickService.Pause();
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
        ExpeditionEvents.OnDestinationArrival += DestinationOptions;
        ExpeditionEvents.OnExpeditionEnd += LastDecision;
        GameEvents.OnEventTrigger += EventHappen;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnDestinationArrival -= DestinationOptions;
        ExpeditionEvents.OnExpeditionEnd -= LastDecision;
        GameEvents.OnEventTrigger -= EventHappen;
    }
}
