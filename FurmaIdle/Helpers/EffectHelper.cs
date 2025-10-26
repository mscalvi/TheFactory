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
            // Characters
            CharacterUnlock,
            CharacterCost,
            // Resources
            ResourceGain,
            ResourceUnlock,
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
