using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class CharacterModel
    {
        // Basics
        public string Id { get; set; }
        public ItemHelper.ItemType ItemType { get; set; } = ItemHelper.ItemType.Character;
        public ItemHelper.CharacterType CharacterType { get; set; } = ItemHelper.CharacterType.None;
        public string Name { get; set; }
        public ItemHelper.CharacterClass Class { get; set; }
        public string Description { get; set; }
        public string Lore { get; set; }
        public string Icon { get; set; }
        public string NavIcon { get; set; }
        public string Image { get; set; }
        public string BigImage { get; set; }
        public VersionHelper.UseState UseState { get; set; } = VersionHelper.UseState.InUse;

        // Modifier
        public List<ModifierModel> Modifiers { get; set; }

        // Status
        public string UnlockId { get; set; }
        public UnlockHelper.State State { get; set; }
        public UnlockHelper.CharState CharState { get; set; }
        public string? InStageId { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; } = UnlockHelper.Persistence.Permanent;

        // Contracts
        public List<string> ContractsIds { get; set; }

        // Knowledge
        public string? KnowledgeFactor1 { get; set; }
        public string? KnowledgeFactor2 { get; set; }

        // Traits e Speciality
        public string TraitId { get; set; }
        public string SpecialtyId { get; set; }
    }
}
