using GuildsLifeCounter.Models;

namespace GuildsLifeCounter.Services;

public class MatchService
{
    public MatchModel? Current { get; private set; }

    public event Action? Changed; // <- evento de mudança

    public void NewMatch(int players, int mode)
    {
        var m = new MatchModel { GameMode = mode, Players = new() };
        for (int i = 0; i < players; i++)
            m.Players.Add(new PlayerModel { Nome = $"Jogador {i + 1}" });
        Current = m;
        Notify();
    }

    public void Notify() => Changed?.Invoke(); // <- dispara re-render
}
