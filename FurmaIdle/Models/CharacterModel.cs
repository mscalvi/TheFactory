using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class CharacterModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MainKnowId { get; set; }
        public string SecondKnowId { get; set; }
        public string? TraitId { get; set; }
        public string SpecialtyId { get; set; }
        public int Sort { get; init; }
        public string Image { get; set; }
        public string BigImage { get; set; }
        public string FullImage { get; set; }
        public ResetPersistenceEnum.ResetPersistence Persistence { get; set; } = ResetPersistenceEnum.ResetPersistence.Permanent;

        // Stats
        public CharStateEnum.CharState CharState { get; set; }
        public bool Avaliable { get; set; }
        public string? CharDestId { get; set; }
        public string CharStageId { get; set; }
        public bool Unlocked { get; set; }
        public int Cost { get; set; }
        public string CostResourceId { get; set; }

        // Contracts
        public List<string> KnowContractsIds { get; set; }
        public List<string> UnknowContractsIds { get; set; }
        public int MaxContracts { get; set; }
    }
}
