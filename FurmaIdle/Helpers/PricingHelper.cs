using FurmaIdle.Models;
using System.Collections.Generic;

namespace FurmaIdle.Helpers
{
    public static class PricingHelper
    {
        public enum PricingId
        {
            // Unlock
            CharacterUnlock1,       // Unlock Character Stage 1

            ContractUnlock1,       // Unlock Contract Stage 1

            ContractLevelUnlock1,   // Unlock Contract Level Stage 1

            KnowledgeUnlock1,       // Unlock Knowledge Stage 1

            LocalUnlock1,           // Unlock Local Stage 1

            ResourceUnlock01,       // Unlock Resource 01

            StageUnlock1,           // Unlock Stage -> 1

            ExpansionUnlock1,       // Unlock Expansion Stage 1

            TechUnlockk01,          // Unlock Tech Know 01
            TechUnlockk02,          // Unlock Tech Know 02
            TechUnlockk03,          // Unlock Tech Know 03
            TechUnlockk04,          // Unlock Tech Know 04
            TechUnlockk05,          // Unlock Tech Know 05

            // Party
            PartySize1,             // Party Size Increase Stage 1
            ContractCapUnlock1,     // Contract Cap Increase Stage 1

            // Contract Cost Expedition
            ContractCost1,         // Contract Cost Stage 1

            // Contract Gain Expedition
            ContractGain1,         // Contract Gain Stage 1

            // Contract Time Expedition
            ContractTime1,         // Contract Time Stage 1

            // Click Gain Expedition
            ClickGainS1,             // Click Gain Soma Stage 1
            ClickGainM1,             // Click Gain Multi Stage 1

            // Resource Gain Expedition
            ResourceGain011,        // Resource 01 Stage 1

            // Contract Purchase
            ContractPurchase11,          // Purchase Contract Stage 1 Level 1
            ContractPurchase12,          // Purchase Contract Stage 1 Level 2
            ContractPurchase13,          // Purchase Contract Stage 1 Level 3
            ContractPurchase14,          // Purchase Contract Stage 1 Level 4

            // Tech Upgrades
            TechUpgrade1,           // Tech Upgrade Stage 1

            // Expansion Upgrades
            ExpansionUpgrade1,     // Expansion Upgrade Stage 1
        }
        public enum CostFactor
        {
            None,
            CharactersUnlocked,
            KnowledgesUnlocked,
            ResourcesUnlocked,
            LocalsUnlocked,
            ExpansionsUnlocked,
            PartySize,
            Level
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
                    double costFactorCurve)
                {
                    CostCoinId = costCoinId;
                    CostBase = costBase;
                    CostCurve = costCurve;
                    CostFactor = costFactor;
                    CostFactorCurve = costFactorCurve;
                }

                public string CostCoinId { get; }
                public long CostBase { get; }
                public double CostCurve { get; }
                public CostFactor CostFactor { get; }
                public double CostFactorCurve { get; }
            }

            private static readonly Dictionary<PricingId, Entry> _map = new()
            {
                // CostCoinId, Base, Curve, Factor?, Operation?, FactorCurve?
                // Unlock Character Stage X
                [PricingId.CharacterUnlock1] = new Entry("m01", 1, 1.0, CostFactor.CharactersUnlocked, 9.0),

                // Unlock Contract Stage X Level X
                [PricingId.ContractUnlock1] = new Entry("m01", 5, 3.0, CostFactor.Level, 7),

                // Unlock Contract Level Stage X
                [PricingId.ContractLevelUnlock1] = new Entry("m01", 1000, 4.0, CostFactor.None, 1),

                // Unlock Knowledge Stage X
                [PricingId.KnowledgeUnlock1] = new Entry("m01", 100000, 1.0, CostFactor.KnowledgesUnlocked, 5.9),

                // Unlock Local Stage X
                [PricingId.LocalUnlock1] = new Entry("m01", 70000, 1.0, CostFactor.LocalsUnlocked, 5.2),

                // Unlock Resource X
                [PricingId.ResourceUnlock01] = new Entry("m01", 1500, 1.0, CostFactor.None, 1),

                // Unlock Stage -> X
                [PricingId.StageUnlock1] = new Entry("m01", 1000000000000, 1.0, CostFactor.None, 1),

                // Unlock Expansion Stage X
                [PricingId.ExpansionUnlock1] = new Entry("m01", 500000, 1.0, CostFactor.Level, 10.0),

                // Unlock Tech Know X
                [PricingId.TechUnlockk01] = new Entry("k01", 1, 1.0, CostFactor.Level, 4.9),
                [PricingId.TechUnlockk02] = new Entry("k02", 1, 1.0, CostFactor.Level, 4.9),
                [PricingId.TechUnlockk03] = new Entry("k03", 1, 1.0, CostFactor.Level, 4.9),
                [PricingId.TechUnlockk04] = new Entry("k04", 1, 1.0, CostFactor.Level, 6.2),
                [PricingId.TechUnlockk05] = new Entry("k05", 1, 1.0, CostFactor.Level, 6.2),

                // Party Stage X
                [PricingId.PartySize1] = new Entry("m01", 8000, 1, CostFactor.Level, 2.2),
                [PricingId.ContractCapUnlock1] = new Entry("m01", 10, 3.3, CostFactor.Level, 8.8),

                // Contract Cost Stage X Contract Level X
                [PricingId.ContractCost1] = new Entry("m01", 200, 4.0, CostFactor.Level, 3),

                // Contract Gain Stage X Contract Level X
                [PricingId.ContractGain1] = new Entry("m01", 50, 1.6, CostFactor.Level, 3),

                // Contract Time Stage X Contract Level X
                [PricingId.ContractTime1] = new Entry("m01", 200, 1.8, CostFactor.Level, 4),

                // Purchase Contract Stage X Level X
                [PricingId.ContractPurchase11] = new Entry("m01", 10, 1.13, CostFactor.None, 1),
                [PricingId.ContractPurchase12] = new Entry("m01", 100, 1.14, CostFactor.None, 1),
                [PricingId.ContractPurchase13] = new Entry("m01", 1000, 1.15, CostFactor.None, 1),
                [PricingId.ContractPurchase14] = new Entry("m01", 10000, 1.16, CostFactor.None, 1),

                // Click Gain Stage X
                [PricingId.ClickGainS1] = new Entry("m01", 50, 2.4, CostFactor.None, 1),
                [PricingId.ClickGainM1] = new Entry("m01", 600, 3.6, CostFactor.None, 1),

                // Resource X Gain Stage X
                [PricingId.ResourceGain011] = new Entry("m01", 500, 2.4, CostFactor.None, 1),

                // Tech Upgrades Stage X
                [PricingId.TechUpgrade1] = new Entry("m01", 25000, 1.0, CostFactor.Level, 2.9),

                // Expansion Upgrades Stage X
                [PricingId.ExpansionUpgrade1] = new Entry("m01", 10000, 1.0, CostFactor.Level, 3.2),
            };

            public static Entry Get(PricingId id) => _map[id];
        }
    }
}
