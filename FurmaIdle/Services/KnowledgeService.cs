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
                if (knowledge.GainCoinId != kCoinId) continue;

                double coinsK = coins * factor;

                double kGain = 0;
                double kPrev = 0;

                if (expansion.ExpansionStats.KnowledgeGain?.TryGetValue(kId, out var stored) == true)
                    kPrev = stored;

                kGain = coinsK/(knowledge.GainCoinBase * Math.Pow(kPrev + 1, knowledge.GainCoinCurve));

                var modifier = _modifier.GetModifiers(ItemHelper.ItemType.Knowledge, kId, stage.Id, EffectHelper.EffectSupertype.Gain);
                double final = (kGain + modifier.AddMod) * modifier.MultMod;

                if (final > 0) result[kId] = final;
            }
            return result;
        }
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
                kFactors.Add(know.Key, (know.Value / kTotal));
            }

            return kFactors;
        }       
    }
}
