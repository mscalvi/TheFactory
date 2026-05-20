using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
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

    public static string GetPathTypeName(PathType type, GameState.Language language)
    {
        switch (language)
        {
            case GameState.Language.Portugues:
                switch (type)
                {
                    case PathType.CurrentEntrilhas:
                        return "Entrilhas";
                    case PathType.UrbanRiver:
                        return "Rio Urbano";
                    case PathType.ForestRiver:
                        return "Rio Florestal";
                    case PathType.OpenOcean:
                        return "Oceano Aberto";
                    case PathType.CoastalShelf:
                        return "Plataforma Costeira";
                    case PathType.DeepSea:
                        return "Mar Profundo";
                    case PathType.AbyssalPlain:
                        return "Planície Abissal";
                    case PathType.CoralReef:
                        return "Recife de Coral";
                    case PathType.KelpForest:
                        return "Floresta de Algas";
                    case PathType.Mangrove:
                        return "Maguezal";
                    case PathType.Estuary:
                        return "Estuário";
                    case PathType.RiverDelta:
                        return "Delta de Rio";
                    case PathType.FreshwaterRiver:
                        return "Rio Caudaloso";
                    case PathType.CanyonWaters:
                        return "Canyon Submerso";
                    case PathType.UnderwaterRidge:
                        return "Riacho Submerso";
                    case PathType.IceEdge:
                        return "Borda Glacial";
                    case PathType.GlacierRunoff:
                        return "Derretimento Glacial";
                    case PathType.VolcanicSeafloor:
                        return "Fundo Vulcânico";
                    case PathType.ThermalVentField:
                        return "Campo de Fumarolas";
                    case PathType.SaltFlatWaters:
                        return "Águas Salinas Rasas";
                    case PathType.Lagoon:
                        return "Lagoa";
                    case PathType.InlandSea:
                        return "Mar Interior";
                    case PathType.FloodedRuins:
                        return "Ruínas Submersas";

                    default:
                        return type.ToString();
                }

            case GameState.Language.English:
                switch (type)
                {
                    case PathType.CurrentEntrilhas:
                        return "Entrilhas";
                    case PathType.UrbanRiver:
                        return "Urban River";
                    case PathType.ForestRiver:
                        return "Forest River";
                    case PathType.OpenOcean:
                        return "Open Ocean";
                    case PathType.CoastalShelf:
                        return "Coastal Shelf";
                    case PathType.DeepSea:
                        return "Mar Profundo";
                    case PathType.AbyssalPlain:
                        return "Abyssal Plain";
                    case PathType.CoralReef:
                        return "Recife de Coral";
                    case PathType.KelpForest:
                        return "Kelp Forest";
                    case PathType.Mangrove:
                        return "Mangrove";
                    case PathType.Estuary:
                        return "Estuary";
                    case PathType.RiverDelta:
                        return "River Delta";
                    case PathType.FreshwaterRiver:
                        return "Freshwater River";
                    case PathType.CanyonWaters:
                        return "Underwater Canyon";
                    case PathType.UnderwaterRidge:
                        return "Underwater Ridge";
                    case PathType.IceEdge:
                        return "Ice Edge";
                    case PathType.GlacierRunoff:
                        return "Glacier Runoff";
                    case PathType.VolcanicSeafloor:
                        return "Volcanic Seafloor";
                    case PathType.ThermalVentField:
                        return "Thermal Vent Field";
                    case PathType.SaltFlatWaters:
                        return "Salt Flat Waters";
                    case PathType.Lagoon:
                        return "Lagoon";
                    case PathType.InlandSea:
                        return "Island Sea";
                    case PathType.FloodedRuins:
                        return "Flooded Ruins";

                    default:
                        return type.ToString();
                }

            default:
                return type.ToString();
        }
    }
    public static string GetEnvironmentName(PathEnvironment type, GameState.Language language)
    {
        switch (language)
        {
            case GameState.Language.Portugues:
                switch (type)
                {
                    case PathEnvironment.ClearWater:
                        return "Águas Claras";
                    case PathEnvironment.MurkyWater:
                        return "Águas Turvas";
                    case PathEnvironment.SedimentHeavy:
                        return "Carregada de Sedmentos";
                    case PathEnvironment.AlgaeRich:
                        return "Rica em Álguas";
                    case PathEnvironment.Cold:
                        return "Fria";
                    case PathEnvironment.Freezing:
                        return "Congelante";
                    case PathEnvironment.Warm:
                        return "Quente";
                    case PathEnvironment.OxygenPoor:
                        return "Baixo Oxigênio";
                    case PathEnvironment.HighSalinity:
                        return "Alta Salinidade";
                    case PathEnvironment.FreshWater:
                        return "Água Limpa";
                    case PathEnvironment.StormAffected:
                        return "Região de Tempestades";
                    case PathEnvironment.FogCovered:
                        return "Coberta por Névoa";
                    case PathEnvironment.OilContaminated:
                        return "Contaminada por Óleo";
                    case PathEnvironment.DebrisFilled:
                        return "Cheia de Destroços";
                    case PathEnvironment.Industrialized:
                        return "Zona Industrial";
                    case PathEnvironment.Overfished:
                        return "Sobrepescada";
                    case PathEnvironment.ProtectedZone:
                        return "Zona de Proteção";
                    case PathEnvironment.HighCurrent:
                        return "Correntes Fortes";
                    case PathEnvironment.LowVisibility:
                        return "Baixa Visbilidade";
                    case PathEnvironment.ShallowLight:
                        return "Iluminação Rasa";

                    default:
                        return type.ToString();
                }

            case GameState.Language.English:
                switch (type)
                {
                    case PathEnvironment.ClearWater:
                        return "Clear Water";
                    case PathEnvironment.MurkyWater:
                        return "Murky Water";
                    case PathEnvironment.SedimentHeavy:
                        return "Sediment Heavy";
                    case PathEnvironment.AlgaeRich:
                        return "Algae Rich";
                    case PathEnvironment.Cold:
                        return "Cold";
                    case PathEnvironment.Freezing:
                        return "Freezing";
                    case PathEnvironment.Warm:
                        return "Warm";
                    case PathEnvironment.OxygenPoor:
                        return "Oxygen Poor";
                    case PathEnvironment.HighSalinity:
                        return "High Salinity";
                    case PathEnvironment.FreshWater:
                        return "Fresh Water";
                    case PathEnvironment.StormAffected:
                        return "Storm Affected";
                    case PathEnvironment.FogCovered:
                        return "Fog Covered";
                    case PathEnvironment.OilContaminated:
                        return "Oil Contaminated";
                    case PathEnvironment.DebrisFilled:
                        return "Debris Filled";
                    case PathEnvironment.Industrialized:
                        return "Industrialized Zone";
                    case PathEnvironment.Overfished:
                        return "Overfished";
                    case PathEnvironment.ProtectedZone:
                        return "Protected Zone";
                    case PathEnvironment.HighCurrent:
                        return "High Current";
                    case PathEnvironment.LowVisibility:
                        return "Low Visibility";
                    case PathEnvironment.ShallowLight:
                        return "Shallow Light";

                    default:
                        return type.ToString();
                }

            default:
                return type.ToString();
        }
    }
    public static string GetModifierName(PathModifier type, GameState.Language language)
    {
        switch (language)
        {
            case GameState.Language.Portugues:
                switch (type)
                {
                    case PathModifier.Normal:
                        return "Normal";
                    case PathModifier.ElectricDischarge:
                        return "Descarga Elétrica";
                    case PathModifier.Bioluminescent:
                        return "Bioluminescente";
                    case PathModifier.ArmoredShell:
                        return "Carapaça Reforçada";
                    case PathModifier.FastSwarm:
                        return "Enxames Rápidos";
                    case PathModifier.ApexPredators:
                        return "Predadores de Topo";
                    case PathModifier.Camouflaged:
                        return "Camuflagem Natural";
                    case PathModifier.Venomous:
                        return "Venenosos";
                    case PathModifier.Parasitic:
                        return "Parasitas";
                    case PathModifier.Regenerative:
                        return "Regenerativos";
                    case PathModifier.SurfaceAmbush:
                        return "Emboscada de Superfície";
                    case PathModifier.HighPressureAdapted:
                        return "Adaptados à Pressão";
                    case PathModifier.Burrowers:
                        return "Escavadores";
                    case PathModifier.Territorial:
                        return "Territorialistas";
                    case PathModifier.Migratory:
                        return "Migratórios";
                    case PathModifier.Aggressive:
                        return "Extremamente Agressivos";
                    case PathModifier.Defensive:
                        return "Defensivos";
                    case PathModifier.SonicSensitive:
                        return "Sensíveis a Vibração";
                    case PathModifier.HeatResistant:
                        return "Resistentes ao Calor";
                    case PathModifier.ColdResistant:
                        return "Resistentes ao Frio";
                    case PathModifier.ScavengerDominant:
                        return "Dominância de Necrófagos";
        
                    default:
                        return type.ToString();
                }

            case GameState.Language.English:
                switch (type)
                {
                    case PathModifier.Normal:
                        return "Normal";
                    case PathModifier.ElectricDischarge:
                        return "Electric Discharge";
                    case PathModifier.Bioluminescent:
                        return "Bioluminescent";
                    case PathModifier.ArmoredShell:
                        return "Armored Shell";
                    case PathModifier.FastSwarm:
                        return "Fast Swarm";
                    case PathModifier.ApexPredators:
                        return "Apex Predators";
                    case PathModifier.Camouflaged:
                        return "Camouflaged";
                    case PathModifier.Venomous:
                        return "Venomous";
                    case PathModifier.Parasitic:
                        return "Parasitic";
                    case PathModifier.Regenerative:
                        return "Regenerative";
                    case PathModifier.SurfaceAmbush:
                        return "Surface Ambush";
                    case PathModifier.HighPressureAdapted:
                        return "High Pressure Adapted";
                    case PathModifier.Burrowers:
                        return "Burrowers";
                    case PathModifier.Territorial:
                        return "Territorial";
                    case PathModifier.Migratory:
                        return "Migratory";
                    case PathModifier.Aggressive:
                        return "Agressive";
                    case PathModifier.Defensive:
                        return "Defensive";
                    case PathModifier.SonicSensitive:
                        return "Sonic Sensitive";
                    case PathModifier.HeatResistant:
                        return "Heat Resistant";
                    case PathModifier.ColdResistant:
                        return "Cold Resistant";
                    case PathModifier.ScavengerDominant:
                        return "Scavenger Dominant";

                    default:
                        return type.ToString();
                }

            default:
                return type.ToString();
        }
    }
}
