using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Collections.Concurrent;
using static FurmaIdle.Helpers.ItemHelper;

namespace FurmaIdle.Services
{
    public interface IIncomeService
    {
        Task<GainModel> AddAsync(ItemHelper.ItemType type, string itemId, double amount, ItemHelper.ItemType? sourceType, string sourceId);
    }

    public sealed class IncomeService : IIncomeService
    {
        private readonly ICurrentGameService _game;

        public IncomeService(ICurrentGameService game)
        {
            _game = game;
        }

        public async Task<GainModel> AddAsync(ItemHelper.ItemType type, string itemId, double amount, ItemHelper.ItemType? sourceType, string? sourceId)
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
                if (!ApplyStats(Game, type, itemId, gain, frac))
                {
                    gain = 0;
                }

                if(sourceType == ItemHelper.ItemType.Click)
                {
                    Game.ExpeditionStats.ClicksMade.TryGetValue(sourceId, out var previous);

                    long clickCount = 1 + previous;

                    Game.ExpeditionStats.ClicksMade[sourceId] = clickCount;
                    Game.ExpansionStats.ClicksMade[sourceId] = clickCount;
                    Game.GameStats.ClicksMade[sourceId] = clickCount;
                }

                result = new GainModel
                {
                    ItemId = itemId,
                    ItemType = type,
                    GainEffective = (int)Math.Clamp(gain, int.MinValue, int.MaxValue),
                    GainTotal = amount,
                    GainFraction = frac
                };
            }, save: gain != 0 || saveFrac);

            return result!;
        }

        private bool ApplyStats(GameModel Game, ItemHelper.ItemType type, string id, long gain, double frac)
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

                Game.ExpansionStats.CoinsGain[id] = coin;
                Game.GameStats.CoinsGain[id] = coinarch;
            }

           return true;
        }
    }
}
