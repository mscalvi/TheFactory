using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class RouteModel
    {
        // Basics
        public string Id { get; set; }
        public string Name { get; set; }
        public ItemHelper.ItemType ItemType { get; set; } = ItemHelper.ItemType.Route;
        public string Description { get; set; }
        public string Lore { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public VersionHelper.UseState UseState { get; set; } = VersionHelper.UseState.InUse;

        // Modifier
        public List<ModifierModel> Modifiers { get; set; }

        // Status
        public string UnlockId { get; set; }
        public UnlockHelper.State State { get; set; }
        public UnlockHelper.RouteState RouteState { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; } = UnlockHelper.Persistence.Permanent;

        // Atributes
        public string PointA { get; set; }
        public string PointB { get; set; }
        public double Distance { get; set; }
    }
}
