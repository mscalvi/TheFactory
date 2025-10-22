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

            var gain = (long)Math.Floor(amount);
            var frac = amount - gain;

            GainModel? result = null;

            var saveFrac = Math.Round(frac * 100, MidpointRounding.AwayFromZero) != 0;
            await _game.Mutate(Game =>
            {
                if (!StatsApply(Game, type, itemId, gain, frac))
                {
                    Console.WriteLine($"[Income] Falha ao aplicar ganho: type={type} id={itemId} eff={gain}");
                    gain = 0;
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
            }, save: gain != 0 || saveFrac);

            return result!;
        }

        private static bool StatsApply(GameModel Game, ItemHelper.ItemType type, string id, long gain, double frac)
        {
            if (type == ItemHelper.ItemType.Coin)
            {
                // ---- trabalhar em centavos (0..99) ----
                Game.ExpeditionStats.CoinsFrac.TryGetValue(id, out var restDouble);
                int restCents = (int)Math.Round(restDouble * 100, MidpointRounding.AwayFromZero);

                int addCents = (int)Math.Round(frac * 100, MidpointRounding.AwayFromZero);
                int totalCents = restCents + addCents;

                long extra = totalCents / 100;          // carry em unidades inteiras
                int newRestCents = totalCents % 100;    // 0..99

                double newRestDouble = newRestCents / 100.0;

                // ---- acumula moedas ----
                Game.ExpeditionStats.Coins.TryGetValue(id, out var coin);
                coin = coin + gain + extra;

                Game.ExpeditionStats.CoinsGain.TryGetValue(id, out var coinarch);
                coinarch = coinarch + gain + extra;

                // ---- persistir (resto com 2 casas) ----
                Game.ExpeditionStats.Coins[id] = coin;
                Game.ExpeditionStats.CoinsGain[id] = coinarch;
                Game.ExpeditionStats.CoinsFrac[id] = newRestDouble;

                Console.WriteLine($"[Income] Ganho: {gain+frac} {id}. Expedição: {coin + newRestDouble:F2} - Histórico: {coinarch}");
            }

            return true;
        }
    }
}
