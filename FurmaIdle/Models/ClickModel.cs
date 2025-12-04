using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class ClickModel
    {
        public string Id { get; set; }
        public ItemHelper.ItemType ItemType { get; set; } = ItemHelper.ItemType.Click;
        public string StageId { get; set; }
        public double BaseGain { get; set; }
        public VersionHelper.UseState UseState { get; set; } = VersionHelper.UseState.InUse;
        public string Icon { get; set; }

        // Modifiers
        public List<ModifierModel> Modifiers { get; set; }
    }
}
