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
        if (ExpeditionState.ActualDestination != null)
        {
            ExpeditionState.OldDestination = ExpeditionState.ActualDestination;
            ExpeditionState.ActualDestination = null;
        }

        if (!GameState.ProgressState.m000)
        {
            ExpeditionState.ActualPath = DataState.paths.GetValueOrDefault("p000101");
        }
        else
        {
            ExpeditionState.ActualPath = CalculatePath(selected);
        }
        
        ExpeditionState.NewDestination = selected;

        ExpeditionState.DestinationArrival = CalculateDays(ExpeditionState.ActualPath, ShipState.Ship);
    }

    public void NewPathChose(PathInstance selected)
    {
        ExpeditionState.OldDestination = ExpeditionState.ActualDestination;
        ExpeditionState.NewDestination = null;
        ExpeditionState.ActualDestination = null;

        ExpeditionState.ActualPath = selected;

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

        Debug.LogError("Expedition PathService - Path não encontrado!");
        return null;
    }

    void CalculatePathOptions()
    {
        ExpeditionState.ActualDestination = ExpeditionState.NewDestination;

        if (GameState.ProgressState.m000)
        {
            ExpeditionEvents.OnPathOptionsCalculated?.Invoke();
            Debug.LogError("Expedition PathService - Descobrir o que acontece aqui!");
        }
        else
        {
            ExpeditionState.ExpeditionStatus = GameHelper.ExpeditionStatus.Finished;
            ExpeditionEvents.OnExpeditionEnd?.Invoke();
        }

        GameEvents.OnDestinationArrival?.Invoke(ExpeditionState.ActualDestination);
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
        ExpeditionEvents.OnDestinationArrival += CalculatePathOptions;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnDestinationArrival -= CalculatePathOptions;
    }
}
