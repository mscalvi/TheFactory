using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class ResourceModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Total { get; set; }
        public double Actual { get; set; }
        public double PerSecond { get; set; }
        public double PendingFrac { get; set; }
        public bool Unlocked { get; set; } = false;
        public string UpgUnlockId { get; set; }
        public bool Avaliable { get; set; } = false;
        public string Image { get; set; }
        public int Sort { get; set; }
        public int CharacterCap { get; set; }
        public ResourceEnum.ResourceType ResourceType { get; set; }
        public ResetPersistenceEnum.ResetPersistence Persistence { get; set; } = ResetPersistenceEnum.ResetPersistence.ExpeditionOnly;
    }
}
