using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace EscalaFacil.Components.Services;

public sealed class JsonDataStore : IDataStore
{
    private readonly string _root;
    private readonly JsonSerializerOptions _json;

    public JsonDataStore(string? root = null, JsonSerializerOptions? jsonOptions = null)
    {
        _root = root ?? Path.Combine(FileSystem.AppDataDirectory, "EscalaFacil");
        _json = jsonOptions ?? new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        Directory.CreateDirectory(_root);
    }

    public async Task SaveAsync<T>(string key, T value, CancellationToken ct = default)
    {
        var path = KeyToPath(key);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(fs, value, _json, ct);
        await fs.FlushAsync(ct);
    }

    public async Task<T?> LoadAsync<T>(string key, CancellationToken ct = default)
    {
        var path = KeyToPath(key);
        if (!File.Exists(path)) return default;

        await using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return await JsonSerializer.DeserializeAsync<T>(fs, _json, ct);
    }

    public Task<bool> DeleteAsync(string key, CancellationToken ct = default)
    {
        var path = KeyToPath(key);
        if (File.Exists(path))
        {
            File.Delete(path);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        => Task.FromResult(File.Exists(KeyToPath(key)));

    public IEnumerable<string> EnumerateKeys(string? prefix = null)
    {
        if (!Directory.Exists(_root)) yield break;

        var rootLen = _root.Length + 1;
        foreach (var file in Directory.EnumerateFiles(_root, "*.json", SearchOption.AllDirectories))
        {
            var rel = file.Substring(rootLen);
            var key = rel.Replace(Path.DirectorySeparatorChar, '/');
            key = key.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                ? key[..^5]
                : key;

            if (string.IsNullOrEmpty(prefix) || key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                yield return key;
        }
    }

    public string GetPath(string key) => KeyToPath(key);

    private string KeyToPath(string key)
    {
        key = key.Replace('\\', '/');
        var parts = key.Split('/', StringSplitOptions.RemoveEmptyEntries)
                       .Select(SanitizePart)
                       .ToArray();
        var path = Path.Combine(new[] { _root }.Concat(parts).ToArray());
        return Path.HasExtension(path) ? path : path + ".json";
    }

    private static string SanitizePart(string s)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            s = s.Replace(c, '_');
        return s;
    }
}
