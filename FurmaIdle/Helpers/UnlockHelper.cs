namespace FurmaIdle.Helpers
{
    public class UnlockHelper
    {
        public enum State
        { 
            Blocked,
            Unlocked,
            Available
        }

        public enum Persistence
        {
            Permanent,
            untilExpansion,
            untilExpedition,
            untilTimer,
        }

        public enum CharState
        {
            Blocked,
            InBase,
            InStage,
            InLine
        }

        public enum ContractState
        {
            InUse,
            Available,
            Blocked,
        }

        public enum ExpeditionState
        {
            Active,
            Idle,
        }

        public enum ShipState
        {
            Blocked,
            InBase,
            InStage,
            InLine,
            InDiscovery,
            InRoute,
        }

        public enum RouteState
        {
            Blocked,
            Available,
            Known,
            Discovering,
        }
    }
}
