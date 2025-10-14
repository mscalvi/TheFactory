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
            // Specialties
            SpecialtyCost,
            // Clicks
            ClickGain,
            // Stages
            StageUnlock
        }

        public enum EffectOperation
        {
            Additive,
            Multiplicative,
            Override,
            Unlock
        }
    }
}
