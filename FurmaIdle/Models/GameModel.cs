namespace FurmaIdle.Models
{
    public class GameModel
    {
        public int SchemaVersion { get; set; }
        public DateTime LastTick { get; set; }
        public Dictionary<string, ResourceModel> Resources { get; set; } = new();
        public Dictionary<string, StageModel> Stages { get; set; } = new();
        public Dictionary<string, ClickModel> Clicks { get; set; } = new();

    }
}
