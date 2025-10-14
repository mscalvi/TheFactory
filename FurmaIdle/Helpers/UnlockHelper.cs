namespace FurmaIdle.Helpers
{
    public class UnlockHelper
    {
        public enum State
        { 
            Blocked,
            Unlocked,
            Avaliable
        }

        public enum Persistence
        {
            Permanent,
            untilExpansion,
            untilExpedition
        }
        public enum CharState
        {
            Blocked,
            InBase,
            OnStage
        }
    }
}
