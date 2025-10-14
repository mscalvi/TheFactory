namespace FurmaIdle.Helpers
{
    public class EnumHelper
    {
        public enum State
        { 
            Blocked,
            Unlocked,
            Avaliable,
            InBase,
            InStage
        }

        public enum Persistence
        {
            Permanent,
            untilExpansion,
            untilExpedition
        }
        public enum EffectOperation 
        { 
            Additive,
            Multiplicative, 
            Override,
            Unlock
        }
        public enum EffectType 
        { 
            ContractGain, 
            ContractTime, 
            ResourceGen, 
            ContractCap, 
            ClicksGain, 
            ResourceUnlock, 
            ResourceCapPerChar, 
            PartyCap 
        }

    }
}
