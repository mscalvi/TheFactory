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
            InStage
        }
        public enum ExpeditionState
        {
            Active,
            Idle,
        }
    }
}
