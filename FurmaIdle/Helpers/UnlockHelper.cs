namespace FurmaIdle.Helpers
{
    public class UnlockHelper
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
    }
}
