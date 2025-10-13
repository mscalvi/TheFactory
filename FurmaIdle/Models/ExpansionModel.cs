using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class ExpansionModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string DestUnlockId { get; set; }
        public int Cost { get; set; }
        public string CoinCostId { get; set; }
        public bool Avaliable { get; set; }
        public bool Unlocked { get; set; }
        public ResetPersistenceEnum.ResetPersistence Persistence { get; set; } = ResetPersistenceEnum.ResetPersistence.Permanent;
    }
}
