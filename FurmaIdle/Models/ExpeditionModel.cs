using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class ExpeditionModel
    {
        public string StageId { get; set; }
        public string ExpansionId { get; set; }
        public List<string> PartyId { get; set; } = new();
        public DateTimeOffset? Start { get; set; }
        public ExpeditionEnum.ExpeditionStatus ExpeditionStatus { get; set; }
    }
}
