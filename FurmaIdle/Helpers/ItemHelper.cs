namespace FurmaIdle.Helpers
{
    public class ItemHelper
    {
        public enum ItemType
        {
            Resource,
            Coin,
            Knowledge,
            Stage,
            Character,
            Tech,
            Local,
            Upgrade,
            Contract,
            Route,
            Ship,
            // Others
            Specialty,
            Expedition,
            Expansion,
            Click,
            Trait,
            Offline,
        }

        public enum UpgradeSubType
        {
            None,
            UnlockCharacter,
            UnlockContract,
            UnlockCoin,
            UnlockResource,
            UnlockStage,
            UnlockTech,
            UnlockKnowledge,
            UnlockLocal,
            UnlockExpansion,
            UnlockRoute,
            UnlockShip,
        }

        public enum CharacterType
        {
            None,
        }

        public enum CharacterClass
        {
            Artesão,
            Taberneiro,
            Pescador,
            Explorador,
            Caçador,
            Bardo,
        }
    }
}
