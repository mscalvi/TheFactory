using FurmaIdle.Models;
using System.Collections.Generic;

namespace FurmaIdle.Helpers
{
    public static class PricingHelper
    {
        public enum PricingId
        {
            // Unlock
            CharacterUnlock0,       // Unlock Character Stage 0

            ContractUnlock0,       // Unlock Contract Stage 0

            ContractLevelUnlock0,   // Unlock Contract Level Stage 0

            KnowledgeUnlock0,       // Unlock Knowledge Stage 0

            LocalUnlock0,           // Unlock Local Stage 0

            ResourceUnlock01,       // Unlock Resource 01

            StageUnlock1,           // Unlock Stage -> 1

            ExpansionUnlock0,       // Unlock Expansion Stage 0

            TechUnlockk01,          // Unlock Tech Know 01
            TechUnlockk02,          // Unlock Tech Know 02
            TechUnlockk03,          // Unlock Tech Know 03
            TechUnlockk04,          // Unlock Tech Know 04
            TechUnlockk05,          // Unlock Tech Know 05

            // Party
            PartySize0,             // Party Size Increase Stage 0
            ContractCapUnlock0,     // Contract Cap Increase Stage 0

            // Contract Cost Expedition
            ContractCost0,         // Contract Cost Stage 0

            // Contract Gain Expedition
            ContractGain0,         // Contract Gain Stage 0 

            // Contract Time Expedition
            ContractTime0,         // Contract Time Stage 0

            // Click Gain Expedition
            ClickGain0,             // Click Gain Stage 0

            // Contract Purchase
            ContractPurchase01,          // Purchase Contract Stage 0 Level 1
            ContractPurchase02,          // Purchase Contract Stage 0 Level 2
            ContractPurchase03,          // Purchase Contract Stage 0 Level 3
            ContractPurchase04,          // Purchase Contract Stage 0 Level 4

            // Tech Upgrades
            TechUpgrade0,           // Tech Upgrade Stage 0

            // Expansion Upgrades
            ExpansionUpgrade0,     // Expansion Upgrade Stage 0
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
                [PricingId.CharacterUnlock0] = new Entry("m01", 1, 1.0, CostFactor.CharactersUnlocked, 9.0),

                // Unlock Contract Stage X Level X
                [PricingId.ContractUnlock0] = new Entry("m01", 5, 3.0, CostFactor.Level, 7),

                // Unlock Contract Level Stage X
                [PricingId.ContractLevelUnlock0] = new Entry("m01", 1000, 4.0, CostFactor.None, 1),

                // Unlock Knowledge Stage X
                [PricingId.KnowledgeUnlock0] = new Entry("m01", 100000, 1.0, CostFactor.KnowledgesUnlocked, 5.9),

                // Unlock Local Stage X
                [PricingId.LocalUnlock0] = new Entry("m01", 70000, 1.0, CostFactor.LocalsUnlocked, 5.2),

                // Unlock Resource X
                [PricingId.ResourceUnlock01] = new Entry("m01", 1500, 1.0, CostFactor.None, 1),

                // Unlock Stage -> X
                [PricingId.StageUnlock1] = new Entry("m01", 10000000000, 1.0, CostFactor.None, 1),

                // Unlock Expansion Stage X
                [PricingId.ExpansionUnlock0] = new Entry("m01", 500000, 1.0, CostFactor.Level, 6.5),

                // Unlock Tech Know X
                [PricingId.TechUnlockk01] = new Entry("k01", 1, 1.0, CostFactor.Level, 4.9),
                [PricingId.TechUnlockk02] = new Entry("k02", 1, 1.0, CostFactor.Level, 4.9),
                [PricingId.TechUnlockk03] = new Entry("k03", 1, 1.0, CostFactor.Level, 4.9),
                [PricingId.TechUnlockk04] = new Entry("k04", 1, 1.0, CostFactor.Level, 6.2),
                [PricingId.TechUnlockk05] = new Entry("k05", 1, 1.0, CostFactor.Level, 6.2),

                // Party Stage X
                [PricingId.PartySize0] = new Entry("m01", 8000, 1, CostFactor.Level, 2.2),
                [PricingId.ContractCapUnlock0] = new Entry("m01", 10, 2.8, CostFactor.None, 1),

                // Contract Cost Stage X Contract Level X
                [PricingId.ContractCost0] = new Entry("m01", 200, 4.0, CostFactor.Level, 3),

                // Contract Gain Stage X Contract Level X
                [PricingId.ContractGain0] = new Entry("m01", 50, 1.6, CostFactor.Level, 3),

                // Contract Time Stage X Contract Level X
                [PricingId.ContractTime0] = new Entry("m01", 200, 1.8, CostFactor.Level, 4),

                // Purchase Contract Stage X Level X
                [PricingId.ContractPurchase01] = new Entry("m01", 10, 1.13, CostFactor.None, 1),
                [PricingId.ContractPurchase02] = new Entry("m01", 100, 1.14, CostFactor.None, 1),
                [PricingId.ContractPurchase03] = new Entry("m01", 1000, 1.15, CostFactor.None, 1),
                [PricingId.ContractPurchase04] = new Entry("m01", 10000, 1.16, CostFactor.None, 1),

                // Click Gain Stage X
                [PricingId.ClickGain0] = new Entry("m01", 50, 2.4, CostFactor.None, 1),

                // Tech Upgrades Stage X
                [PricingId.TechUpgrade0] = new Entry("m01", 25000, 1.0, CostFactor.Level, 2.9),

                // Expansion Upgrades Stage X
                [PricingId.ExpansionUpgrade0] = new Entry("m01", 10000, 1.0, CostFactor.Level, 3.2),
            };

            public static Entry Get(PricingId id) => _map[id];
        }
    }
}
