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
            var stage = _locate.LocateStage(_game.CurrentGame, stageId);

            GainModel ? result = null;

            var saveFrac = Math.Round(frac * 100, MidpointRounding.AwayFromZero) != 0;
            await _game.Mutate(Game =>
            {
                if (!ApplyStats(Game, type, itemId, gain, frac, stage))
                {
                    gain = 0;
                }

                if(sourceType == ItemHelper.ItemType.Click)
                {
                    stage.ExpeditionStats.ClicksMade.TryGetValue(sourceId, out var prevExp);
                    stage.ExpeditionStats.ClicksMade[sourceId] = prevExp + 1;

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

        private bool ApplyStats(GameModel Game, ItemHelper.ItemType type, string id, long gain, double frac, StageModel stage)
        {
            if (type == ItemType.Coin)
            {
                var modifier = GetKnowledgeModifiers(Game, id);
                gain = (long)Math.Floor(gain * modifier);
                frac = frac * modifier;

                // ---- trabalhar em centavos (0..99) ----
                stage.ExpeditionStats.CoinsFrac.TryGetValue(id, out var restDouble);
                int restCents = (int)Math.Round(restDouble * 100, MidpointRounding.AwayFromZero);

                int addCents = (int)Math.Round(frac * 100, MidpointRounding.AwayFromZero);
                int totalCents = restCents + addCents;

                long extra = totalCents / 100;          // carry em unidades inteiras
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

                Game.ExpansionStats.CoinsGain.TryGetValue(id, out var coinExpa);
                coinExpa = coinExpa + gain + extra;
                Game.ExpansionStats.CoinsGain[id] = coinExpa;

                Game.GameStats.CoinsGain.TryGetValue(id, out var coinGame);
                coinGame = coinGame + gain + extra;
                Game.GameStats.CoinsGain[id] = coinGame;
            }
            if (type == ItemType.Resource)
            {
                stage.ExpeditionStats.ResourcesFrac.TryGetValue(id, out var restDouble);
                int restCents = (int)Math.Round(restDouble * 100, MidpointRounding.AwayFromZero);

                int addCents = (int)Math.Round(frac * 100, MidpointRounding.AwayFromZero);
                int totalCents = restCents + addCents;

                long extra = totalCents / 100;          // carry em unidades inteiras
                int newRestCents = totalCents % 100;    // 0..99

                double newRestDouble = newRestCents / 100.0;

                // ---- acumula ----
                stage.ExpeditionStats.Resources.TryGetValue(id, out var coin);
                coin = coin + gain + extra;

                stage.ExpeditionStats.ResourcesGain.TryGetValue(id, out var coinExpe);
                coinExpe = coinExpe + gain + extra;

                // ---- persistir ----
                stage.ExpeditionStats.Resources[id] = coin;
                stage.ExpeditionStats.ResourcesGain[id] = coinExpe;
                stage.ExpeditionStats.ResourcesFrac[id] = newRestDouble;

                Game.ExpansionStats.ResourcesGain.TryGetValue(id, out var coinExpa);
                coinExpa = coinExpa + gain + extra;
                Game.ExpansionStats.ResourcesGain[id] = coinExpa;

                Game.GameStats.ResourcesGain.TryGetValue(id, out var coinGame);
                coinGame = coinGame + gain + extra;
                Game.GameStats.ResourcesGain[id] = coinGame;
            }
            if (type == ItemType.Knowledge)
            {
                Game.ExpansionStats.Knowledge.TryGetValue(id, out var know);
                know = know + gain;
                Game.ExpansionStats.Knowledge[id] = know;

                Game.ExpansionStats.KnowledgeGain.TryGetValue(id, out var knowExpa);
                knowExpa = knowExpa + gain;
                Game.ExpansionStats.KnowledgeGain[id] = knowExpa;

                Game.GameStats.KnowledgeGain.TryGetValue(id, out var knowGame);
                knowGame = knowGame + gain;
                Game.GameStats.KnowledgeGain[id] = knowGame;
            }

            return true;
        }

        private double GetKnowledgeModifiers(GameModel game, string coinId)
        {
            double mult = 1.0;

            if (coinId == "m01")
            {
                foreach (var kv in game.Knowledges)
                {
                    if (kv.Key != "k01" || kv.Key != "k02" || kv.Key != "k03") continue;
                    var k = kv.Value;
                    var knowledge = _locate.LocateKnowledge(game, k.Id);

                    game.ExpansionStats.KnowledgeGain.TryGetValue(knowledge.Id, out var totalK);

                    double bonus = 1.0 + (knowledge.GenerationFactor * Math.Pow(totalK, knowledge.GainCoinCurve));

                    mult *= bonus;
                }
            }

            return mult;
        }

    }
}
