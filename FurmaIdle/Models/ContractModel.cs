using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class ContractModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Quant { get; set; } = 0;
        public string Image { get; set; }
        public string FirstKnowId { get; set; }
        public string SecondKnowId { get; set; }
        public string ThirdKnowId { get; set; }
        public string FirstDiferential { get; set; }
        public string SecondDiferential { get; set; }
        public bool Unlocked {  get; set; }
        public string ConDestId { get; set; }
        public bool Avaliable { get; set; }
        public ResetPersistenceEnum.ResetPersistence Persistence { get; set; } = ResetPersistenceEnum.ResetPersistence.ExpeditionOnly;
    }
}
