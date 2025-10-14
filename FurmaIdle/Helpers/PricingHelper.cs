using System.Collections.Generic;

namespace FurmaIdle.Helpers
{
    public static class PricingHelper
    {
        public enum PricingId
        {
            CharacterUnlock1m01,
            ClickGain1m01,
            ContractCapUnlock1m01,
            ContractCost1m01,
            ContractCost2m01,
            ContractCost3m01,
            ContractCost4m01,
            ContractGain1m01,
            ContractGain2m01,
            ContractGain3m01,
            ContractGain4m01,
            ContractLevelUnlock1m01,
            ContractTime1m01,
            ContractTime2m01,
            ContractTime3m01,
            ContractTime4m01,
            ContractUnlock2m01,
            ContractUnlock3m01,
            ContractUnlock4m01,
            KnowledgeUnlock1m01,
            ResourceGain1m01,
            ResourceGain2m01,
            ResourceUnlock1m01,
            StageUnlock1m01,
            UnlockLocal1m01,
            TechUnlock1k01,
            TechUnlock1k02,
            TechUnlock1k03,
            TechUnlock1k04,
            TechUnlock1k05,
            Expansions1
        }

        public enum CostFactor
        {
            None,
            CharactersUnlocked,
            KnowledgesUnlocked,
            ResourcesUnlocked,
            LocalsUnlocked
        }

        public static class PricingCost
        {
            public readonly struct Entry
            {
                public Entry(
                    string costCoinId,
                    double costBase,
                    double costCurve,
                    CostFactor? costFactor = null,
                    EffectHelper.EffectOperation? costFactorOperation = null,
                    double? costFactorCurve = null)
                {
                    CostCoinId = costCoinId;
                    CostBase = costBase;
                    CostCurve = costCurve;
                    CostFactor = costFactor;
                    CostFactorOperation = costFactorOperation;
                    CostFactorCurve = costFactorCurve;
                }

                public string CostCoinId { get; }
                public double CostBase { get; }
                public double CostCurve { get; }
                public CostFactor? CostFactor { get; }
                public EffectHelper.EffectOperation? CostFactorOperation { get; }
                public double? CostFactorCurve { get; }
            }

            private static readonly Dictionary<PricingId, Entry> _map = new()
            {
                // CostCoinId, Base, Curve, Factor?, Operation?, FactorCurve?
                [PricingId.CharacterUnlock1m01] = new Entry("m01", 3000, 1.0, CostFactor.CharactersUnlocked, EffectHelper.EffectOperation.Multiplicative, 1.7),
                [PricingId.ClickGain1m01] = new Entry("m01", 200, 2.4),
                [PricingId.ContractCapUnlock1m01] = new Entry("m01", 10, 1.3),
                [PricingId.ContractCost1m01] = new Entry("m01", 100, 2.1),
                [PricingId.ContractCost2m01] = new Entry("m01", 1000, 2.1),
                [PricingId.ContractCost3m01] = new Entry("m01", 20000, 2.1),
                [PricingId.ContractCost4m01] = new Entry("m01", 150000, 2.1),
                [PricingId.ContractGain1m01] = new Entry("m01", 50, 1.8),
                [PricingId.ContractGain2m01] = new Entry("m01", 500, 1.8),
                [PricingId.ContractGain3m01] = new Entry("m01", 10000, 1.8),
                [PricingId.ContractGain4m01] = new Entry("m01", 150000, 1.8),
                [PricingId.ContractLevelUnlock1m01] = new Entry("m01", 50, 10.0),
                [PricingId.ContractTime1m01] = new Entry("m01", 200, 2.9),
                [PricingId.ContractTime2m01] = new Entry("m01", 2000, 2.9),
                [PricingId.ContractTime3m01] = new Entry("m01", 40000, 2.9),
                [PricingId.ContractTime4m01] = new Entry("m01", 15000, 2.9),
                [PricingId.ContractUnlock2m01] = new Entry("m01", 200, 1.0),
                [PricingId.ContractUnlock3m01] = new Entry("m01", 4000, 1.0),
                [PricingId.ContractUnlock4m01] = new Entry("m01", 50000, 1.0),
                [PricingId.KnowledgeUnlock1m01] = new Entry("m01", 100000, 1.0, CostFactor.KnowledgesUnlocked, EffectHelper.EffectOperation.Multiplicative, 1.7),
                [PricingId.ResourceGain1m01] = new Entry("m01", 9000, 1.0),
                [PricingId.ResourceGain2m01] = new Entry("m01", 35000, 1.0),
                [PricingId.ResourceUnlock1m01] = new Entry("m01", 2000, 1.0, CostFactor.ResourcesUnlocked, EffectHelper.EffectOperation.Multiplicative, 1.7),
                [PricingId.StageUnlock1m01] = new Entry("m01", 50000000, 1.0),
                [PricingId.UnlockLocal1m01] = new Entry("m01", 100000, 1.0, CostFactor.LocalsUnlocked, EffectHelper.EffectOperation.Multiplicative, 1.7),
                [PricingId.TechUnlock1k01] = new Entry("k01", 1, 10.0),
                [PricingId.TechUnlock1k02] = new Entry("k02", 1, 10.0),
                [PricingId.TechUnlock1k03] = new Entry("k03", 1, 10.0),
                [PricingId.TechUnlock1k04] = new Entry("k04", 1, 10.0),
                [PricingId.TechUnlock1k05] = new Entry("k05", 1, 10.0),
                [PricingId.Expansions1] = new Entry("m01", 500000, 13.0),
            };

            public static Entry Get(PricingId id) => _map[id];

            public static bool TryGet(PricingId id, out Entry entry) => _map.TryGetValue(id, out entry);
        }
    }
}
