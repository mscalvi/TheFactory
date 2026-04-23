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
    private ShipState ShipState;
    private DataState DataState;
    private ExpeditionState ExpeditionState;

    private EventPopUpDesigner EventPanel;
    private DecisionsPopUpDesigner DecisionPanel;
    private FinalPopUpDesigner FinalPanel;

    private List<DestinationInstance> DestinationOptions;
    private List<PathInstance> PathOptions;

    private Queue<EventInstance> eventQueue = new Queue<EventInstance>();
    private bool isShowingEvent = false;

    public void Initialize(ExpeditionState expedition, ShipState ship, GameState game, DecisionsPopUpDesigner panel, TickService tick, DataState db, FinalPopUpDesigner finalpanel, PathService path, EventPopUpDesigner eventpanel)
    {
        TickService = tick;
        PathService = path;

        ExpeditionState = expedition;
        ShipState = ship;
        GameState = game;
        DataState = db;

        DecisionPanel = panel;
        FinalPanel = finalpanel;
        EventPanel = eventpanel;

        DestinationOptions = new List<DestinationInstance>();
        PathOptions = new List<PathInstance>();

        if (!GameState.ProgressState.m000)
        {
            DestinationOptions.Clear();
            DestinationSelection(DataState.destinations.GetValueOrDefault("d101"));
        } else
        {
            TickService.Pause();

            GameModeDecision();
        }
    }

    private void GameModeDecision()
    {
        if (ExpeditionState.ActualDestination != null)
        {
            DestinationOptions.Clear();
            PathOptions.Clear();

            foreach (var destination in ExpeditionState.ActualDestination.CloseDestinations)
            {
                if (destination.Key.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
                {
                    DestinationOptions.Add(destination.Key);
                }
                else
                {
                    PathOptions.Add(destination.Value);
                }
            }

            DecisionPanel.ShowOptions(DestinationOptions, PathOptions, DestinationSelection, PathSelection);
        } else
        {
            Debug.Log("Expedition DecisionService - Ship não possui Destination atual.");
        }
    }

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

        EventPanel.ShowEvent(nextEvent, EventConfirm);
    }

    private void EventConfirm(bool Confirmed)
    {
        EventPanel.Hide();

        isShowingEvent = false;

        TryShowNextEvent();
    }

    private void DestinationSelection(DestinationInstance selected)
    {
        PathService.NewDestinationChose(selected);

        if (ExpeditionState.ActualDestination == null)
        {
            DecisionPanel.Hide();

            TickService.Resume();

            ExpeditionEvents.OnDestinationChose?.Invoke();
        }
    }

    private void PathSelection(PathInstance selected)
    {
        PathService.NewPathChose(selected);

        if (ExpeditionState.ActualDestination == null)
        {
            DecisionPanel.Hide();

            TickService.Resume();

            ExpeditionEvents.OnDestinationChose?.Invoke();
        }
    }

    // Game Over
    public void LastDecision(bool victory)
    {
        TickService.Pause();

        FinalPanel.ShowResults(victory, ExpeditionState, LastSelecion);
    }

    private void LastSelecion(bool victory)
    {
        ExpeditionEvents.OnFinalPopUpClose?.Invoke();
    }

    // Events

    void OnEnable()
    {
        ExpeditionEvents.OnDestinationArrival += PauseShip;
        ExpeditionEvents.OnPathOptionsCalculated += SendShip;
        ExpeditionEvents.OnEventTrigger += EventHappen;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnDestinationArrival -= PauseShip;
        ExpeditionEvents.OnPathOptionsCalculated -= SendShip;
        ExpeditionEvents.OnEventTrigger -= EventHappen;
    }

    private void PauseShip()
    {
        TickService.Pause();
    }

    private void SendShip()
    {
        GameModeDecision();
    }
}
