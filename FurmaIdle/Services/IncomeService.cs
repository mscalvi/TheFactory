using System.Collections.Concurrent;
using FurmaIdle.Helpers;
using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public interface IIncomeService
    {
        Task<GainModel> AddAsync(ItemHelper.ItemType type, string itemId, double amount);
        long? AddAmount { get; }
    }

    public sealed class IncomeService : IIncomeService
    {
        private readonly ICurrentGameService _game;

        private readonly ConcurrentDictionary<string, double> _fractions = new();

        public long? AddAmount { get; private set; } = 0;

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

            var rest = 0;
            var total = rest + amount;

            var gain = (long)Math.Floor(total);
            var frac = total - gain;

            GainModel? result = null;

            await _game.Mutate(Game =>
            {
                StatsEnsure(Game);

                if (gain != 0)
                {
                    if (!StatsApply(Game, type, itemId, gain, frac))
                    {
                        Console.WriteLine($"[Income] Falha ao aplicar ganho: type={type} id={itemId} eff={gain}");
                        gain = 0;
                    }
                }

                result = new GainModel
                {
                    ItemId = itemId,
                    ItemType = type,
                    GainEffective = (int)Math.Clamp(gain, int.MinValue, int.MaxValue),
                    GainTotal = amount,
                    GainFraction = frac
                };

                AddAmount = result.GainEffective;
            }, save: gain != 0);

            Console.WriteLine($"[Income] Sucesso ao aplicar ganho: type={type} id={itemId} eff={gain}");
            return result!;
        }

        private static void StatsEnsure(GameModel g)
        {
            g.Stats ??= new StatsModel();

            g.Stats.Coins ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.Resources ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.Knowledge ??= new Dictionary<string, long>(StringComparer.Ordinal);

            g.Stats.CoinsFrac ??= new Dictionary<string, double>(StringComparer.Ordinal);
            g.Stats.ResourcesFrac ??= new Dictionary<string, double>(StringComparer.Ordinal);
            g.Stats.KnowledgeFrac ??= new Dictionary<string, double>(StringComparer.Ordinal);

            g.Stats.CoinsGain ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.ResourcesGain ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.KnowledgeGain ??= new Dictionary<string, long>(StringComparer.Ordinal);

            g.Stats.CoinsSpent ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.ResourcesSpent ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.KnowledgeSpent ??= new Dictionary<string, long>(StringComparer.Ordinal);
        }

        private static bool StatsApply(GameModel Game, ItemHelper.ItemType type, string id, long gain, double frac)
        {
            if (type == ItemHelper.ItemType.Coin)
            {
                var extra = 0L;

                Game.Stats.CoinsFrac.TryGetValue(id, out var rest);
                Console.WriteLine($"Moeda: {id}. Fração: {rest}");
                rest = rest + frac;

                if (rest >= 1)
                {
                    rest = rest - 1;
                    extra = 1;
                }

                Game.Stats.Coins.TryGetValue(id, out var coin);
                Console.WriteLine($"Moeda: {id}. Total: {coin}");
                coin = coin + gain + extra;

                Game.Stats.CoinsGain.TryGetValue(id, out var coinarch);
                Console.WriteLine($"Moeda: {id}. Legado: {coinarch}");
                coinarch = coinarch + gain + extra;

                Game.Stats.CoinsFrac[id] = rest;
                Game.Stats.Coins[id] = coin;
                Game.Stats.CoinsGain[id] = coinarch;
            }

            return true;
        }
    }
}
