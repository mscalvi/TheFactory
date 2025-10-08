using FurmaIdle.Enums;

namespace FurmaIdle.Models
{
    public class CharacterModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MainKnowId { get; set; }
        public string SecondKnowId { get; set; }
        public CharTraitModel? Trait { get; set; }
        public string SpecialtyId { get; set; }
        public int Sort { get; init; }
        public string Image { get; set; }
        public string BigImage { get; set; }
        public string FullImage { get; set; }

        // Stats
        public CharStateEnum.CharState CharState { get; set; }
        public bool Avaliable { get; set; }
        public string? CharDestId { get; set; }
        public bool Unlocked { get; set; }

        // Contracts
        public List<string> KnowContractsIds { get; set; }
        public List<string> UnknowContractsIds { get; set; }
        public int MaxContracts { get; set; }
    }
}
