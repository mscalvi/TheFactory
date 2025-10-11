using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class TechModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public string CostKnowledgeId { get; set; }
        public string Image { get; set; }
        public bool Unlocked { get; set; }
        public bool Avaliable { get; set; }
        public string DestinationId { get; set; }
        public ResetPersistenceEnum.ResetPersistence Persistence { get; set; } = ResetPersistenceEnum.ResetPersistence.ExpansionOnly;
    }
}
