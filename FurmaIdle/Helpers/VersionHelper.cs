namespace FurmaIdle.Helpers
{
    public class VersionHelper
    {
        public const int SchemaVersion = 1;
        public const int GameVersion = 1;

        public const string BuildVersion = "1.1.0";

        public enum UseState
        {
            InUse,
            Idle,
            Legacy,
        }
    }
}
