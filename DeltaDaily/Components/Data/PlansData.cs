// Arquivo: Components/Data/PlansData.cs
namespace DeltaDaily.Components.Data;

public static class PlansData
{
    // Conjunto com comparação case-insensitive para evitar duplicatas "Plano A" vs "plano a"
    private static readonly HashSet<string> _plans =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Plano A", "Plano B", "Plano C"
        };

    // Notificação opcional: componentes podem assinar para atualizar UI quando a lista mudar
    public static event Action? Changed;

    /// <summary>
    /// Retorna a lista de planos ordenada alfabeticamente.
    /// </summary>
    public static IReadOnlyList<string> All =>
        _plans.OrderBy(p => p, StringComparer.CurrentCultureIgnoreCase).ToList();

    /// <summary>
    /// Tenta adicionar um plano. Retorna true se incluiu; false se já existia ou nome inválido.
    /// </summary>
    public static bool Add(string? name)
    {
        var n = Normalize(name);
        if (string.IsNullOrEmpty(n)) return false;

        var added = _plans.Add(n);
        if (added) Changed?.Invoke();
        return added;
    }

    /// <summary>
    /// Tenta remover um plano existente.
    /// </summary>
    public static bool Remove(string? name)
    {
        var n = Normalize(name);
        if (string.IsNullOrEmpty(n)) return false;

        var removed = _plans.Remove(n);
        if (removed) Changed?.Invoke();
        return removed;
    }

    /// <summary>
    /// Verifica existência (case-insensitive).
    /// </summary>
    public static bool Exists(string? name)
    {
        var n = Normalize(name);
        return !string.IsNullOrEmpty(n) && _plans.Contains(n);
    }

    private static string Normalize(string? s) => (s ?? "").Trim();
}
