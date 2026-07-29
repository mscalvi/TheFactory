using UnityEngine;

public static class NumberHelper
{
    static readonly string[] suffixes =
    {
        "",
        "K",
        "M",
        "B",
        "T",
        "Qa",
        "Qi",
        "Sx",
        "Sp",
        "Oc",
        "No"
    };

    public static string Format(double value)
    {
        if (value < 1000)
            return value.ToString("N0");

        int suffixIndex = 0;

        while (value >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            value /= 1000f;
            suffixIndex++;
        }

        if (value >= 100)
            return value.ToString("N0") + suffixes[suffixIndex];

        if (value >= 10)
            return value.ToString("N1") + suffixes[suffixIndex];

        return value.ToString("N2") + suffixes[suffixIndex];
    }
}