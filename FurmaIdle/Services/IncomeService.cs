using System.Collections.Concurrent;
using FurmaIdle.Helpers;
using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public interface IIncomeService
    {
        Task<GainModel> AddAsync(ItemHelper.ItemType type, string itemId, double amount);
    }

    public sealed class IncomeService : IIncomeService
    {
        private readonly ICurrentGameService _game;

        private readonly ConcurrentDictionary<string, double> _fractions = new();

        public IncomeService(ICurrentGameService game)
        {
            _game = game;
        }

        public async Task<GainModel> AddAsync(ItemHelper.ItemType type, string itemId, double amount)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                throw new ArgumentException("itemId inválido.", nameof(itemId));
            if (double.IsNaN(amount) || double.IsInfinity(amount))
                throw new ArgumentOutOfRangeException(nameof(amount), "amount inválido");

            var key = StatsKey(type, itemId);

            var startFrac = _fractions.GetOrAdd(key, 0.0);
            var total = startFrac + amount;

            var gain = (long)Math.Floor(total);
            var frac = total - gain;

            GainModel? result = null;

            await _game.Mutate(Game =>
            {
                StatsEnsure(Game);

                if (gain != 0)
                {
                    if (!StatsApply(Game, type, itemId, gain))
                    {
                        Console.WriteLine("[Income] Falha ao aplicar ganho: type={Type} id={Id} eff={Eff}", type, itemId, gain);
                        gain = 0;
                        frac = startFrac;
                    }
                }

                _fractions[key] = frac;

                result = new GainModel
                {
                    ItemId = itemId,
                    ItemType = type,
                    GainEffective = (int)Math.Clamp(gain, int.MinValue, int.MaxValue),
                    GainTotal = amount,
                    GainFraction = frac
                };
            }, save: gain != 0);

            return result!;
        }

        private static string StatsKey(ItemHelper.ItemType t, string id) => $"{(int)t}:{id}";

        private static void StatsEnsure(GameModel g)
        {
            g.Stats ??= new StatsModel();

            g.Stats.Coins ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.Resources ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.Knowledge ??= new Dictionary<string, long>(StringComparer.Ordinal);

            g.Stats.CoinsGain ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.ResourcesGain ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.KnowledgeGain ??= new Dictionary<string, long>(StringComparer.Ordinal);

            g.Stats.CoinsSpent ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.ResourcesSpent ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.KnowledgeSpent ??= new Dictionary<string, long>(StringComparer.Ordinal);
        }

        private static bool StatsApply(GameModel Game, ItemHelper.ItemType type, string id, long gain)
        {
            (Dictionary<string, long>? current, Dictionary<string, long>? lifetime) = type switch
            {
                ItemHelper.ItemType.Coin => (Game.Stats!.Coins, Game.Stats!.CoinsGain),
                ItemHelper.ItemType.Resource => (Game.Stats!.Resources, Game.Stats!.ResourcesGain),
                ItemHelper.ItemType.Knowledge => (Game.Stats!.Knowledge, Game.Stats!.KnowledgeGain),
                _ => (null, null),
            };

            if (current is null || lifetime is null) return false;

            current.TryGetValue(id, out var cur);
            current[id] = cur + gain;

            lifetime.TryGetValue(id, out var curGain);
            lifetime[id] = curGain + gain;

            return true;
        }
    }
}
