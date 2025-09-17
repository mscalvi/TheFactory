using GuildsLifeCounter.Models;
using System.Collections.Concurrent;

namespace GuildsLifeCounter.Services;

public class CharacterPoolService
{
    private readonly List<CharacterModel> _all;
    private readonly ConcurrentDictionary<string, byte> _taken = new(); // Ids reservados

    public CharacterPoolService()
    {
        // Pré-cadastrados (placeholders). Você troca pelos seus depois.
        _all = new()
        {
            new CharacterModel { Id="c01", Nome="Ari",   Lider=true,  Saude=12 },
            new CharacterModel { Id="c02", Nome="Bela",  Lider=false, Saude=10 },
            new CharacterModel { Id="c03", Nome="Ciro",  Lider=false, Saude=11 },
            new CharacterModel { Id="c04", Nome="Dara",  Lider=false, Saude=9  },
            new CharacterModel { Id="c05", Nome="Ena",   Lider=true,  Saude=13 },
            new CharacterModel { Id="c06", Nome="Faro",  Lider=false, Saude=10 },
            new CharacterModel { Id="c07", Nome="Gus",   Lider=false, Saude=8  },
            new CharacterModel { Id="c08", Nome="Hera",  Lider=false, Saude=10 },
            new CharacterModel { Id="c09", Nome="Ian",   Lider=false, Saude=10 },
            new CharacterModel { Id="c10", Nome="Juno",  Lider=false, Saude=12 },
        };
    }

    public IReadOnlyList<CharacterModel> GetAll() => _all;

    public CharacterModel? Find(string? id) =>
        string.IsNullOrWhiteSpace(id) ? null : _all.FirstOrDefault(c => c.Id == id);

    // Lista “disponível” = todos menos os já reservados
    public IReadOnlyList<CharacterModel> GetAvailable() =>
        _all.Where(c => !_taken.ContainsKey(c.Id)).ToList();

    public bool TryReserve(string id) => _taken.TryAdd(id, 1);
    public void Release(string? id)
    {
        if (!string.IsNullOrWhiteSpace(id))
            _taken.TryRemove(id, out _);
    }
}
