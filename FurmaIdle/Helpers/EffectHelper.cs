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
            // Knowledge
            KnowledgeUnlock,
            KnowledgeGain,
            KnowledgeCost,
            // Characters
            CharacterUnlock,
            CharacterCost,
            // Resources
            ResourceGain,
            ResourceUnlock,
            ResourceCap,
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
