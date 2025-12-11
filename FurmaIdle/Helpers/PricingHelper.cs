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

            ContractUnlock0,       // Unlock Contract Stage 0
            ContractUnlock1,       // Unlock Contract Stage 1

            ContractLevelUnlock00,   // Unlock Contract Level Stage 0
            ContractLevelUnlock01,   // Unlock Contract Level Stage 1

            KnowledgeUnlock1,       // Unlock Knowledge Stage 1

            LocalUnlock01,           // Unlock Local Stage 1

            ResourceUnlock01,       // Unlock Resource 01

            StageUnlock1,           // Unlock Stage -> 1
            StageUnlock2,           // Unlock Stage -> 2

            ExpansionUnlock1,       // Unlock Expansion Stage 1
            ExpansionUnlock2,       // Unlock Expansion Stage 2

            TechUnlockk01,          // Unlock Tech Know 01
            TechUnlockk02,          // Unlock Tech Know 02
            TechUnlockk03,          // Unlock Tech Know 03
            TechUnlockk04,          // Unlock Tech Know 04
            TechUnlockk05,          // Unlock Tech Know 05

            ShipUnlock1,             // Unlock Ship Stage 1
            RouteUnlock1,            // Unlock Route Stage 1

            // Party
            PartySize1,             // Party Size Increase Stage 1
            ContractCapUnlock0,     // Contract Cap Increase Stage 0
            ContractCapUnlock1,     // Contract Cap Increase Stage 1

            // Contract Cost Expedition
            ContractCost0,         // Contract Cost Stage 0
            ContractCost1,         // Contract Cost Stage 1

            // Contract Gain Expedition
            ContractGain0,         // Contract Gain Stage 0
            ContractGain1,         // Contract Gain Stage 1

            // Contract Time Expedition
            ContractTime0,         // Contract Time Stage 0
            ContractTime1,         // Contract Time Stage 1

            // Click Gain Expedition
            ClickGainS0,             // Click Gain Soma Stage 0
            ClickGainM0,             // Click Gain Multi Stage 0
            ClickGainS1,             // Click Gain Soma Stage 1
            ClickGainM1,             // Click Gain Multi Stage 1

            // Resource Gain Expedition
            ResourceGain0101,        // Resource 01 Stage 1

            // Contract Purchase
            ContractPurchase0,          // Purchase Contract Level 0
            ContractPurchase1,          // Purchase Contract Level 1
            ContractPurchase2,          // Purchase Contract Level 2
            ContractPurchase3,          // Purchase Contract Level 3
            ContractPurchase4,          // Purchase Contract Level 4

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
                [PricingId.ContractUnlock0] = new Entry("m01", 300, 3.0, CostFactor.Level, 7),
                [PricingId.ContractUnlock1] = new Entry("m01", 5, 3.0, CostFactor.Level, 7),

                // Unlock Contract Level X-X+2
                [PricingId.ContractLevelUnlock00] = new Entry("m01", 10, 4.1, CostFactor.None, 1),
                [PricingId.ContractLevelUnlock01] = new Entry("m01", 600, 3.8, CostFactor.Level, 4.3),

                // Unlock Knowledge Stage X
                [PricingId.KnowledgeUnlock1] = new Entry("m01", 60000, 1.0, CostFactor.KnowledgesUnlocked, 5.3),

                // Unlock Local Stage X
                [PricingId.LocalUnlock01] = new Entry("m01", 140000, 1.0, CostFactor.LocalsUnlocked, 6.5),

                // Unlock Resource X
                [PricingId.ResourceUnlock01] = new Entry("m01", 1500, 1.0, CostFactor.None, 1),

                // Unlock Stage X -> X+1
                [PricingId.StageUnlock1] = new Entry("m01", 1000, 1.0, CostFactor.None, 1),
                [PricingId.StageUnlock2] = new Entry("m01", 1000000000000, 1.0, CostFactor.None, 1),

                // Unlock Expansion Stage X
                [PricingId.ExpansionUnlock1] = new Entry("m01", 500000, 1.0, CostFactor.Level, 10.0),
                [PricingId.ExpansionUnlock2] = new Entry("m01", 500000000000000, 1.0, CostFactor.Level, 10.0),

                // Unlock Tech Know X
                [PricingId.TechUnlockk01] = new Entry("k01", 1, 1.0, CostFactor.Level, 4.9),
                [PricingId.TechUnlockk02] = new Entry("k02", 1, 1.0, CostFactor.Level, 4.9),
                [PricingId.TechUnlockk03] = new Entry("k03", 1, 1.0, CostFactor.Level, 4.9),
                [PricingId.TechUnlockk04] = new Entry("k04", 1, 1.0, CostFactor.Level, 6.2),
                [PricingId.TechUnlockk05] = new Entry("k05", 1, 1.0, CostFactor.Level, 6.2),

                // Unlock Ship Stage X Level X
                [PricingId.ShipUnlock1] = new Entry("m01", 10000000000, 1.0, CostFactor.Level, 4.2),

                // Unlock Route Stage X Level X
                [PricingId.RouteUnlock1] = new Entry("m01", 750000000, 1.0, CostFactor.Level, 4.2),

                // Party Stage X
                [PricingId.PartySize1] = new Entry("m01", 8000, 1, CostFactor.Level, 2.2),
                [PricingId.ContractCapUnlock0] = new Entry("m01", 10, 1.8, CostFactor.Level, 8.8),
                [PricingId.ContractCapUnlock1] = new Entry("m01", 10, 3.3, CostFactor.Level, 8.8),

                // Contract Cost Stage X
                [PricingId.ContractCost0] = new Entry("m01", 50, 4.0, CostFactor.Level, 3),
                [PricingId.ContractCost1] = new Entry("m01", 200, 4.0, CostFactor.Level, 3),

                // Contract Gain Stage X
                [PricingId.ContractGain0] = new Entry("m01", 15, 1.6, CostFactor.Level, 3),
                [PricingId.ContractGain1] = new Entry("m01", 50, 1.6, CostFactor.Level, 3),

                // Contract Time Stage X
                [PricingId.ContractTime0] = new Entry("m01", 50, 1.6, CostFactor.Level, 4),
                [PricingId.ContractTime1] = new Entry("m01", 200, 1.6, CostFactor.Level, 4),

                // Purchase Contract Stage X Level X
                [PricingId.ContractPurchase0] = new Entry("m01", 10, 1.11, CostFactor.None, 1),
                [PricingId.ContractPurchase1] = new Entry("m01", 10, 1.13, CostFactor.None, 1),
                [PricingId.ContractPurchase2] = new Entry("m01", 100, 1.14, CostFactor.None, 1),
                [PricingId.ContractPurchase3] = new Entry("m01", 1000, 1.15, CostFactor.None, 1),
                [PricingId.ContractPurchase4] = new Entry("m01", 10000, 1.16, CostFactor.None, 1),

                // Click Gain Stage X
                [PricingId.ClickGainS0] = new Entry("m01", 20, 2.2, CostFactor.None, 1),
                [PricingId.ClickGainM0] = new Entry("m01", 350, 3.6, CostFactor.None, 1),
                [PricingId.ClickGainS1] = new Entry("m01", 50, 2.4, CostFactor.None, 1),
                [PricingId.ClickGainM1] = new Entry("m01", 600, 3.6, CostFactor.None, 1),

                // Resource X Gain Stage X
                [PricingId.ResourceGain0101] = new Entry("m01", 500, 1.9, CostFactor.None, 1),

                // Tech Upgrades Stage X
                [PricingId.TechUpgrade1] = new Entry("m01", 15000, 1.0, CostFactor.Level, 3.3),

                // Expansion Upgrades Stage X
                [PricingId.ExpansionUpgrade1] = new Entry("m01", 10000, 1.0, CostFactor.Level, 3.2),
            };

            public static Entry Get(PricingId id) => _map[id];
        }
    }
}
