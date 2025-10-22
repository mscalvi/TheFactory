using FurmaIdle.Models;
using System.Collections.Generic;

namespace FurmaIdle.Helpers
{
    public static class PricingHelper
    {
        public enum PricingId
        {
            // Characteres
            CharacterUnlock0m01,
            CharacterUnlock1m01,

            // Clicks
            ClickGain1m01,

            // Contracts
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
            ContractUnlock1m01,
            ContractUnlock2m01,
            ContractUnlock3m01,
            ContractUnlock4m01,
            ContractAdd1m01,
            ContractAdd2m01,
            ContractAdd3m01,
            ContractAdd4m01,
            ContractAdd5m01,
            ContractAdd6m01,

            // Knowledges
            KnowledgeUnlock1m01,
            KnowledgeGain1m01,

            // Resources
            ResourceGain1m01,
            ResourceGain2m01,
            ResourceUnlock1m01,

            // Stages
            StageUnlock1m01,

            // Locals
            LocalUnlock0m01,
            LocalUnlock1m01,

            // Techs
            TechUnlock1k01,
            TechUnlock1k02,
            TechUnlock1k03,
            TechUnlock1k04,
            TechUnlock1k05,

            // Expansions
            Expansions1,

            // Party
            PartySize
        }
        public enum CostFactor
        {
            None,
            CharactersUnlocked,
            KnowledgesUnlocked,
            ResourcesUnlocked,
            LocalsUnlocked,
            ExpansionsUnlocked,
            PartySize
        }
        public enum CostFactorType
        {
            None,
            Additive,
            Multiplicative,
        }
        public static class PricingCost
        {
            public readonly struct Entry
            {
                public Entry(
                    string costCoinId,
                    long costBase,
                    double costCurve,
                    CostFactor costFactor,
                    CostFactorType costFactorType,
                    double costFactorCurve)
                {
                    CostCoinId = costCoinId;
                    CostBase = costBase;
                    CostCurve = costCurve;
                    CostFactor = costFactor;
                    CostFactorType = costFactorType;
                    CostFactorCurve = costFactorCurve;
                }

                public string CostCoinId { get; }
                public long CostBase { get; }
                public double CostCurve { get; }
                public CostFactor CostFactor { get; }
                public CostFactorType CostFactorType { get; }
                public double CostFactorCurve { get; }
            }

            private static readonly Dictionary<PricingId, Entry> _map = new()
            {
                // CostCoinId, Base, Curve, Factor?, Operation?, FactorCurve?
                [PricingId.CharacterUnlock0m01] = new Entry("m01", 0, 1.0, CostFactor.None, CostFactorType.None, 0),
                [PricingId.CharacterUnlock1m01] = new Entry("m01", 3000, 1.0, CostFactor.CharactersUnlocked, CostFactorType.Multiplicative, 1.7),
                [PricingId.ClickGain1m01] = new Entry("m01", 200, 2.4, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractCapUnlock1m01] = new Entry("m01", 10, 1.4, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractCost1m01] = new Entry("m01", 100, 2.1, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractCost2m01] = new Entry("m01", 1000, 2.1, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractCost3m01] = new Entry("m01", 20000, 2.1, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractCost4m01] = new Entry("m01", 150000, 2.1, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractGain1m01] = new Entry("m01", 50, 1.8, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractGain2m01] = new Entry("m01", 500, 1.8, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractGain3m01] = new Entry("m01", 10000, 1.8, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractGain4m01] = new Entry("m01", 150000, 1.8, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractLevelUnlock1m01] = new Entry("m01", 300, 14.0, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractTime1m01] = new Entry("m01", 200, 2.9, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractTime2m01] = new Entry("m01", 2000, 2.9, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractTime3m01] = new Entry("m01", 40000, 2.9, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractTime4m01] = new Entry("m01", 150000, 2.9, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractUnlock1m01] = new Entry("m01", 5, 1.0, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractUnlock2m01] = new Entry("m01", 200, 1.0, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractUnlock3m01] = new Entry("m01", 4000, 1.0, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractUnlock4m01] = new Entry("m01", 50000, 1.0, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractAdd1m01] = new Entry("m01", 10, 1.12, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractAdd2m01] = new Entry("m01", 100, 1.13, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractAdd3m01] = new Entry("m01", 2000, 1.14, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractAdd4m01] = new Entry("m01", 50000, 1.19, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractAdd5m01] = new Entry("m01", 100000, 1.21, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ContractAdd6m01] = new Entry("m01", 5000000, 1.23, CostFactor.None, CostFactorType.None, 0),
                [PricingId.KnowledgeUnlock1m01] = new Entry("m01", 100000, 1.0, CostFactor.KnowledgesUnlocked, CostFactorType.Multiplicative, 1.7),
                [PricingId.KnowledgeGain1m01] = new Entry("m01", 50000, 2.9, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ResourceGain1m01] = new Entry("m01", 9000, 1.0, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ResourceGain2m01] = new Entry("m01", 35000, 1.0, CostFactor.None, CostFactorType.None, 0),
                [PricingId.ResourceUnlock1m01] = new Entry("m01", 2000, 1.0, CostFactor.ResourcesUnlocked, CostFactorType.Multiplicative, 1.7),
                [PricingId.StageUnlock1m01] = new Entry("m01", 50000000, 1.0, CostFactor.None, CostFactorType.None, 0),
                [PricingId.LocalUnlock0m01] = new Entry("m01", 0, 1.0, CostFactor.LocalsUnlocked, CostFactorType.Multiplicative, 1),
                [PricingId.LocalUnlock1m01] = new Entry("m01", 100000, 1.0, CostFactor.LocalsUnlocked, CostFactorType.Multiplicative, 1.7),
                [PricingId.TechUnlock1k01] = new Entry("k01", 1, 10.0, CostFactor.None, CostFactorType.None, 0),
                [PricingId.TechUnlock1k02] = new Entry("k02", 1, 10.0, CostFactor.None, CostFactorType.None, 0),
                [PricingId.TechUnlock1k03] = new Entry("k03", 1, 10.0, CostFactor.None, CostFactorType.None, 0),
                [PricingId.TechUnlock1k04] = new Entry("k04", 1, 10.0, CostFactor.None, CostFactorType.None, 0),
                [PricingId.TechUnlock1k05] = new Entry("k05", 1, 10.0, CostFactor.None, CostFactorType.None, 0),
                [PricingId.Expansions1] = new Entry("m01", 500000, 1.0, CostFactor.ExpansionsUnlocked, CostFactorType.Multiplicative, 13.0),
                [PricingId.PartySize] = new Entry("m01", 100000, 1.0, CostFactor.PartySize, CostFactorType.Multiplicative, 7.0),
            };

            public static Entry Get(PricingId id) => _map[id];

            public static bool TryGet(PricingId id, out Entry entry) => _map.TryGetValue(id, out entry);
        }
    }
}
