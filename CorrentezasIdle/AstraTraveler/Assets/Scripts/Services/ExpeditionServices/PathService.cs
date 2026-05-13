using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathService : MonoBehaviour
{
    private GameState GameState;


    public void Initialize(GameState game)
    {
        GameState = game;
    }

    public void GenerateNextDestination()
    {
        int minGap = GameState.ExpeditionState.ActualMinimalDestinationGap;
        int maxGap = GameState.ExpeditionState.ActualMaximalDestinationGap;

        int gap = UnityEngine.Random.Range(minGap, maxGap + 1);

        GameState.ExpeditionState.NextDestination = GameState.ExpeditionState.ActualDestination + gap;

        GenerateNextPath();
        IncreaseDestinationGap();
    }

    private void GenerateNextPath()
    {
        GameState.ExpeditionState.LastPath = GameState.ExpeditionState.ActualPath;

        int day = GameState.ExpeditionState.ActualDestination;

        var type = GetRandomType(day);

        if (GameState.ExpeditionState.LastPath.Type == type && day > 50)
        {
            if (UnityEngine.Random.value < 0.5f)
            {
                type = GetRandomType(day);
            }
        }

        var env = GetRandomEnvironment(day, type);

        if (GameState.ExpeditionState.LastPath.Environment == env)
        {
            if (UnityEngine.Random.value < 0.5f)
            {
                env = GetRandomEnvironment(day, type);
            }
        }
        var mod = GetRandomModifier(day, type);

        if (GameState.ExpeditionState.LastPath.Modifier == mod)
        {
            if (UnityEngine.Random.value < 0.5f)
            {
                mod = GetRandomModifier(day, type);
            }
        }

        PathHelper.PathTagSet newPathTag = new PathHelper.PathTagSet
        {
            Type = type,
            Environment = env,
            Modifier = mod
        };

        GameState.ExpeditionState.ActualPath = newPathTag;
    }

    private void IncreaseDestinationGap()
    {
        float Increase = UnityEngine.Random.Range(0, GameState.ExpeditionState.ActualDestinationGapIncrease);
        GameState.ExpeditionState.ActualMinimalDestinationGap += (int)(GameState.ExpeditionState.ActualMinimalDestinationGap * Increase);

        Increase = UnityEngine.Random.Range(0, GameState.ExpeditionState.ActualDestinationGapIncrease);
        GameState.ExpeditionState.ActualMaximalDestinationGap += (int)(GameState.ExpeditionState.ActualMaximalDestinationGap * Increase);
    }

    private PathHelper.PathType GetRandomType(int day)
    {
        List<PathHelper.PathType> available = new();

        available.Add(PathHelper.PathType.CurrentEntrilhas);

        if (day > 50)
        {
            available.Remove(PathHelper.PathType.CurrentEntrilhas);
            available.Add(PathHelper.PathType.CoastalShelf);
            available.Add(PathHelper.PathType.InlandSea);
        }

        if (day > 100)
        {
            available.Add(PathHelper.PathType.OpenOcean);
            available.Add(PathHelper.PathType.ForestRiver);
            available.Add(PathHelper.PathType.UrbanRiver);
        }

        if (day > 250)
        {
            available.Add(PathHelper.PathType.DeepSea);
            available.Add(PathHelper.PathType.RiverDelta);
            available.Add(PathHelper.PathType.SaltFlatWaters);
            available.Add(PathHelper.PathType.Lagoon);
            available.Add(PathHelper.PathType.FreshwaterRiver);
        }

        if (day > 500)
        {
            available.Add(PathHelper.PathType.AbyssalPlain);
            available.Add(PathHelper.PathType.CoralReef);
            available.Add(PathHelper.PathType.KelpForest);
            available.Add(PathHelper.PathType.Mangrove);
            available.Add(PathHelper.PathType.Estuary);
            available.Add(PathHelper.PathType.CanyonWaters);
            available.Add(PathHelper.PathType.UnderwaterRidge);
            available.Add(PathHelper.PathType.ThermalVentField);
        }

        if (day > 1000)
        {
            available.Add(PathHelper.PathType.IceEdge);
            available.Add(PathHelper.PathType.GlacierRunoff);
            available.Add(PathHelper.PathType.VolcanicSeafloor);
            available.Add(PathHelper.PathType.FloodedRuins);
        }

        return available[UnityEngine.Random.Range(0, available.Count)];
    }

    private PathHelper.PathEnvironment GetRandomEnvironment(int day, PathHelper.PathType type)
    {
        List<PathHelper.PathEnvironment> available = new();

        available.Add(PathHelper.PathEnvironment.MurkyWater);
        available.Add(PathHelper.PathEnvironment.FreshWater);
        available.Add(PathHelper.PathEnvironment.ClearWater);

        if (day > 50)
        {
            available.Add(PathHelper.PathEnvironment.ShallowLight);
            available.Add(PathHelper.PathEnvironment.HighCurrent);
            available.Add(PathHelper.PathEnvironment.HighSalinity);
        }

        if (day > 100)
        {
            available.Add(PathHelper.PathEnvironment.LowVisibility);
            available.Add(PathHelper.PathEnvironment.SedimentHeavy);
            available.Add(PathHelper.PathEnvironment.ProtectedZone);
            available.Add(PathHelper.PathEnvironment.AlgaeRich);
        }

        if (day > 250)
        {
            available.Add(PathHelper.PathEnvironment.Cold);
            available.Add(PathHelper.PathEnvironment.Overfished);
            available.Add(PathHelper.PathEnvironment.OilContaminated);
        }

        if (day > 500)
        {
            available.Add(PathHelper.PathEnvironment.Freezing);
            available.Add(PathHelper.PathEnvironment.Warm);
            available.Add(PathHelper.PathEnvironment.OxygenPoor);
            available.Add(PathHelper.PathEnvironment.Industrialized);
        }

        if (day > 1000)
        {
            available.Add(PathHelper.PathEnvironment.StormAffected);
            available.Add(PathHelper.PathEnvironment.DebrisFilled);
            available.Add(PathHelper.PathEnvironment.FogCovered);
        }


        if (type == PathHelper.PathType.VolcanicSeafloor)
        {
            available.Remove(PathHelper.PathEnvironment.Freezing);
        }

        if (type == PathHelper.PathType.IceEdge || type == PathHelper.PathType.GlacierRunoff)
        {
            available.Remove(PathHelper.PathEnvironment.Warm);
        }

        return available[UnityEngine.Random.Range(0, available.Count)];
    }

    private PathHelper.PathModifier GetRandomModifier(int day, PathHelper.PathType type)
    {
        List<PathHelper.PathModifier> available = new();

        available.Add(PathHelper.PathModifier.Normal);

        if (day > 50)
        {
            available.Add(PathHelper.PathModifier.Defensive);
            available.Add(PathHelper.PathModifier.Migratory);
            available.Add(PathHelper.PathModifier.Aggressive);
        }

        if (day > 100)
        {
            available.Add(PathHelper.PathModifier.Territorial);
            available.Add(PathHelper.PathModifier.FastSwarm);
            available.Add(PathHelper.PathModifier.Camouflaged);
            available.Add(PathHelper.PathModifier.Venomous);
        }

        if (day > 250)
        {
            available.Add(PathHelper.PathModifier.SonicSensitive);
            available.Add(PathHelper.PathModifier.Parasitic);
            available.Add(PathHelper.PathModifier.SurfaceAmbush);
            available.Add(PathHelper.PathModifier.Burrowers);
            available.Add(PathHelper.PathModifier.ArmoredShell);
        }

        if (day > 500)
        {
            available.Add(PathHelper.PathModifier.Bioluminescent);
            available.Add(PathHelper.PathModifier.ColdResistant);
            available.Add(PathHelper.PathModifier.ElectricDischarge);
            available.Add(PathHelper.PathModifier.HeatResistant);
            available.Add(PathHelper.PathModifier.HighPressureAdapted);
        }

        if (day > 1000)
        {
            available.Add(PathHelper.PathModifier.ApexPredators);
            available.Add(PathHelper.PathModifier.Regenerative);
            available.Add(PathHelper.PathModifier.ScavengerDominant);
        }

        return available[UnityEngine.Random.Range(0, available.Count)];
    }


    // Events
    private void OnEnable()
    {
        ExpeditionEvents.OnDestinationArrival += GenerateNextDestination;
    }

    private void OnDisable()
    {
        ExpeditionEvents.OnDestinationArrival -= GenerateNextDestination;
    }
}
