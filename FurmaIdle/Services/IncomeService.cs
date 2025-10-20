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

            await _game.Mutate(Game =>
            {
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
            }, save: gain != 0 || frac > 0);

            Console.WriteLine($"[Income] Ganho: Tipo = {type} ID = {itemId} Quantidade = {gain}");
            return result!;
        }

        private static bool StatsApply(GameModel Game, ItemHelper.ItemType type, string id, long gain, double frac)
        {
            if (type == ItemHelper.ItemType.Coin)
            {
                var extra = 0L;

                Game.ExpeditionStats.CoinsFrac.TryGetValue(id, out var rest);
                rest = rest + frac;

                if (rest >= 1)
                {
                    rest = rest - 1;
                    extra = 1;
                }

                Game.ExpeditionStats.Coins.TryGetValue(id, out var coin);
                coin = coin + gain + extra;

                Game.ExpeditionStats.CoinsGain.TryGetValue(id, out var coinarch);
                Console.WriteLine($"[Income] Moeda: {id}. Expedição: {coin}.{rest} - Histórico: {coinarch}");
                coinarch = coinarch + gain + extra;

                Game.ExpeditionStats.CoinsFrac[id] = rest;
                Game.ExpeditionStats.Coins[id] = coin;
                Game.ExpeditionStats.CoinsGain[id] = coinarch;
            }

            return true;
        }
    }
}
