using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class LocalModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Lore { get; set; }
        public string? UnlockId { get; set; }
        public int? Level { get; set; }
        public UnlockHelper.State State { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; }
        public VersionHelper.UseState UseState { get; set; } = VersionHelper.UseState.InUse;
        public string StageId { get; set; }

        // Modifier
        public List<ModifierModel> Modifiers { get; set; }
    }
}
