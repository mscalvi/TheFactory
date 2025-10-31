using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FurmaIdle.Services
{
    public interface IKnowledgeService
    {
        Dictionary<string, double> ComputeGains(StageModel stage, long coins);
        IEnumerable<KnowledgePreview> ComputePreview(StageModel stage, long coinsSoFar, double coinsPerSec = 0);

        Task<Dictionary<string, long>> ApplyGainsAsync(StageModel stage, long coins);
    }

    public sealed record KnowledgePreview(
        string KnowledgeId,
        double Factor,          
        double KPrev,           
        double CoinsNeededNext,
        double? EtaSeconds
    );

    public sealed class KnowledgeService : IKnowledgeService
    {
        private readonly IUiLogService _log;
        private readonly ICurrentGameService _game;
        private readonly IIncomeService _income;
        private readonly ILocateService _locate;

        public KnowledgeService (IUiLogService log, ICurrentGameService game, IIncomeService income, ILocateService locate)
        {
            _log = log;
            _game = game;
            _income = income;
            _locate = locate;
        }
        public Dictionary<string, double> ComputeGains(StageModel stage, long coins)
        {
            var result = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            var weights = GetKnowFactor(stage);
            var kCoinId = stage.CoinId;

            foreach (var (kId, w) in weights)
            {
                var knowledge = _locate.LocateKnowledge(_game.CurrentGame, kId);
                if (knowledge.GainCoinId != kCoinId) continue;

                double coinsK = coins * w;

                double kPrev = 0;
                if (_game.CurrentGame.ExpansionStats?.KnowledgeGain?.TryGetValue(kId, out var stored) == true)
                    kPrev = stored;

                double baseC = knowledge.GainCoinBase;
                double curve = knowledge.GainCoinCurve;

                double cPrev = baseC * Math.Pow(kPrev + 1.0, 1.0 / curve);
                double cNew = cPrev + coinsK;
                double kNew = Math.Pow(cNew / baseC, curve) - 1.0;
                double dK = kNew - kPrev;

                var (add, mult) = GetModifiers(knowledge, EffectHelper.EffectType.KnowledgeGain);
                double final = (dK + add) * mult;

                if (final > 0) result[kId] = final;
            }
            return result;
        }

        public async Task<Dictionary<string, long>> ApplyGainsAsync(StageModel stage, long coins)
        {
            var fractional = ComputeGains(stage, coins);
            var applied = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

            foreach (var (kId, d) in fractional)
            {
                long gainInt = (long)Math.Floor(d);
                if (gainInt < 1) continue;

                await _income.AddAsync(
                    ItemHelper.ItemType.Knowledge,
                    kId, gainInt,
                    ItemHelper.ItemType.Expedition, stage.Id, stage.Id
                );

                applied[kId] = gainInt;
            }
            return applied;
        }

        public IEnumerable<KnowledgePreview> ComputePreview(StageModel stage, long coinsSoFar, double coinsPerSec = 0)
        {
            var weights = GetKnowFactor(stage);
            var kCoinId = stage.CoinId;

            foreach (var (kId, w) in weights)
            {
                var k = _locate.LocateKnowledge(_game.CurrentGame, kId);
                if (k.GainCoinId != kCoinId) continue;

                double kPrev = 0;
                if (_game.CurrentGame.ExpansionStats?.KnowledgeGain?.TryGetValue(kId, out var stored) == true)
                    kPrev = stored;

                double baseC = k.GainCoinBase;
                double curve = k.GainCoinCurve;

                double cPrev = baseC * Math.Pow(kPrev + 1.0, 1.0 / curve);
                long kTargetInt = (long)Math.Floor(kPrev) + 1;
                double cTarget = baseC * Math.Pow(kTargetInt + 1.0, 1.0 / curve);

                double deltaC = Math.Max(0, cTarget - cPrev);

                double? eta = null;
                if (coinsPerSec > 0 && w > 0)
                    eta = deltaC / (coinsPerSec * w);

                yield return new KnowledgePreview(kId, w, kPrev, deltaC, eta);
            }
        }

        // Helpers
        private Dictionary<string, double> GetKnowFactor(StageModel stage)
        {
            Dictionary<string, int> kCounters = new Dictionary<string, int>();
            Dictionary<string, double> kFactors = new Dictionary<string, double>();

            foreach (var know in _game.CurrentGame.Knowledges)
            {
                int counter = 0;
                if (know.Value.State == Helpers.UnlockHelper.State.Unlocked)
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

                    if (stage.ActiveContracts.Count > 0)
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
                            if (contract.KnowledgeFactor3 == know.Value.Id)
                            {
                                counter += 3;
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

            foreach (var know in kCounters)
            {
                kFactors.Add(know.Key, (know.Value) / (kTotal));
            }

            return kFactors;
        }
        private static (double AddMod, double MultMod) GetModifiers(KnowledgeModel knowledge, EffectHelper.EffectType type)
        {
            double AddMod = 0;
            double MultMod = 1;

            foreach (var modifier in knowledge.Modifiers)
            {
                if (type == modifier.Type)
                {
                    if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                    {
                        AddMod += modifier.Value;
                    }
                    if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                    {
                        MultMod *= modifier.Value;
                    }
                }
            }

            return (AddMod, MultMod);
        }


        // Obsoleta, só fiquei com dó de apagar
        public async Task GetKnowledgeGain(StageModel stage, long coins)
        {
            var result = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            var kFactors = GetKnowFactor(stage);
            string kCoinId = stage.CoinId;

            foreach (var know in kFactors)
            {
                var knowledge = _locate.LocateKnowledge(_game.CurrentGame, know.Key);

                if (knowledge.GainCoinId != kCoinId) continue;

                double coinsK = coins * know.Value;

                // K já existente (acumulado)
                double kPrev = 0.0;
                if (_game.CurrentGame.ExpansionStats?.KnowledgeGain is not null &&
                    _game.CurrentGame.ExpansionStats.KnowledgeGain.TryGetValue(know.Key, out var stored))
                {
                    kPrev = Math.Max(0.0, stored);
                }

                double cPrev = knowledge.GainCoinBase * Math.Pow(kPrev + 1.0, 1.0 / knowledge.GainCoinCurve);
                double cNew = cPrev + coinsK;
                double kNew = Math.Max(0.0, Math.Pow(cNew / knowledge.GainCoinBase, knowledge.GainCoinCurve) - 1.0);
                double dK = Math.Max(0.0, kNew - kPrev);

                var modifier = GetModifiers(knowledge, EffectHelper.EffectType.KnowledgeGain);

                double finalGain = (dK + modifier.AddMod) * modifier.MultMod;

                long finalInt = (long)Math.Floor(finalGain);

                if (finalGain > 0) result[know.Key] = finalGain;

            }

            foreach (var know in result)
            {
                await _income.AddAsync(ItemHelper.ItemType.Knowledge, know.Key, know.Value, ItemHelper.ItemType.Expedition, stage.Id, stage.Id);
            }
        }
    }
}
