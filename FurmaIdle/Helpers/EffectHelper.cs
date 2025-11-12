namespace FurmaIdle.Helpers
{
    public class EffectHelper
    {
        public enum EffectType
        {
            // Gain
            ContractGainPerTech,
            ContractGainPerLocal,
            ContractGain,
            KnowledgeGain,
            KnowledgeGainPerTech,
            KnowledgeGainPerLocal,
            ResourceGain,
            ResourceGainPerTech,
            ResourceGainPerLocal,
            BurstCoinGain,
            ClickGain,
            ClickGainCent,
            CoinGain,

            // Cost
            ContractCost,
            KnowledgeCost,
            CharacterCost,
            SpecialtyCost,
            ExpansionCost,
            ExpeditionCost,
            UpgradeCost,

            // Time
            ContractTime,

            // Unlock
            ContractUnlock,
            KnowledgeUnlock,
            CharacterUnlock,
            ResourceUnlock,
            CoinUnlock,
            ExpansionUnlock,
            TechUnlock,
            LocalUnlock,
            StageUnlock,

            // Others
            ContractLevelUnlock,
            ContractCapUnlock,
            ResourceCap,
            PartyCapSize,
        }

        public enum EffectSupertype 
        { 
            Cost,
            Gain,
            Time,
            Unlock,
            Cap,
            Offline,
        }


        public enum EffectOperation
        {
            Additive,
            Multiplicative,
            Override,
            Unlock
        }

        public enum EffectScope
        {
            Permanent,
            Expedition,
            Expansion
        }
    }
}
