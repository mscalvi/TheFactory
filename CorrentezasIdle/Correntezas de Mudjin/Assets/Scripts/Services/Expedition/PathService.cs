using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathService : MonoBehaviour
{
    private GameState GameState;
    private ShipState ShipState;
    private DataState DataState;
    private ExpeditionState ExpeditionState;

    private List<DestinationInstance> DestinationOptions;
    private List<PathInstance> PathOptions;

    public void Initialize(ExpeditionState expedition, ShipState ship, GameState game, DataState db)
    {
        ExpeditionState = expedition;
        ShipState = ship;
        GameState = game;
        DataState = db;

        DestinationOptions = new List<DestinationInstance>();
        PathOptions = new List<PathInstance>();
    }

    public void NewDestinationChose(DestinationInstance selected)
    {
        if (ExpeditionState.CurrentDestination != null)
        {
            ExpeditionState.OldDestination = ExpeditionState.CurrentDestination;
        }

        if (!GameState.ProgressState.m000)
        {
            ExpeditionState.CurrentPath = DataState.paths.GetValueOrDefault("p000101");
        }
        else
        {
            ExpeditionState.CurrentPath = CalculatePath(selected);
        }
        
        ExpeditionState.NewDestination = selected;

        ExpeditionState.DestinationArrival = CalculateDays(ExpeditionState.CurrentPath, ShipState.Ship);
    }

    public void NewPathChose(PathInstance selected)
    {
        ExpeditionState.OldDestination = ExpeditionState.CurrentDestination;
        ExpeditionState.NewDestination = null;
        ExpeditionState.CurrentDestination = null;

        ExpeditionState.CurrentPath = selected;

        ExpeditionState.DestinationArrival = 0;
    }

    private PathInstance CalculatePath(DestinationInstance newDestination)
    {
        var oldDestination = GameState.ExpeditionState.OldDestination;

        foreach (var path in GameState.DataState.paths)
        {
            var p = path.Value;

            bool forward = p.Destination1.Id == oldDestination.Id &&
                           p.Destination2.Id == newDestination.Id;

            bool backward = p.Destination2.Id == oldDestination.Id &&
                            p.Destination1.Id == newDestination.Id;

            if (forward || backward)
            {
                return p;
            }
        }

        Debug.LogError("Path não encontrado!");
        return null;
    }

    void CalculatePathOptions()
    {
        ExpeditionState.CurrentDestination = ExpeditionState.NewDestination;

        if (GameState.ProgressState.m000)
        {
            RunEvents.OnPathOptionsCalculated?.Invoke();
        }
        else
        {
            ExpeditionState.ExpeditionStatus = GameHelper.ExpeditionStatus.Finished;
            ProgressEvents.OnFirstExpeditionFinish?.Invoke();
            RunEvents.OnExpeditionEnd?.Invoke();
        }
    }

    // Helpers
    private int CalculateDays(PathInstance Path, ShipInstance Ship)
    {
        int RealDistance = (int)Math.Ceiling(Path.Distance / Ship.BaseSpeed);

        return RealDistance;
    }

    // Events

    void OnEnable()
    {
        RunEvents.OnDestinationArrival += CalculatePathOptions;
    }

    void OnDisable()
    {
        RunEvents.OnDestinationArrival -= CalculatePathOptions;
    }
}
