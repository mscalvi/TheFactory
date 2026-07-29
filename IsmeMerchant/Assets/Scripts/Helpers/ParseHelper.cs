using System;
using System.Collections.Generic;
using System.Globalization;

public static class ParseHelper
{
    public static int Int(string value)
    {
        return int.Parse(
            value.Trim(),
            CultureInfo.InvariantCulture
        );
    }

    public static float Float(string value)
    {
        return float.Parse(
            value.Trim(),
            CultureInfo.InvariantCulture
        );
    }

    public static double Double(string value)
    {
        return double.Parse(
            value.Trim(),
            CultureInfo.InvariantCulture
        );
    }

    public static bool Bool(string value)
    {
        return bool.Parse(value.Trim());
    }

    public static string String(string value)
    {
        return value.Trim();
    }

    // Single Enum
    public static T Enum<T>(string value) where T : struct
    {
        value = value.Trim();

        // Custom PathType Groups
        if (typeof(T) == typeof(PathHelper.PathType))
        {
            object result = value switch
            {
                "All" => PathHelper.PathTypeGroups.All,
                "AllRivers" => PathHelper.PathTypeGroups.AllRivers,
                "AllOcean" => PathHelper.PathTypeGroups.AllOcean,
                "ShallowWaters" => PathHelper.PathTypeGroups.ShallowWaters,
                "DeepWaters" => PathHelper.PathTypeGroups.DeepWaters,
                "ColdRegions" => PathHelper.PathTypeGroups.ColdRegions,
                "VolcanicRegions" => PathHelper.PathTypeGroups.VolcanicRegions,
                "VegetationDense" => PathHelper.PathTypeGroups.VegetationDense,
                "TransitionalZones" => PathHelper.PathTypeGroups.TransitionalZones,
                "HazardZones" => PathHelper.PathTypeGroups.HazardZones,
                "ArtificialZones" => PathHelper.PathTypeGroups.ArtificialZones,

                _ => System.Enum.Parse<T>(value, true)
            };

            return (T)result;
        }

        if (typeof(T) == typeof(PathHelper.PathEnvironment))
        {
            object result = value switch
            {
                "All" => PathHelper.PathEnvironmentGroups.All,
                _ => System.Enum.Parse<T>(value, true)
            };

            return (T)result;
        }

        if (typeof(T) == typeof(PathHelper.PathModifier))
        {
            object result = value switch
            {
                "All" => PathHelper.PathModifierGroups.All,
                _ => System.Enum.Parse<T>(value, true)
            };

            return (T)result;
        }

        return System.Enum.Parse<T>(
            value,
            true
        );
    }

    // List of normal enums
    public static List<T> EnumList<T>(string value) where T : struct
    {
        List<T> list = new();

        if (string.IsNullOrWhiteSpace(value))
            return list;

        string[] values = value.Split('|');

        foreach (var v in values)
        {
            list.Add(Enum<T>(v));
        }

        return list;
    }

    // Flags Enum Combination
    public static T EnumFlags<T>(string value) where T : struct
    {
        if (string.IsNullOrWhiteSpace(value))
            return default;

        long result = 0;

        string[] values = value.Split('|');

        foreach (var v in values)
        {
            var parsed = Enum<T>(v);

            result |= Convert.ToInt64(parsed);
        }

        return (T)System.Enum.ToObject(typeof(T), result);
    }
}