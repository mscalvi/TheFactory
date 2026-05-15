using System.Collections.Generic;
using UnityEngine;

public static class CSVLoaderService
{
    public static List<Dictionary<string, string>> Load(string path)
    {
        TextAsset file = Resources.Load<TextAsset>(path);

        List<Dictionary<string, string>> rows = new();

        string[] lines = file.text.Split('\n');

        string[] headers = lines[0]
            .Trim()
            .Replace("\uFEFF", "")
            .Split(';');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] values = line.Split(';');

            bool emptyRow = true;

            for (int j = 0; j < values.Length; j++)
            {
                if (!string.IsNullOrWhiteSpace(values[j]))
                {
                    emptyRow = false;
                    break;
                }
            }

            if (emptyRow)
                continue;

            Dictionary<string, string> row = new();

            for (int j = 0; j < headers.Length; j++)
            {
                if (j >= values.Length)
                    continue;

                row[headers[j]] = values[j];
            }

            rows.Add(row);
        }

        return rows;
    }
}