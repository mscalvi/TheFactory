namespace FurmaIdle.Helpers
{
    public class EffectHelper
    {
        public enum EffectType
        {
            // Gain
            ContractGainPerTech,
            ContractGainPerLocal,
            ContractGainPerCharacter,
            ContractGainPerExpeditionTime,
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
            ClickGainPerLocal,
            ClickGainPerCharacterInBase,
            ClickGainPerContractLevel,

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
            ContractTimePerExpansionTime,

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
            ShipUnlock,
            RouteUnlock,
            UpgradeUnlock,

            // Others
            ContractLevelUnlock,
            ContractCapUnlock,

            ResourceCap,
            ResourceCapPerUnlockedContract,

            PartyCapSize,
        }

        public enum EffectSupertype 
        { 
            Cost,
            Gain,
            Time,
            Unlock,
            ContractCap,
            ContractLevel,
            Offline,
            PartySize,
            ResourceCap,
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
