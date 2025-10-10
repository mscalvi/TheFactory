using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class DestinationModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Cost { get; set; }
        public string CostResourceId { get; set; }
        public string Image { get; set; }
        public bool Unlocked { get; set; }
        public string StageId { get; set; }
        public bool Avaliable { get; set; }
        public ResetPersistenceEnum.ResetPersistence Persistence { get; set; } = ResetPersistenceEnum.ResetPersistence.ExpansionOnly;
    }
}
