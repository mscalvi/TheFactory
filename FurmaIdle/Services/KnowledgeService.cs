using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FurmaIdle.Services
{
    public interface IKnowledgeService
    {
        Task EndExpeditionKnowGain(StageModel stage, long coins);

        Dictionary<string, double> KnowledgeGain(StageModel stage, long coins);
        double GetKnowledgeBurst(GameModel game, string coinId, ExpansionModel expansion);
        Dictionary<string, double> GetKnowFactor(StageModel stage);
    }

    public sealed class KnowledgeService : IKnowledgeService
    {
        private readonly IUiLogService _log;
        private readonly ICurrentGameService _game;
        private readonly IIncomeService _income;
        private readonly ILocateService _locate;
        private readonly IModifierService _modifier;

        public KnowledgeService (IUiLogService log, ICurrentGameService game, IIncomeService income, ILocateService locate, IModifierService modifier)
        {
            _log = log;
            _game = game;
            _income = income;
            _locate = locate;
            _modifier = modifier;
        }

        public async Task EndExpeditionKnowGain(StageModel stage, long coins)
        {
            var parcialResult = KnowledgeGain(stage, coins);

            foreach (var (kId, result) in parcialResult)
            {
                long gainInt = (long)Math.Floor(result);
                if (gainInt < 1) continue;

                await _income.AddAsync(
                    ItemHelper.ItemType.Knowledge,
                    kId, gainInt,
                    ItemHelper.ItemType.Expedition, stage.Id, stage.Id
                );
            }
        }

        public Dictionary<string, double> KnowledgeGain(StageModel stage, long coins)
        {
            var result = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            var factors = GetKnowFactor(stage);
            var kCoinId = stage.CoinId;
            var expansion = _locate.LocateExpansion(_game.CurrentGame, _game.CurrentGame.CurrentExpansionId);

            foreach (var (kId, factor) in factors)
            {
                var knowledge = _locate.LocateKnowledge(_game.CurrentGame, kId);
                if (!string.Equals(knowledge.GainCoinId, kCoinId, StringComparison.OrdinalIgnoreCase))
                    continue;

                double coinsK = coins * factor;
                if (coinsK <= 0) continue;

                int kPrev = 0;
                if (expansion.ExpansionStats.KnowledgeGain?.TryGetValue(kId, out var stored) == true)
                    kPrev = (int)stored;

                int gained = 0;
                double remaining = coinsK;

                // (opcional: um limite de segurança para evitar laços enormes)
                const int hardCap = 1_000_000;

                while (gained < hardCap)
                {
                    double nextCost = knowledge.GainCoinBase * Math.Pow(kPrev + gained + 1, knowledge.GainCoinCurve);
                    if (remaining + 1e-9 >= nextCost)
                    {
                        remaining -= nextCost;
                        gained++;
                    }
                    else break;
                }

                var modifier = _modifier.GetModifiers(ItemHelper.ItemType.Knowledge, kId, stage.Id, EffectHelper.EffectSupertype.Gain);
                double final = (gained + modifier.AddMod) * modifier.MultMod;

                if (final > 0) result[kId] = final;
            }
            return result;
        }

        public Dictionary<string, double> GetKnowFactor(StageModel stage)
        {
            Dictionary<string, int> kCounters = new Dictionary<string, int>();
            Dictionary<string, double> kFactors = new Dictionary<string, double>();

            foreach (var know in _game.CurrentGame.Knowledges)
            {
                int counter = 0;
                if (know.Value.State == UnlockHelper.State.Unlocked)
                {
                    foreach (var characterId in stage.Expedition.PartyIds)
                    {
                        var character = _locate.LocateCharacter(_game.CurrentGame, characterId);
                        if (character.KnowledgeFactor1 == know.Value.Id)
                        {
                            counter += 1;
                        }
                        if (character.KnowledgeFactor2 == know.Value.Id)
                        {
                            counter += 2;
                        }
                    }

                    if (stage.ActiveContracts is not null && stage.ActiveContracts.Count > 0)
                    {
                        foreach (var contractId in stage.ActiveContracts)
                        {
                            var contract = _locate.LocateContract(_game.CurrentGame, contractId.Key);
                            if (contract.KnowledgeFactor1 == know.Value.Id)
                            {
                                counter += 1;
                            }
                            if (contract.KnowledgeFactor2 == know.Value.Id)
                            {
                                counter += 2;
                            }
                        }
                    }

                    kCounters.Add(know.Key, counter);
                }
            }

            int kTotal = 0;
            foreach (var know in kCounters)
            {
                kTotal += know.Value;
            }

            if (kTotal == 0) return kFactors; // todos 0 → sem factors

            foreach (var know in kCounters)
            {
                kFactors[know.Key] = (double)know.Value / (double)kTotal;
            }

            return kFactors;
        }

        public double GetKnowledgeBurst(GameModel game, string coinId, ExpansionModel expansion)
        {
            double burst = 1.0;
            switch (coinId)
            {
                case "m01":
                    foreach (var kv in game.Knowledges)
                    {
                        if (kv.Value.GenerationCoin != IncomeHelper.CoinBurst.m01) continue;
                        var k = kv.Value;
                        var knowledge = _locate.LocateKnowledge(game, k.Id);

                        expansion.ExpansionStats.KnowledgeGain.TryGetValue(knowledge.Id, out var totalK);

                        double bonus = 1.0 + (knowledge.GenerationFactor * Math.Pow(totalK, knowledge.GainCoinCurve));

                        burst *= bonus;
                    }
                    break;
                case "m02":
                    foreach (var kv in game.Knowledges)
                    {
                        if (kv.Value.GenerationCoin != IncomeHelper.CoinBurst.m02) continue;
                        var k = kv.Value;
                        var knowledge = _locate.LocateKnowledge(game, k.Id);

                        expansion.ExpansionStats.KnowledgeGain.TryGetValue(knowledge.Id, out var totalK);

                        double bonus = 1.0 + (knowledge.GenerationFactor * Math.Pow(totalK, knowledge.GainCoinCurve));

                        burst *= bonus;
                    }
                    break;
            }

            return burst;
        }
    }
}
