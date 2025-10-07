using FurmaIdle.Enums;

namespace FurmaIdle.Models
{
    public class CharacterModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MainKnowId { get; set; }
        public string SecondKnowId { get; set; }
        public List<string> KnowContractsIds { get; set; }
        public List<string> UnknowContractsIds { get; set; }
        public CharTraitModel Trait { get; set; }
        public string SpecialtyId { get; set; }
        public int Sort { get; init; }

        // Stats
        public CharStateEnum.CharState CharState { get; set; }
        public string? CharStageId { get; set; }
        public bool StartUnlocked { get; init; }
    }
}
