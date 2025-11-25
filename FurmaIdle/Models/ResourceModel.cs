using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class ResourceModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UnlockId { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public string Lore { get; set; }
        public string Description { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; }
        public UnlockHelper.State State { get; set; }
        public VersionHelper.UseState UseState { get; set; } = VersionHelper.UseState.InUse;

        public double RsPerSecond { get; set; }
        public int RsPerChar { get; set; }

        public double RegenActual { get; set; } = 0;

        // Modifier
        public List<ModifierModel> Modifiers { get; set; }
    }
}
