using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EscalaFacil.Components.Services;

public interface IDataStore
{
    Task SaveAsync<T>(string key, T value, CancellationToken ct = default);
    Task<T?> LoadAsync<T>(string key, CancellationToken ct = default);
    Task<bool> DeleteAsync(string key, CancellationToken ct = default);
    Task<bool> ExistsAsync(string key, CancellationToken ct = default);
    IEnumerable<string> EnumerateKeys(string? prefix = null);
    string GetPath(string key);
}
