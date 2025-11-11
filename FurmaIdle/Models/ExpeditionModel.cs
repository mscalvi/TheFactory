using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class ExpeditionModel
    {
        public string StageId { get; set; } = "";                
        public List<string> PartyIds { get; set; } = new();
        public UnlockHelper.ExpeditionState? ExpeditionState { get; set; } = UnlockHelper.ExpeditionState.Idle;
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? FinishedAt { get; set; }

        public double CurrentCoinPerSec { get; set; } = 0.0;

        // Modifiers
        public List<ModifierModel> Modifiers { get; set; }
    }
}
