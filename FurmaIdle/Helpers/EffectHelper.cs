namespace FurmaIdle.Helpers
{
    public class EffectHelper
    {
        public enum EffectType
        {
            // Contracts
            ContractGain,
            ContractTime,
            ContractCost,
            ContractUnlock,
            ContractLevelUnlock,
            ContractCapUnlock,

            ContractGainPerTech,
            ContractGainPerLocal,

            // Knowledge
            KnowledgeUnlock,
            KnowledgeGain,
            KnowledgeCost,

            KnowledgeGainPerTech,
            KnowledgeGainPerLocal,

            // Characters
            CharacterUnlock,
            CharacterCost,

            // Resources
            ResourceGain,
            ResourceUnlock,
            ResourceCap,

            ResourceGainPerTech,
            ResourceGainPerLocal,

            // Coins
            CoinGain,
            BurstCoinGain,
            // Specialties
            SpecialtyCost,
            // Clicks
            ClickGain,
            // Stages
            StageUnlock,
            PartyCapSize,
            // Locals
            LocalUnlock,
            // Tech
            TechUnlock,
            // Expansions
            ExpansionUnlock,
            ExpansionCost,
            // Expedition
            ExpeditionCost,
            ExpeditionGain,
            // Upgrade
            UpgradeCost,
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
