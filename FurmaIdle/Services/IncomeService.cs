using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Collections.Concurrent;
using static FurmaIdle.Helpers.ItemHelper;

namespace FurmaIdle.Services
{
    public interface IIncomeService
    {
        Task<GainModel> AddAsync(ItemHelper.ItemType type, string itemId, double amount, ItemHelper.ItemType? sourceType, string sourceId, string stageId);
    }

    public sealed class IncomeService : IIncomeService
    {
        private readonly ICurrentGameService _game;
        private readonly IUiLogService _log;
        private readonly ILocateService _locate;

        public IncomeService(ICurrentGameService game, IUiLogService log, ILocateService locate)
        {
            _game = game;
            _log = log;
            _locate = locate;
        }

        public async Task<GainModel> AddAsync(ItemHelper.ItemType type, string itemId, double amount, ItemHelper.ItemType? sourceType, string? sourceId, string stageId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                throw new ArgumentException("itemId inválido.", nameof(itemId));
            if (double.IsNaN(amount) || double.IsInfinity(amount))
                throw new ArgumentOutOfRangeException(nameof(amount), "amount inválido");

            var gain = (long)Math.Floor(amount);
            var frac = amount - gain;
            var game = _game.CurrentGame;
            var stage = _locate.LocateStage(game, stageId);
            var expansion = _locate.LocateExpansion(game, game.CurrentExpansionId);

            GainModel ? result = null;

            var saveFrac = Math.Round(frac * 100, MidpointRounding.AwayFromZero) != 0;
            await _game.Mutate(game =>
            {
                ApplyStats(game, type, itemId, gain, frac, stage);

                if (sourceType == ItemHelper.ItemType.Click)
                {
                    if (stage.Expedition.ExpeditionState == UnlockHelper.ExpeditionState.Active)
                    {
                        stage.ExpeditionStats.ClicksMade.TryGetValue(sourceId, out var prevExp);
                        stage.ExpeditionStats.ClicksMade[sourceId] = prevExp + 1;
                    }
                    else
                    {
                        game.NoExpeditionStats.ClicksMade.TryGetValue(sourceId, out var prevExp);
                        game.NoExpeditionStats.ClicksMade[sourceId] = prevExp + 1;
                    }

                    expansion.ExpansionStats.ClicksMade.TryGetValue(sourceId, out var prevExpa);
                    expansion.ExpansionStats.ClicksMade[sourceId] = prevExpa + 1;

                    game.GameStats.ClicksMade.TryGetValue(sourceId, out var prevGame);
                    game.GameStats.ClicksMade[sourceId] = prevGame + 1;
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

        private void ApplyStats(GameModel game, ItemHelper.ItemType type, string id, long gain, double frac, StageModel stage)
        {
            var expansion = _locate.LocateExpansion(game, game.CurrentExpansionId);
            if (type == ItemType.Coin)
            {
                long extra = 0;
                // ---- trabalhar em centavos (0..99) ----
                stage.ExpeditionStats.CoinsFrac.TryGetValue(id, out var restDouble);
                int restCents = (int)Math.Round(restDouble * 100, MidpointRounding.AwayFromZero);

                int addCents = (int)Math.Round(frac * 100, MidpointRounding.AwayFromZero);
                int totalCents = restCents + addCents;

                extra = totalCents / 100;          // carry em unidades inteiras
                int newRestCents = totalCents % 100;    // 0..99

                double newRestDouble = newRestCents / 100.0;

                // ---- acumula moedas ----
                stage.ExpeditionStats.Coins.TryGetValue(id, out var coin);
                coin = coin + gain + extra;

                stage.ExpeditionStats.CoinsGain.TryGetValue(id, out var coinExpe);
                coinExpe = coinExpe + gain + extra;

                // ---- persistir ----
                stage.ExpeditionStats.Coins[id] = coin;
                stage.ExpeditionStats.CoinsGain[id] = coinExpe;
                stage.ExpeditionStats.CoinsFrac[id] = newRestDouble;

                expansion.ExpansionStats.Coins.TryGetValue(id, out var allCoins);
                allCoins = allCoins + gain + extra;
                expansion.ExpansionStats.Coins[id] = allCoins;
                expansion.ExpansionStats.CoinsGain.TryGetValue(id, out var coinExpa);
                coinExpa = coinExpa + gain + extra;
                expansion.ExpansionStats.CoinsGain[id] = coinExpa;

                game.GameStats.CoinsGain.TryGetValue(id, out var coinGame);
                coinGame = coinGame + gain + extra;
                game.GameStats.CoinsGain[id] = coinGame;
            }
            if (type == ItemType.Resource)
            {
                expansion.ExpansionStats.ResourcesFrac.TryGetValue(id, out var restDouble);
                int restCents = (int)Math.Round(restDouble * 100, MidpointRounding.AwayFromZero);

                int addCents = (int)Math.Round(frac * 100, MidpointRounding.AwayFromZero);
                int totalCents = restCents + addCents;

                long extra = totalCents / 100;          // carry em unidades inteiras
                int newRestCents = totalCents % 100;    // 0..99

                double newRestDouble = newRestCents / 100.0;

                // ---- acumula ----
                expansion.ExpansionStats.Resources.TryGetValue(id, out var coin);
                coin = coin + gain + extra;

                expansion.ExpansionStats.ResourcesGain.TryGetValue(id, out var coinExpe);
                coinExpe = coinExpe + gain + extra;

                // ---- persistir ----
                expansion.ExpansionStats.Resources[id] = coin;
                expansion.ExpansionStats.ResourcesGain[id] = coinExpe;
                expansion.ExpansionStats.ResourcesFrac[id] = newRestDouble;

                game.GameStats.ResourcesGain.TryGetValue(id, out var coinGame);
                coinGame = coinGame + gain + extra;
                game.GameStats.ResourcesGain[id] = coinGame;
            }
            if (type == ItemType.Knowledge)
            {
                expansion.ExpansionStats.Knowledge.TryGetValue(id, out var know);
                know = know + gain;
                expansion.ExpansionStats.Knowledge[id] = know;

                expansion.ExpansionStats.KnowledgeGain.TryGetValue(id, out var knowExpa);
                knowExpa = knowExpa + gain;
                expansion.ExpansionStats.KnowledgeGain[id] = knowExpa;

                game.GameStats.KnowledgeGain.TryGetValue(id, out var knowGame);
                knowGame = knowGame + gain;
                game.GameStats.KnowledgeGain[id] = knowGame;
            }
        }
    }
}
