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
        private readonly IUiLogService _log;

        public IncomeService(ICurrentGameService game, IUiLogService log)
        {
            _game = game;
            _log = log;
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
                    Game.ExpeditionStats.ClicksMade.TryGetValue(sourceId, out var prevExp);
                    Game.ExpeditionStats.ClicksMade[sourceId] = prevExp + 1;

                    Game.ExpansionStats.ClicksMade.TryGetValue(sourceId, out var prevExpa);
                    Game.ExpansionStats.ClicksMade[sourceId] = prevExpa + 1;

                    Game.GameStats.ClicksMade.TryGetValue(sourceId, out var prevGame);
                    Game.GameStats.ClicksMade[sourceId] = prevGame + 1;
                }

                result = new GainModel
                {
                    ItemId = itemId,
                    ItemType = type,
                    GainEffective = (int)Math.Clamp(gain, int.MinValue, int.MaxValue),
                    GainTotal = amount,
                    GainFraction = frac
                };
            }, save: false);

            return result!;
        }

        private bool ApplyStats(GameModel Game, ItemHelper.ItemType type, string id, long gain, double frac)
        {
            if (type == ItemType.Coin)
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

                Game.ExpeditionStats.CoinsGain.TryGetValue(id, out var coinExpe);
                coinExpe = coinExpe + gain + extra;

                // ---- persistir ----
                Game.ExpeditionStats.Coins[id] = coin;
                Game.ExpeditionStats.CoinsGain[id] = coinExpe;
                Game.ExpeditionStats.CoinsFrac[id] = newRestDouble;

                Game.ExpansionStats.CoinsGain.TryGetValue(id, out var coinExpa);
                coinExpa = coinExpa + gain + extra;
                Game.ExpansionStats.CoinsGain[id] = coinExpa;

                Game.GameStats.CoinsGain.TryGetValue(id, out var coinGame);
                coinGame = coinGame + gain + extra;
                Game.GameStats.CoinsGain[id] = coinGame;
            }
            if (type == ItemType.Resource)
            {
                Game.ExpeditionStats.ResourcesFrac.TryGetValue(id, out var restDouble);
                int restCents = (int)Math.Round(restDouble * 100, MidpointRounding.AwayFromZero);

                int addCents = (int)Math.Round(frac * 100, MidpointRounding.AwayFromZero);
                int totalCents = restCents + addCents;

                long extra = totalCents / 100;          // carry em unidades inteiras
                int newRestCents = totalCents % 100;    // 0..99

                double newRestDouble = newRestCents / 100.0;

                // ---- acumula ----
                Game.ExpeditionStats.Resources.TryGetValue(id, out var coin);
                coin = coin + gain + extra;

                Game.ExpeditionStats.ResourcesGain.TryGetValue(id, out var coinExpe);
                coinExpe = coinExpe + gain + extra;

                // ---- persistir ----
                Game.ExpeditionStats.Resources[id] = coin;
                Game.ExpeditionStats.ResourcesGain[id] = coinExpe;
                Game.ExpeditionStats.ResourcesFrac[id] = newRestDouble;

                Game.ExpansionStats.ResourcesGain.TryGetValue(id, out var coinExpa);
                coinExpa = coinExpa + gain + extra;
                Game.ExpansionStats.ResourcesGain[id] = coinExpa;

                Game.GameStats.ResourcesGain.TryGetValue(id, out var coinGame);
                coinGame = coinGame + gain + extra;
                Game.GameStats.ResourcesGain[id] = coinGame;
            }
            if (type == ItemType.Knowledge)
            {
                // ---- acumula moedas ----
                Game.ExpeditionStats.Knowledge.TryGetValue(id, out var coin);
                coin = coin + gain;

                Game.ExpeditionStats.KnowledgeGain.TryGetValue(id, out var coinExpe);
                coinExpe = coinExpe + gain;

                // ---- persistir ----
                Game.ExpeditionStats.Knowledge[id] = coin;
                Game.ExpeditionStats.KnowledgeGain[id] = coinExpe;

                Game.ExpansionStats.KnowledgeGain.TryGetValue(id, out var coinExpa);
                coinExpa = coinExpa + gain;
                Game.ExpansionStats.KnowledgeGain[id] = coinExpa;

                Game.GameStats.KnowledgeGain.TryGetValue(id, out var coinGame);
                coinGame = coinGame + gain;
                Game.GameStats.KnowledgeGain[id] = coinGame;
            }

            return true;
        }
    }
}
