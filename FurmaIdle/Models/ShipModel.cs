using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class ShipModel
    {
        // Basics
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Lore { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public string NavIcon { get; set; }
        public string BigImage { get; set; }
        public VersionHelper.UseState UseState { get; set; } = VersionHelper.UseState.InUse;

        // Modifier
        public List<ModifierModel> Modifiers { get; set; }

        // Status
        public string UnlockId { get; set; }
        public UnlockHelper.State State { get; set; }
        public UnlockHelper.ShipState ShipState { get; set; }
        public string? InStageId { get; set; }
        public string? InRouteId { get; set; }
        public double TravelProgress { get; set; }
        public UnlockHelper.Persistence Persistence { get; set; } = UnlockHelper.Persistence.Permanent;

        // Atributes
        public double Speed { get; set; }
        public int MaxTripulation { get; set; }
    }
}
