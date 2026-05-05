using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHelper
{
    public struct PathTagSet
    {
        public PathType Type;
        public PathEnvironment? Environment;
        public PathModifier? Modifier;
    }

    [Flags]
    public enum PathType
    {
        None = 0,

        CurrentEntrilhas = 1 << 0,
        UrbanRiver = 1 << 1,
        ForestRiver = 1 << 2,
        OpenOcean = 1 << 3,
        CoastalShelf = 1 << 4,
        DeepSea = 1 << 5,
        AbyssalPlain = 1 << 6,
        CoralReef = 1 << 7,
        KelpForest = 1 << 8,
        Mangrove = 1 << 9,
        Estuary = 1 << 10,
        RiverDelta = 1 << 11,
        FreshwaterRiver = 1 << 12,
        CanyonWaters = 1 << 13,
        UnderwaterRidge = 1 << 14,
        IceEdge = 1 << 15,
        GlacierRunoff = 1 << 16,
        VolcanicSeafloor = 1 << 17,
        ThermalVentField = 1 << 18,
        SaltFlatWaters = 1 << 19,
        Lagoon = 1 << 20,
        InlandSea = 1 << 21,
        FloodedRuins = 1 << 22
    }

    public enum PathEnvironment
    {
        ClearWater,
        MurkyWater,
        SedimentHeavy,
        AlgaeRich,
        Cold,
        Freezing,
        Warm,
        OxygenPoor,
        HighSalinity,
        FreshWater,
        StormAffected,
        FogCovered,
        OilContaminated,
        DebrisFilled,
        Industrialized,
        Overfished,
        ProtectedZone,
        HighCurrent,
        LowVisibility,
        ShallowLight
    }

    public enum PathModifier
    {
        Normal,
        ElectricDischarge,
        Bioluminescent,
        ArmoredShell,
        FastSwarm,
        ApexPredators,
        Camouflaged,
        Venomous,
        Parasitic,
        Regenerative,
        HighPressureAdapted,
        SurfaceAmbush,
        Burrowers,
        Territorial,
        Migratory,
        Aggressive,
        Defensive,
        SonicSensitive,
        HeatResistant,
        ColdResistant,
        ScavengerDominant
    }

    public static class PathTypeGroups
    {
        public const PathHelper.PathType AllRivers =
            PathHelper.PathType.UrbanRiver |
            PathHelper.PathType.ForestRiver |
            PathHelper.PathType.FreshwaterRiver |
            PathHelper.PathType.RiverDelta;

        public const PathHelper.PathType AllOcean =
            PathHelper.PathType.OpenOcean |
            PathHelper.PathType.CoastalShelf |
            PathHelper.PathType.DeepSea |
            PathHelper.PathType.AbyssalPlain |
            PathHelper.PathType.InlandSea;

        public const PathHelper.PathType ShallowWaters =
            PathHelper.PathType.CoastalShelf |
            PathHelper.PathType.CoralReef |
            PathHelper.PathType.Lagoon |
            PathHelper.PathType.Mangrove;

        public const PathHelper.PathType DeepWaters =
            PathHelper.PathType.DeepSea |
            PathHelper.PathType.AbyssalPlain |
            PathHelper.PathType.CanyonWaters |
            PathHelper.PathType.UnderwaterRidge;

        public const PathHelper.PathType ColdRegions =
            PathHelper.PathType.IceEdge |
            PathHelper.PathType.GlacierRunoff;

        public const PathHelper.PathType VolcanicRegions =
            PathHelper.PathType.VolcanicSeafloor |
            PathHelper.PathType.ThermalVentField;

        public const PathHelper.PathType VegetationDense =
            PathHelper.PathType.KelpForest |
            PathHelper.PathType.Mangrove |
            PathHelper.PathType.CoralReef;

        public const PathHelper.PathType TransitionalZones =
            PathHelper.PathType.Estuary |
            PathHelper.PathType.RiverDelta |
            PathHelper.PathType.CoastalShelf;

        public const PathHelper.PathType HazardZones =
            PathHelper.PathType.VolcanicSeafloor |
            PathHelper.PathType.ThermalVentField |
            PathHelper.PathType.CanyonWaters;

        public const PathHelper.PathType ArtificialZones =
            PathHelper.PathType.UrbanRiver |
            PathHelper.PathType.FloodedRuins;
    }
}
