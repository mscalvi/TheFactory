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

    private DecisionsPopUpDesigner DecisionPanel;
    private FinalPopUpDesigner FinalPanel;

    private List<DestinationInstance> DestinationOptions;
    private List<PathInstance> PathOptions;

    public void Initialize(ExpeditionState expedition, ShipState ship, GameState game, DecisionsPopUpDesigner panel, TickService tick, DataState db, FinalPopUpDesigner finalpanel, PathService path)
    {
        TickService = tick;
        PathService = path;

        ExpeditionState = expedition;
        ShipState = ship;
        GameState = game;
        DataState = db;

        DecisionPanel = panel;
        FinalPanel = finalpanel;

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
        if (ExpeditionState.CurrentDestination != null)
        {
            DestinationOptions.Clear();
            PathOptions.Clear();

            foreach (var destination in ExpeditionState.CurrentDestination.CloseDestinations)
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
            Debug.Log("Ship não possui Destination atual");
        }
    }

    private void DestinationSelection(DestinationInstance selected)
    {
        PathService.NewDestinationChose(selected);

        if (ExpeditionState.CurrentDestination == null)
        {
            DecisionPanel.Hide();

            TickService.Resume();

            RunEvents.OnDestinationChose?.Invoke();
        }
    }

    private void PathSelection(PathInstance selected)
    {
        PathService.NewPathChose(selected);

        if (ExpeditionState.CurrentDestination == null)
        {
            DecisionPanel.Hide();

            TickService.Resume();

            RunEvents.OnDestinationChose?.Invoke();
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
        RunEvents.OnFinalPopUpClose?.Invoke();
    }

    // Events

    void OnEnable()
    {
        RunEvents.OnDestinationArrival += PauseShip;
        RunEvents.OnPathOptionsCalculated += SendShip;
    }

    void OnDisable()
    {
        RunEvents.OnDestinationArrival -= PauseShip;
        RunEvents.OnPathOptionsCalculated -= SendShip;
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
