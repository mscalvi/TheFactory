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

    public static T Enum<T>(string value) where T : struct
    {
        return System.Enum.Parse<T>(
            value.Trim(),
            true
        );
    }

    public static string String(string value)
    {
        return value.Trim();
    }

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
}